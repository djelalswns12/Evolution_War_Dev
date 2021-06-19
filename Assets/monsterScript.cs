using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class monsterScript :  MonoBehaviourPunCallbacks,IPunObservable
{
    public int flystate; // 공중 여부 체크
    private int moveMethod; // 캥거루 걸음 걸이 체크
    private float setSpeed;
    private PlayerScript Myplayer;
    private Animator anim;
    private Rigidbody2D rigid;
    private SpriteOutline myOutline;
    private bool outlineCheck;
    public bool dir;//True:왼쪽>오른쪽 False:오른쪽>왼쪽
    public int movestate; // 움직임 상태번호
    public bool movestateChange;
    public int creatnumber;
    public bool someThingBlock; // 앞에 무엇인가 막혔는지 정하는 변수

    public PhotonView pv;
    public SpriteRenderer sp;

    public LayerMask whatIsLayer;//길막 유닛 체크 마스크
    public LayerMask whatIsLayer2; //공격대상 체크 마스크
    public bool attackPlayer; // 공격중인지 확인 하는 변수

    public int myIndex;
    public string myName;
    public bool killTarget;

    public int bossAttackSignal;
    public int redPoint, bluePoint;
    public Text stateText; // 몬스터 체력바 위의 텍스트 이다.
    [Header("건들여야하는것")]

    public Text redtxt;
    public Text bluetxt;
    public float mhp, hp;
    public float speed;
    public int dieMoneyGet;
    public int damage;
    public int dropMoney;

    public Vector2 attOffset, size;//중첩 확인
    public Vector2 attOffset2, size2;//자신앞에 유닛 존재여부 확인
    public Vector2 attOffset3, size3;//공격범위에 적있는지 확인
    public Vector2 throwPos;//던지기 위치

    // Start is called before the first frame update
    void Awake()
    {
        if(transform.tag!="Test")
        gameObject.layer = LayerMask.NameToLayer("Nothing");
    }
    void Start()
    {
        if (stateText != null)
        {
            stateText.text = "";
        }
        //myIndex = NetworkMaster.Instance.SetMonsterNametoIndex(myName);
        //dropMoney=int.Parse(NetworkMaster.Instance.GetMonsterOption(myIndex, 2);
        if (transform.tag == "Player")
        {
            Myplayer = GetComponent<PlayerScript>();
        }
        myOutline = GetComponent<SpriteOutline>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        SetMask(sp); // 마스크에 가려질 오브젝트인지 확인한다 player일 경우 무시한다
        pv = GetComponent<PhotonView>();
        setSpeed = speed;
        if (transform.tag != "Test")
            dir = NetworkMaster.player.GetComponent<PlayerScript>().dir; //테스트 몹이 아니라면 플레이어 dir을 받아온다

        dir = pv.IsMine ? dir : !dir; // 나의 몬스터라면 플레이어의 dir을 아니라면 반대를 적용한다.
        sp.flipX = dir;
            
        //외곽선 표시기능
            if (transform.tag == "monster" || transform.tag == "Player" || transform.tag == "trap")
            {
            if (pv.IsMine)
            {
                outlineCheck = false;
            }
            else
            {
                outlineCheck = true;
            }
            }
            else if (transform.tag == "boss")
            {
                outlineCheck = true;
            }
        myOutline.enabled = outlineCheck;
    }
    // Update is called once per frame

    void Update()
    {
        #region 다른 PC에서도 실행하는 영역
        //레이어 미설정되었을 경우 rigidbody 적용을 막는 역할
        if (LayerMask.LayerToName(gameObject.layer) == "Nothing")
        {
            rigid.isKinematic = true;
        }
        else
        {
            rigid.isKinematic = false;
        }
        if (gameObject.tag == "Player")
        {
            SetColiderLayerPlayer();
        }
        else
        {
            //일반몬스터+보스
            SetColiderLayer();
        }
        sp.sortingLayerName = LayerMask.LayerToName(gameObject.layer);//렌더링 우선순위 설정
        if (gameObject.tag == "boss")
        {
            redtxt.text = redPoint.ToString();
            bluetxt.text = bluePoint.ToString();
            SetFocusMonster();
        }
        #endregion

        #region 상대방 캐릭터 행위 기술
        if (pv.IsMine == false)
        {
            if (gameObject.tag == "Player")
            {
                //나의 몬스터가 상대방의 플레이어의 가시범위에 닿았으면 시야를 적 플레이어만큼 확대시켜주는 역할
                if (FrontEnemyThere())
                {
                    SetFocusMonster();
                }else if (MainGameManager.mainGameManager.GetFocus() == gameObject)
                {
                    MainGameManager.mainGameManager.SetFocus(null);
                }
            }
        }
        #endregion 상대방 캐릭터 행위 기술
        #region 본인 캐릭터 행위 기술
        else
        {
            Setdir();

            if (hp <= 0)
            {
                if (gameObject.tag == "Player")
                {

                }
                else
                {
                    pv.RPC("MonsterDie", RpcTarget.All,killTarget);
                    //Destroy(gameObject);
                    return;
                }
            }

            SetFocusMonster();
            if (gameObject.tag == "monster")
            {
                PlayMonster();

            }
            else if (gameObject.tag == "Player")
            {
                PlayPlayer();
            }
            else if (gameObject.tag == "Test")
            {
                PlayBoss();
            }
            else if (gameObject.tag == "boss")
            {
                PlayBoss();
            }
            else if (gameObject.tag == "trap")
            {
                PlayTrap();
            }

        }
        #endregion
    }//Update End

    //////////////////////////////
    #region Method
    //////////////////////////////
    #region Method -> InGameMethod
    private void SetColiderLayer()
    {
        if (gameObject.tag != "trap")
        {
            whatIsLayer = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer)) + LayerMask.GetMask("centerunit");//본인레이어 유닛과 플레이어와의 가로막힘을 위한 레이어추가
            if (flystate == 1)
            {
                //공중 유닛 이라면
                if (LayerMask.LayerToName(gameObject.layer) == "downflyunit")
                {
                    //아래 공중유닛일 경우
                    whatIsLayer2 = whatIsLayer + LayerMask.GetMask("downunit");
                }
                else
                {
                    //윗 공중유닛일 경우
                    whatIsLayer2 = whatIsLayer + LayerMask.GetMask("upunit");
                }
            }
            else
            {
                //지상 유닛이라면
                if (LayerMask.LayerToName(gameObject.layer) == "downunit")
                {
                    whatIsLayer2 = whatIsLayer + LayerMask.GetMask("downflyunit");
                }
                else
                {
                    whatIsLayer2 = whatIsLayer + LayerMask.GetMask("upflyunit");
                }
            }
        }
        else if (gameObject.tag == "trap")
        {
            if (flystate == 1)
            {
                //공중 유닛 이라면
                if (LayerMask.LayerToName(gameObject.layer) == "downTrap")
                {
                    //아래 공중유닛일 경우
                    whatIsLayer = LayerMask.GetMask("downunit");
                    whatIsLayer2 = whatIsLayer + LayerMask.GetMask("downflyunit");
                }
                else
                {
                    //윗 공중유닛일 경우
                    whatIsLayer = LayerMask.GetMask("upunit");
                    whatIsLayer2 = whatIsLayer + LayerMask.GetMask("upflyunit");
                }
            }
            else
            {
                //지상 유닛이라면
                if (LayerMask.LayerToName(gameObject.layer) == "downTrap")
                {
                    whatIsLayer = LayerMask.GetMask("downunit");
                    whatIsLayer2 = whatIsLayer + LayerMask.GetMask("downflyunit");
                }
                else
                {
                    whatIsLayer = LayerMask.GetMask("upunit");
                    whatIsLayer2 = whatIsLayer + LayerMask.GetMask("upflyunit");
                }
            }
        }
    }
    private void SetColiderLayerPlayer()
    {
        whatIsLayer2 = LayerMask.GetMask("downunit")+ LayerMask.GetMask("downlfyunit")+ LayerMask.GetMask("upunit") + LayerMask.GetMask("upflyunit");//본인레이어 유닛과 플레이어와의 가로막힘을 위한 레이어추가
    }
    void PlayPlayer()
    {

    }
    void PlayTrap()
    {
        if (!UnitThereExceptTrap())
        {
            //앞에 무언가가 없다면
            animReset();
            movestate = 0;
        }
        else
        {
            //앞에 무언가가 있다면
            movestate = 1;
        }
        if (movestate == 1)
        {
            Collider2D[] hit2 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
            for (int i = 0; i < hit2.Length; i++)
            {
                if (hit2[i].gameObject == gameObject)
                {
                    continue;
                }
                if (hit2[i].tag == "monster" && hit2[i].GetComponent<PhotonView>().IsMine == true)
                {
                    anim.SetBool("attack", true);
                    break;
                }
                if (hit2[i].tag == "boss")
                {

                }
                if (hit2[i].tag == "Test")
                {

                }
                if (hit2[i].tag == "Player" && hit2[i].GetComponent<PhotonView>().IsMine == false)
                {

                }
                animReset();
            }
        }
    }
    void PlayMonster()
    {
        //txt.text = "";//creatnumber.ToString();
        if (!FrontUnitThere() && !FrontEnemyThere())
        {
            //앞에 무언가가 없다면
            animReset();
            someThingBlock = false;
            OverlapUnit();
        }
        else
        {
            //앞에 무언가가 있다면
            movestate = 1;
        }

        if (movestate == 0)
        {//자유로운 상태
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("move"))
            {
                if (moveMethod == 0)
                {
                    transform.position += Vector3.right * speed * (dir == true ? 1 : -1) * Time.deltaTime * NetworkMaster.Instance.editorSpeed;
                }
            }


        }
        else if (movestate == 1)
        {//중첩 또는 앞에 유닛이 존재할경우
            Collider2D[] hit2 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
            for (int i = 0; i < hit2.Length; i++)
            {
                if (hit2[i].gameObject == gameObject)
                {
                    continue;
                }
                if (hit2[i].tag == "monster" && hit2[i].GetComponent<PhotonView>().IsMine == false)
                {
                    anim.SetBool("attack", true);
                    break;
                }
                if (hit2[i].tag == "boss")
                {
                    anim.SetBool("attack", true);
                    break;
                }
                if (hit2[i].tag == "Test")
                {
                    anim.SetBool("attack", true);
                    break;
                }
                if (hit2[i].tag == "Player" && hit2[i].GetComponent<PhotonView>().IsMine == false)
                {
                    anim.SetBool("attack", true);
                    break;
                }
                animReset();
            }
        }
        if (NetworkMaster.Instance.endPoint==2)
        {
            animReset();
            movestate = 2;
        }
        //if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        //{
        //    animReset();
        //}
   
    }
    void PlayBoss()
    {
        /*
        n초 간격으로 공격 실행
         */
        if (bossAttackSignal == 0) 
        {
            bossAttackSignal = 1;
            float attackTimer = Random.Range(float.Parse(NetworkMaster.Instance.GetMonsterOption(myName, "attackMinTimer")), float.Parse(NetworkMaster.Instance.GetMonsterOption(myName, "attackMaxTimer")));
            StartCoroutine(bossHit(attackTimer));
        }
        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("attack")+ "//도착지점:"+ anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            animReset();
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SingleAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {
            animReset();
        }
    }
    IEnumerator bossHit(float attackTimer)
    {
        float attackType = Random.Range(0,100);
        if (attackType < int.Parse(NetworkMaster.Instance.GetMonsterOption(myName, "attackType")))
        {
            anim.SetBool("attack", true);
        }
        else
        {
            Debug.Log(anim.GetBool("SingleAttack"));
            anim.SetBool("SingleAttack", true);
        }
        yield return new WaitForSeconds(attackTimer);
        bossAttackSignal = 0;
    }
    public void Setdir()
    {
        if (gameObject.tag == "monster")
        {
            sp.flipX = dir;
        }
        else if (gameObject.tag == "Player")
        {
            sp.flipX = Myplayer.dir;
        }
    }
    public void animReset()
    {
        anim.SetBool("attack", false);
        anim.SetBool("SingleAttack", false);
    }
    public void SetMask(SpriteRenderer sp)
    {
        if (gameObject.tag == "Player")
        {
            return;
        }
        sp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask; // 마스크 확인
        
    }
    public void moveMethodSet(int metohd)
    {
        moveMethod = metohd;
    }
    bool UnitThereExceptTrap()
    {
        Collider2D[] hit2 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
        for (int i = 0; i < hit2.Length; i++)
        {
            if (hit2[i].GetComponent<monsterScript>().dir != dir && hit2[i].tag != "trap" && hit2[i].tag != "boss" && hit2[i].tag != "Player")
            {
                return true;
            }
        }
        return false;
    }
    bool FrontEnemyThere()
    {
        //자신앞에 유닛 존재여부 확인
        Collider2D[] hit2 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
        for (int i = 0; i < hit2.Length; i++)
        {
            //if ((dir == false && hit2[i].transform.position.x < transform.position.x) || (dir == true && hit2[i].transform.position.x > transform.position.x))
            if (hit2[i].GetComponent<monsterScript>().dir != dir || hit2[i].tag=="boss")
            {
                if (hit2[i].gameObject == gameObject)
                {
                    continue;
                }
                if (hit2[i].gameObject == NetworkMaster.player)
                {
                    //Debug.Log("본인 player 콜라이더가 걸렸으므로 무시해야함");
                    return false;
                }
                if (hit2[i].tag == "boss"  || hit2[i].tag== "Test" /*|| hit2[i].tag == "Player"*/)
                {
                    //다중공격 가능한 것들
                    attackPlayer = true;
                }
                else
                {
                    //단일공격 하는 것들
                    attackPlayer = false;
                }
                //가로막는 유닛이 다중공격대상이 아니라면 가로막힌것이므로 전진불가
                if (hit2[i].GetComponent<monsterScript>().attackPlayer == false)
                {
                    someThingBlock = true;
                    return true;
                }
            }
        }
        attackPlayer = false;
        return false;
    }
    bool FrontUnitThere()
    {
        //자신앞에 유닛 존재여부 확인
        Collider2D[] hit2 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset2.x * (dir == true ? -1 : 1)), transform.position.y + attOffset2.y), size2, 0, whatIsLayer);
        for (int i = 0; i < hit2.Length; i++)
        {
            //if ((dir == false && hit2[i].transform.position.x < transform.position.x) || (dir == true && hit2[i].transform.position.x > transform.position.x))
            if((hit2[i].GetComponent<monsterScript>().creatnumber < creatnumber && hit2[i].GetComponent<monsterScript>().attackPlayer == false) || hit2[i].GetComponent<monsterScript>().dir!=dir)
            {
                if (hit2[i].gameObject == gameObject)
                {
                    continue;
                }
                if (hit2[i].gameObject == NetworkMaster.player)
                {
                   // Debug.Log("본인 player 콜라이더가 걸렸으므로 무시해야함");
                    return false;
                }
                if (hit2[i].tag == "boss"  || hit2[i].tag == "Test")
                {
                    attackPlayer = true;
                }
                else
                {
                    attackPlayer = false;
                }
                //가로막는 유닛이 플레이어나 보스를 공격중이 아니라면 가로막혀서 전진불가
                if (hit2[i].GetComponent<monsterScript>().attackPlayer == false)
                {
                    someThingBlock = true;
                    return true;
                }
            }
        }
        attackPlayer = false;
        return false;
    }
    public void OverlapUnit()
    {
        
        Collider2D[] hit = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset.x * (dir == true ? -1 : 1)), transform.position.y + attOffset.y), size, 0, whatIsLayer);

        if (hit.Length == 1) 
        {
            movestate = 0;
            movestateChange = false;
        }
        else
        {
            //movestate가 true이면 이동불가 상태로 갈 수 있다는 뜻이다.
            //movestateChange를 이용하여 release함수에 의해 선택된 아이만 movestate가 변할 수 있게 한다.
            movestateChange = true; 
            NetworkMaster.Instance.GetComponent<overlapScript>().release(hit);
            //네트워크 마스터에서 중첩된 유닛중 우선순위가 가장높은 객체를 골라 movestatChange를 false 시키고 movestate를 0으로 만들어서 이동시킨다.
            if (movestateChange == true)
            {
                movestate = 1;
            }
      
        }
    }

    public void MonseterAttack()
    {
        if (pv.IsMine == true)
        {
            Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
            if (hitArea.Length > 0)
            {
                for (int i = 0; i < hitArea.Length; i++)
                {
                    monsterScript target=hitArea[i].gameObject.GetComponent<monsterScript>();
                    if ((target.dir!=this.dir && (target.tag=="monster" || target.tag == "Player")) || target.tag== "boss" || target.tag == "Test")
                    {
                        if (hitArea.Length > 2 && hitArea[i].tag == "Player")
                        {
                            continue;
                            //플레이어 공격중에 캐릭터 생성된다면 공격타겟을 바꿔줘야하기 때문
                        }
                        //pv.RPC("BossHit", RpcTarget.All, 2);
                        target.pv.RPC("GetDamage", RpcTarget.All, damage,dieMoneyGet, NetworkMaster.Instance.dir);
                        return;
                    }
                }
            }
        }
    }
        //애니메이션에서 실행됨
    public void MonseterThrow(string name)
    {
        if (pv.IsMine == true)
        {
            Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
            if (hitArea.Length > 0)
            {
                for (int i = 0; i < hitArea.Length; i++)
                {
                    monsterScript target = hitArea[i].gameObject.GetComponent<monsterScript>();
                    if ((target.dir != this.dir && (target.tag == "monster" || target.tag == "Player")) || target.tag == "boss" || target.tag == "Test")
                    {
                        if (hitArea.Length > 2 && hitArea[i].tag == "Player")
                        {
                            continue;
                            //플레이어 공격중에 캐릭터 생성된다면 공격타겟을 바꿔줘야하기 때문
                        }
                        //pv.RPC("BossHit", RpcTarget.All, 2);
                        NetworkMaster.Instance.CreatThrow(name, new Vector2(transform.position.x + (throwPos.x * (dir == true ? -1 : 1)), transform.position.y + throwPos.y), damage,this, target.gameObject);
                        return;
                    }
                }
            }
        }
    }
    public void resetSpeed()
    {
        movestateChange = false;
        movestate = 0;
    }
    public void RpcCallGetDamage(int damage,int dieMoneyGet,bool dir)
    {
       pv.RPC("GetDamage",RpcTarget.All, damage, dieMoneyGet, dir);
    }
    public void SetFocusMonster()
    {
  
            if (MainGameManager.mainGameManager.GetFocus() != null)
            {
                if (MainGameManager.mainGameManager.GetFocus().transform.position.x < transform.position.x )
                {
                    //왼쪽 플레이어
                    if (NetworkMaster.Instance.dir == true)
                    {
                        MainGameManager.mainGameManager.SetFocus(gameObject);
                    }
                }
                else
                {
                    //오른쪽 플레이어
                if (NetworkMaster.Instance.dir == false)
                {
                        MainGameManager.mainGameManager.SetFocus(gameObject);
                }
                }
            }
            else
            {
                    MainGameManager.mainGameManager.SetFocus(gameObject);
            }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (attOffset.x * (dir == true ? -1 : 1)), transform.position.y + attOffset.y), size);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (attOffset2.x * (dir == true ? -1 : 1)), transform.position.y + attOffset2.y), size2);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + (throwPos.x * (dir == true ? -1 : 1)), transform.position.y + throwPos.y),0.1f);
    }
    #endregion
    //////////////////////////////

    #region Method > OnlineMetohd
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    
        if (stream.IsWriting)
        {
            stream.SendNext(dir);
            stream.SendNext(mhp);
            stream.SendNext(hp);
            stream.SendNext(LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer)));
            stream.SendNext(gameObject.layer);
            stream.SendNext(flystate);
            stream.SendNext(dropMoney);
            stream.SendNext(redPoint);
            stream.SendNext(bluePoint);
            stream.SendNext(myName);
            stream.SendNext(sp.flipX);
        }
        else
        {
            dir = (bool)stream.ReceiveNext();
            mhp=(float)stream.ReceiveNext();
            hp=(float)stream.ReceiveNext();
            whatIsLayer=(int)stream.ReceiveNext();
            gameObject.layer = (int)stream.ReceiveNext();
            flystate = (int)stream.ReceiveNext();
            dropMoney = (int)stream.ReceiveNext();
            redPoint=(int)stream.ReceiveNext();
            bluePoint=(int)stream.ReceiveNext();
            myName= (string)stream.ReceiveNext();
            sp.flipX = (bool)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public void rpcTest()
    {
        Debug.Log("hi");
    }
    [PunRPC]
    public void PointCal(int damage,bool team)
    {
        if (pv.IsMine)
        {
            if (hp <= 0)
                return;

            if (redPoint + bluePoint + damage > mhp)
            {
                damage = (int)mhp - (redPoint + bluePoint);
            }
            if (team)
            {
                redPoint += damage;
            }
            else
            {
                bluePoint += damage;
            }
        }
    }
    [PunRPC]
    public void CrowdControl(Vector3 setPos,float xForce,float yForce)
    {
        rigid.AddForce(Vector2.up * yForce,ForceMode2D.Impulse);
        if(setPos.x > transform.position.x)
        {
            rigid.AddForce(Vector2.left * xForce, ForceMode2D.Impulse);
        }
        else
        {
            rigid.AddForce(Vector2.right * xForce, ForceMode2D.Impulse);
        }
        
    }
    [PunRPC]
    public void GetDamage(int damage,int moneyGet,bool requestDir)
    {
        if (pv.IsMine)
        {
            pv.RPC("PointCal", RpcTarget.All, damage, requestDir);
           
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["isTest"] || (NetworkMaster.otherPlayer && NetworkMaster.otherPlayer.GetComponent<monsterScript>().hp > 0))
            {
                //상대방이 살아있고
                if (hp - damage < 0)
                {
                    
                    if (moneyGet == 1)
                    {
                        //죽기전 받은 공격의 소유자가 중립(보스 등)일 경우
                        //killTarget은 false가 되고 돈을 받을 수 없다.
                        killTarget = false;
                    }
                    else
                    {
                        //죽기전 받은 공격의 소유자가 상대방일 경우
                        //killTarget은 true가 되고 돈을 받을 수 있다.
                        killTarget = true;
                    }
                }
                hp -= damage;
            }
            else
            {
                Debug.Log("이미 당신이 이겼으므로 공격받지 않습니다.");
            }
        }
    }

    [PunRPC]
    public void MonsterDie(bool isKillByEnemy=false)
    {
        if (pv.IsMine == false&& gameObject.tag=="monster")
        {
            if (isKillByEnemy == true)
            {
                //내가 죽인게 맞다면 돈을 준다.
            MainGameManager.mainGameManager.CreatGoldEffect(transform.position, (int)(dropMoney * MainGameManager.mainGameManager.dropGoldEff));
            }
        }
        else if (gameObject.tag == "boss")
        {
            int GetMoney = (int)((dropMoney) * (NetworkMaster.Instance.dir == true ? redPoint / mhp : bluePoint / mhp));
            if (pv.IsMine)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (redPoint > bluePoint)
                    {
                        NetworkMaster.Instance.SendMainMsgFunc($"<color=red>RED</color>팀이 보스에게 더많은 피해를 입혔습니다.", 1);
                    }
                    else
                    {
                        NetworkMaster.Instance.SendMainMsgFunc($"<color=blue>BLUE</color>팀이 보스에게 더많은 피해를 입혔습니다.", 1);
                    }
                    //NetworkMaster.Instance.SendGameMsgFunc($"<color=blue>BLUE:</color> {GetMoney}<color=yellow>G</color>  <color=red>RED:</color> {dropMoney - GetMoney}<color=yellow>G</color>", 1);
                    Debug.Log($"{myName} 다음 보스를 소환합니다.");
                    NetworkMaster.Instance.SetNextStage(myName);
                }
            }
            MainGameManager.mainGameManager.CreatGoldEffect(transform.position,GetMoney ); // 드랍머니를 공격 비율만큼 곱한값으로 설정해야한다
            
        }
     Destroy(gameObject);
    }
    #endregion
    #endregion
}
