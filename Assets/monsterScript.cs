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

    public int redPoint, bluePoint;
    public Text stateText;
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
        sp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        pv = GetComponent<PhotonView>();
        setSpeed = speed;
        if (transform.tag != "Test")
            dir = NetworkMaster.player.GetComponent<PlayerScript>().dir;

        dir = pv.IsMine ? dir : !dir;
        sp.flipX = dir;

            if (transform.tag == "monster")
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
        whatIsLayer = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer))+ LayerMask.GetMask("centerunit");//본인레이어+플레이어 공격을 위한 레이어추가
        
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

        sp.sortingLayerName = LayerMask.LayerToName(gameObject.layer);
        if (gameObject.tag == "boss")
        {
            redtxt.text = redPoint.ToString();
            bluetxt.text = bluePoint.ToString();
            SetFocusMonster();
        }

        #endregion

        #region 본인 캐릭터만 실행 영역
        if (pv.IsMine == false) 
            return;
   
        Setdir();

        if (hp <= 0)
        {
            if (gameObject.tag == "Player")
            {

            }
            else{
                pv.RPC("MonsterDie", RpcTarget.All);
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
        #endregion
    }

    //////////////////////////////
    #region Method
    //////////////////////////////
    #region Method -> InGameMethod
    void PlayPlayer()
    {
        
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
    }
    public void moveMethodSet(int metohd)
    {
        moveMethod = metohd;
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
    public void SetFocusMonster()
    {
  
            if (MainGameManager.mainGameManager.GetFocus() != null)
            {
                if (MainGameManager.mainGameManager.GetFocus().transform.position.x < transform.position.x)
                {
                    if (NetworkMaster.Instance.dir == true)
                    {
                        MainGameManager.mainGameManager.SetFocus(gameObject);
                    }
                }
                else
                {
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
    public void GetDamage(int damage,int moneyGet,bool requestDir)
    {
        if (pv.IsMine)
        {
            pv.RPC("PointCal", RpcTarget.All, damage, requestDir);
           
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["isTest"] || (NetworkMaster.otherPlayer && NetworkMaster.otherPlayer.GetComponent<monsterScript>().hp > 0))
            {
                if (hp - damage < 0)
                {
                    if (moneyGet == 1)
                    {
                        killTarget = false;
                    }
                    else
                    {
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
    public void MonsterDie()
    {
        if (pv.IsMine == false&& gameObject.tag=="monster")
        {
            if (killTarget == true)
            {
                //내가 죽인게 맞다면 돈을 준다.
            MainGameManager.mainGameManager.CreatGoldEffect(transform.position, (int)(dropMoney * MainGameManager.mainGameManager.dropGoldEff));
            }
        }
        else if (gameObject.tag == "boss")
        {
            MainGameManager.mainGameManager.CreatGoldEffect(transform.position, (int)((dropMoney)*(NetworkMaster.Instance.dir==true ? redPoint/mhp:bluePoint/mhp))); // 드랍머니를 공격 비율만큼 곱한값으로 설정해야한다
        }
     Destroy(gameObject);
    }
    #endregion
    #endregion
}
