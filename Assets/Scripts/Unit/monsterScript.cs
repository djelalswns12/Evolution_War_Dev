using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class monsterScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public int flystate; // 공중 여부 체크
    private int moveMethod; // 캥거루 걸음 걸이 체크
    private float setSpeed;
    private Animator anim;
    private Rigidbody2D rigid;
    private SpriteOutline myOutline;
    private bool outlineCheck;
    public bool dir;//True:왼쪽>오른쪽 False:오른쪽>왼쪽
    public int movestate; // 움직임 상태번호 0:이동 가능 1:이동 불가
    public bool movestateChange;
    public int creatnumber;
    public bool someThingBlock; // 앞에 무엇인가 막혔는지 정하는 변수

    public PhotonView pv;
    public SpriteRenderer sp;

    public LayerMask whatIsLayer;//길막 유닛 체크 마스크
    public LayerMask FrontUnitLayer;//길막 유닛 체크 마스크
    public LayerMask whatIsLayer2; //공격대상 체크 마스크
    public bool attackPlayer; // 공격중인지 확인 하는 변수

    public int myIndex;
    public string myName;
    public bool killTarget;

    public int bossAttackSignal;
    public int redPoint, bluePoint;
    public Text redtxt;
    public Text bluetxt;
    public float mhp, hp;
    public float speed;
    public int dieMoneyGet;
    public int damage;
    public int dropMoney;
    public float attackSpeed;//보너스공속+스킬공속의 합으로 나타냄
    public float bonusAttackSpeed;//유닛별 공속 증가를 위해
    public float bonusMoney; // 소환물 공격성공시 획득 골드
    public float bossBonusDamage; // 보스에게 추가 데미지
    public float bonusDamage; // 일반 보너스 데미지
    public float thornsSpeed; // 가시덤불 디버프 스피드
    public float oldHumanBuffSpeed; // 원시인 디버프 스피드
    public bool isHitted;
    public bool isGhost;
    [Header("초기 생성시 설정 해줘야 합니다.")]
    public Text stateText; // 몬스터 체력바 위의 텍스트 이다.
    public Canvas monsterUI;
    public Vector2 attOffset, size;//중첩 확인
    public Vector2 attOffset2, size2;//자신앞에 유닛 존재여부 확인
    public Vector2 attOffset3, size3;//공격범위에 적있는지 확인
    public Vector2 throwPos;//던지기 위치
    public Vector2 spawnPos;//태어날 위치
    [Header("끝")]
    public ParticleSystem hitParticle;
    Color orginColor;
    Color redColor = new Color(1, 0, 0);

    Coroutine lionBuff;
    public bool hasLionBuffFlag;
    public bool isPlayerMultiAttack;
    public bool hasGoldBananaFlag;


    Coroutine oldHumanBuff;
    public bool hasOldHumanBuffFlag;//원시인 버프 보유 여부

    Coroutine thornsBuff;
    public bool thornsFlag;//가시덤불 스크립트 보유 여부
    public bool hasThornsBuffFlag;//가시덤불에 의한 슬로 여부


    public bool nuckBackFlag;//죽창 스크립트 보유 여부


    public bool hasPoisionFlag;//중독상태
    public float poisionDamage;//원시인이 줄 수 있는 데미지
    public float poisionSpeed; // 원시인이 줄 수 있는 디버프 속도


    Coroutine TrapEnhance;
    public bool trapEnhanceFlag;

    public AttackEffectScript[] effectScript;
    public PlayerScript myPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        if (transform.tag != "Test")
            gameObject.layer = LayerMask.NameToLayer("Nothing");
        sp = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        thornsSpeed = 1;
        oldHumanBuffSpeed = 1;
        hitParticle = Resources.Load<ParticleSystem>("HitParticle");
        myOutline = GetComponent<SpriteOutline>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        orginColor = sp.color;
        SetMask(sp); // 마스크에 가려질 오브젝트인지 확인한다 player일 경우 무시한다
        pv = GetComponent<PhotonView>();
        setSpeed = speed;

        if (!pv.IsMine)
        {
                myPlayer = NetworkMaster.otherPlayer.GetComponent<PlayerScript>();
        }
        dir = myPlayer.dir;
        sp.flipX = dir;


        if (myPlayer.dir == NetworkMaster.Instance.dir)
        {
            MainGameManager.SetMonsterList(myName, gameObject, true); //게임메니져의 몬스터 리스트에 추가
        }

        //외곽선 표시기능
        if (transform.tag == "monster" || transform.tag == "Player" || transform.tag == "trap")
        {
            if (myPlayer.dir == NetworkMaster.Instance.dir)
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
        if (stateText != null)
        {
            monsterUI.sortingLayerName = sp.sortingLayerName;
            stateText.text = hp + " / " + mhp;
        }
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
        if (stateText != null)
        {
            monsterUI.sortingLayerName = sp.sortingLayerName;
            monsterUI.sortingOrder = 5;

            if (NetworkMaster.Instance.dir == true)
            {
                if (gameObject.transform.position.x < GradiantPos.Instance.transform.position.x)
                {
                    stateText.text = hp + " / " + mhp;
                }
                else
                {
                    stateText.text = "";
                }
            }
            else
            {
                if (gameObject.transform.position.x > GradiantPos.Instance.transform.position.x)
                {
                    stateText.text = hp + " / " + mhp;
                }
                else
                {
                    stateText.text = "";
                }
            }

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
                }
                else if (MainGameManager.mainGameManager.GetFocus() == gameObject)
                {
                    MainGameManager.mainGameManager.SetFocus(null);
                }
            }
        }
        #endregion 상대방 캐릭터 행위 기술
        #region 본인 캐릭터 행위 기술
        else if (pv.IsMine)
        {
            //&& myPlayer.dir == NetworkMaster.Instance.dir
            FrontUnitLayer = whatIsLayer;
            Setdir();
            if (hp > mhp)
            {
                hp = mhp;
            }
            if (hp <= 0)
            {
                hp = 0;
                if (gameObject.tag == "monster")
                {
                    MainGameManager.SetMonsterList(myName, gameObject, false); //게임메니져의 몬스터 리스트에서 제거
                    CreateHitPartiCle();
                }
                pv.RPC("MonsterDie", RpcTarget.All, killTarget);
                //Destroy(gameObject);
                return;
            }
            if (myPlayer.dir == NetworkMaster.Instance.dir)
            {
                SetFocusMonster();
            }
            if (gameObject.tag == "monster")
            {
                anim.SetFloat("AttackSpeed", attackSpeed);
                PlayMonster();
            }
            else if (gameObject.tag == "Player")
            {
                PlayPlayer();
            }
            else if (gameObject.tag == "Test")
            {
                //PlayBoss();
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
        whatIsLayer2 = LayerMask.GetMask("downunit") + LayerMask.GetMask("downlfyunit") + LayerMask.GetMask("upunit") + LayerMask.GetMask("upflyunit");//본인레이어 유닛과 플레이어와의 가로막힘을 위한 레이어추가
    }
    void PlayPlayer()
    {

    }
    void PlayTrap()
    {
        if (myPlayer.dir == NetworkMaster.Instance.dir)
        {
            if (MainGameManager.mainGameManager.GetNowMonster() == gameObject)
            {
                myOutline.enabled = true;
            }
            else
            {
                myOutline.enabled = false;
            }
        }
        if (!UnitThereExceptTrap())
        {
            //앞에 무언가가 없다면
            //animReset();
            movestate = 0;
        }
        else
        {
            //앞에 무언가가 있다면
            movestate = 1;
        }
        if (movestate == 1)
        {

        }
    }
    void PlayMonster()
    {
        //txt.text = "";//creatnumber.ToString();
        SetSkillOpiton(myName);
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
                    transform.position += Vector3.right * speed * (oldHumanBuffSpeed * thornsSpeed) * (dir == true ? 1 : -1) * Time.deltaTime * NetworkMaster.Instance.editorSpeed;
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
                if (hit2[i].tag == "monster" && hit2[i].GetComponent<monsterScript>().myPlayer != myPlayer)
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
                if (hit2[i].tag == "Player" && hit2[i].GetComponent<monsterScript>().myPlayer != myPlayer)
                {
                    anim.SetBool("attack", true);
                    break;
                }
                animReset();
            }
        }
        if (NetworkMaster.Instance.endPoint == 2)
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
            float attackTimer = Random.Range(float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "attackMinTimer", SceneVarScript.Instance.bossOption)), float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "attackMaxTimer", SceneVarScript.Instance.bossOption)));
            StartCoroutine(BossAttack(attackTimer));
        }
        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).IsName("attack")+ "//도착지점:"+ anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.97f)
        {
            animReset();
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SingleAttack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.97f)
        {
            animReset();
        }
        sp.flipX = !GetTargetPlayer();

    }
    public bool GetTargetPlayer()
    {
        if (redPoint > bluePoint)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    IEnumerator BossAttack(float attackTimer)
    {
        float attackType = Random.Range(0, 100);
        if (attackType < int.Parse(SceneVarScript.Instance.GetOptionByName(myName, "attackType", SceneVarScript.Instance.bossOption)))
        {
            anim.SetBool("attack", true);
        }
        else
        {
            anim.SetBool("SingleAttack", true);
        }
        yield return new WaitForSeconds(attackTimer);
        bossAttackSignal = 0;
    }
    IEnumerator colorCo()
    {
        sp.color = redColor;
        yield return new WaitForSeconds(0.04f);
        sp.color = orginColor;
        yield return new WaitForSeconds(0.04f);
        sp.color = redColor;
        yield return new WaitForSeconds(0.04f);
        sp.color = orginColor;
    }
    public void SetSkillOpiton(string myName)
    {
        float totalBonusAttackSpeed=1;
        //고유효과 부여하기 위해서는 이곳에 기입
        if (myName == "Monkey")
        {
            totalBonusAttackSpeed *= (1 + SkillManager.Instance.monkeyAttackSpeed);
            bonusMoney = SkillManager.Instance.bananaBonusGold;
            if (SkillManager.Instance.bananaBonusGold > 0)
            {
                FuncGoldBanana(true);
            }
            else
            {
                FuncGoldBanana(false);
            }
        }
        else if (myName == "Lion")
        {
            bossBonusDamage = SkillManager.Instance.lionBossBonusDamage; //고유 효과
            totalBonusAttackSpeed *= (1 + SkillManager.Instance.lionAttackSpeed) * (1 + bonusAttackSpeed);
        }

        if (NetworkMaster.Instance.GetMode() == "AI" && myPlayer.gameObject==NetworkMaster.otherPlayer)
        {
            attackSpeed=totalBonusAttackSpeed* (1 + AIManager.Instance.GetAttackSpeedBonus());
        }
        else
        {
            attackSpeed = totalBonusAttackSpeed;
        }
    }
    public void FuncGoldBanana(bool st)
    {
        hasGoldBananaFlag = st;
    }
    public void FuncLionAttackSpeedBuff(float time, float value)
    {
        if (lionBuff != null)
        {
            StopCoroutine(lionBuff);
        }
        lionBuff = StartCoroutine(LionAttackSpeedBuff(time, value));
    }
    IEnumerator LionAttackSpeedBuff(float time, float value)
    {
        hasLionBuffFlag = true;
        bonusAttackSpeed = value;
        yield return new WaitForSeconds(time);
        hasLionBuffFlag = false;
        bonusAttackSpeed = 0;
    }
    public void FuncOldHumanBuff(float time, float damage, float speed)
    {
        if (oldHumanBuff != null)
        {
            StopCoroutine(oldHumanBuff);
        }
        oldHumanBuff = StartCoroutine(OldHumanBuff(time, damage, speed));
    }
    IEnumerator OldHumanBuff(float time, float damage, float speed)
    {
        hasOldHumanBuffFlag = true;
        poisionDamage = damage;
        poisionSpeed = speed;
        yield return new WaitForSeconds(time);
        hasOldHumanBuffFlag = false;
        poisionDamage = 0;
        poisionSpeed = 0;
    }
    public void AddBonusDamage(float time,float value)
    {
        StartCoroutine(CoAddBonusDamage(time, value));
    }
    public void FuncTrapEnhanceBuff(float time, float damage)
    {
        StartCoroutine(TrapEnhanceBuff(time, damage));
    }
    IEnumerator TrapEnhanceBuff(float time,float damage)
    {
        trapEnhanceFlag = true;
        AddBonusDamage(time, damage);
        yield return new WaitForSeconds(time);
        trapEnhanceFlag = false;
    }
    IEnumerator CoAddBonusDamage(float time,float value)
    {
        float realValue = value / 100;
        bonusDamage += realValue;
        yield return new WaitForSeconds(time);
        bonusDamage -= realValue;
    }
    public void SetPlayerMultiAttack(bool set)
    {
        isPlayerMultiAttack = set;
    }
    public void Setdir()
    {
        if (gameObject.tag == "monster")
        {
            sp.flipX = dir;
        }
        else if (gameObject.tag == "Player")
        {
            sp.flipX = myPlayer.dir;
        }
    }
    public void animReset()
    {
        anim.SetBool("attack", false);
        if (gameObject.tag == "boss")
        {
            anim.SetBool("SingleAttack", false);
        }
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
        monsterScript monster;
        //자신앞에 유닛 존재여부 확인
        Collider2D[] hit2 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
        for (int i = 0; i < hit2.Length; i++)
        {
            monster = hit2[i].GetComponent<monsterScript>();
            //if ((dir == false && hit2[i].transform.position.x < transform.position.x) || (dir == true && hit2[i].transform.position.x > transform.position.x))
            if (monster.dir != dir || hit2[i].tag == "boss")
            {
                if (hit2[i].gameObject == gameObject)
                {
                    continue;
                }
                if (hit2[i].gameObject == myPlayer.gameObject)
                {
                    //Debug.Log("본인 player 콜라이더가 걸렸으므로 무시해야함");
                    continue;
                }
                if (monster.isGhost == true)
                {
                    continue;
                }
                if (hit2[i].tag == "boss" || hit2[i].tag == "Test" || (hit2[i].tag == "Player"&&isPlayerMultiAttack))
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
                if (monster.attackPlayer == false)
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
        monsterScript monster;
        Collider2D[] hit2 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset2.x * (dir == true ? -1 : 1)), transform.position.y + attOffset2.y), size2, 0, FrontUnitLayer);
        for (int i = 0; i < hit2.Length; i++)
        {
            monster = hit2[i].GetComponent<monsterScript>();
            if ((monster.creatnumber < creatnumber && monster.attackPlayer == false) || monster.dir != dir)
            {
                if (monster.gameObject == gameObject)
                {
                    continue;
                }
                if (monster.gameObject == myPlayer.gameObject)
                {
                    // Debug.Log("본인 player 콜라이더가 걸렸으므로 무시해야함");
                    continue;
                }
                if (monster.isGhost==true)
                {
                    continue;
                }
                if (monster.tag == "boss" || (monster.tag=="Player" && isPlayerMultiAttack))
                {
                    attackPlayer = true;
                }
                else
                {
                    attackPlayer = false;
                }
                //가로막는 유닛이 플레이어나 보스를 공격중이 아니라면 가로막혀서 전진불가
                if (monster.attackPlayer == false)
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
        Collider2D[] hit = Physics2D.OverlapBoxAll(
            new Vector2(transform.position.x + (attOffset.x * (dir == true ? -1 : 1)), transform.position.y + attOffset.y),
            size,
            0,
            whatIsLayer
            );

        if (hit.Length == 1 && hit[0].gameObject == gameObject)
        {
            //유닛간의 중첩이 없고 자기 자신만 있을 경우
            //멈춰있던 스피드 초기화 하여 전진할 수 있도록 한다.
            ResetSpeed();
        }
        else
        {
            movestateChange = true;
            //싱글톤으로 정의된 OverlapScript의 ReleaseMonster 함수를 호출한다
            OverlapScript.Instance.ReleaseMonster(hit, myPlayer);
        }
    }

    public void MonseterAttack()
    {
        if (pv.IsMine == true)
        {
            Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
            if (hitArea.Length > 0)
            {
                monsterScript target=null;
                for (int i = 0; i < hitArea.Length; i++)
                {
                    monsterScript tmpTarget = hitArea[i].gameObject.GetComponent<monsterScript>();
                    if (tmpTarget.isGhost == true)
                    {
                        continue;
                    }
                    if (tmpTarget.myPlayer!=this.myPlayer || tmpTarget.tag=="boss")
                    {
                        if (target != null)
                        {
                            if (Mathf.Abs(Vector2.Distance(tmpTarget.transform.position, transform.position)) < Mathf.Abs(Vector2.Distance(target.transform.position, transform.position)))
                            {
                                target = tmpTarget;
                            }
                        }
                        else
                        {
                            target = tmpTarget;
                        }
                    }
                }
                if (target != null)
                {
                    int calDamage = (int)(damage * (1 + bonusDamage) * (1 + AIManager.Instance.GetBonusDamage(myPlayer.gameObject)));
                    if (target.tag == "boss")
                    {
                        calDamage = (int)(calDamage*(1 + bossBonusDamage));
                    }
                    if (hasOldHumanBuffFlag)
                    {
                        target.pv.RPC("GetPoision", RpcTarget.All, poisionDamage, poisionSpeed);
                    }
                    if (nuckBackFlag)
                    {
                        target.pv.RPC("CrowdControl", RpcTarget.All, 0, float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "nuckBackX", SceneVarScript.Instance.trapOption)), float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "nuckBackY", SceneVarScript.Instance.trapOption)));
                    }
                    if (thornsFlag)
                    {
                        target.pv.RPC("GetThorns", RpcTarget.All, (100 - float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "slow", SceneVarScript.Instance.trapOption))) / 100, float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "slowTime", SceneVarScript.Instance.trapOption)));
                    }
                    target.RpcCallGetDamage(calDamage, dieMoneyGet, myPlayer.dir);
                }
            }
        }
    }
    public void MultiAttack()
    {
        if (pv.IsMine == true)
        {
            Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
            if (hitArea.Length > 0)
            {
                monsterScript target = null;
                for (int i = 0; i < hitArea.Length; i++)
                {

                    target = hitArea[i].gameObject.GetComponent<monsterScript>();
                    if (target.isGhost == true)
                    {
                        continue;
                    }
                    if (target.myPlayer == this.myPlayer && target.tag != "boss")
                    {
                        continue;
                    }

                    if (target != null)
                    {
                        int calDamage = (int)(damage * (1 + bonusDamage) * (1 + AIManager.Instance.GetBonusDamage(myPlayer.gameObject)));
                        if (target.tag == "boss")
                        {
                            calDamage = (int)(calDamage * (1 + bossBonusDamage));
                        }
                        if (hasOldHumanBuffFlag)
                        {
                            target.pv.RPC("GetPoision", RpcTarget.All, poisionDamage, poisionSpeed);
                        }
                        if (nuckBackFlag)
                        {
                            target.pv.RPC("CrowdControl", RpcTarget.All, 0, float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "nuckBackX", SceneVarScript.Instance.trapOption)), float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "nuckBackY", SceneVarScript.Instance.trapOption)));
                        }
                        if (thornsFlag)
                        {
                            target.pv.RPC("GetThorns", RpcTarget.All, (100 - float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "slow", SceneVarScript.Instance.trapOption))) / 100, float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "slowTime", SceneVarScript.Instance.trapOption)));
                        }
                        target.RpcCallGetDamage(calDamage, dieMoneyGet, myPlayer.dir);
                    }
                }
            }
        }
    }
    public void MonsterAttack()
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
                        if (hitArea.Length > 1 && hitArea[i].tag == "Player")
                        {
                            continue;
                            //플레이어 공격중에 캐릭터 생성된다면 공격타겟을 바꿔줘야하기 때문
                        }
                        //pv.RPC("BossHit", RpcTarget.All, 2);
                        int calDamage = (int)(damage * (1 + bonusDamage) * (1 + bossBonusDamage));
                        if (hasOldHumanBuffFlag)
                        {
                            target.pv.RPC("GetPoision", RpcTarget.All, poisionDamage, poisionSpeed);
                        }
                        if (nuckBackFlag)
                        {
                            target.pv.RPC("CrowdControl", RpcTarget.All, 0, float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "nuckBackX", SceneVarScript.Instance.trapOption)), float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "nuckBackY", SceneVarScript.Instance.trapOption)));
                        }
                        if (thornsFlag)
                        {
                            target.pv.RPC("GetThorns", RpcTarget.All, (100 - float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "slow", SceneVarScript.Instance.trapOption))) / 100, float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "slowTime", SceneVarScript.Instance.trapOption)));
                        }
                        target.RpcCallGetDamage(calDamage, dieMoneyGet, myPlayer.dir);
                        return;
                    }
                }
            }
        }
    }
    public void AttackEffectPlay(int index)
    {
        effectScript[index].EffectOn();
    }
    //애니메이션에서 실행됨
    public void MonseterThrow(string name)
    {
        if (pv.IsMine == true)
        {
            Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
            if (hitArea.Length > 0)
            {
                monsterScript target = null;
                for (int i = 0; i < hitArea.Length; i++)
                {
                    monsterScript tmpTarget = hitArea[i].gameObject.GetComponent<monsterScript>();
                    if (tmpTarget.isGhost == true)
                    {
                        continue;
                    }
                    if (tmpTarget.myPlayer != this.myPlayer || tmpTarget.tag == "boss")
                    {
                        if (target != null)
                        {
                            if (Mathf.Abs(Vector2.Distance(tmpTarget.transform.position, transform.position)) < Mathf.Abs(Vector2.Distance(target.transform.position, transform.position)))
                            {
                                target = tmpTarget;
                            }
                        }
                        else
                        {
                            target = tmpTarget;
                        }
                    }
                }
                if (target != null)
                {
                    NetworkMaster.Instance.CreatThrow(name, new Vector2(transform.position.x + (throwPos.x * (dir == true ? -1 : 1)), transform.position.y + throwPos.y), damage, this, target.gameObject, this,bonusMoney);
                }
            }
        }
    }
    public int GetLayerNum()
    {
        if (LayerMask.LayerToName(gameObject.layer)[0].Equals('d'))
        {
            return 0;
            //  Debug.Log(setLayerString+"유닛입니다, 다운유닛 취급");
        }
        else
        {
            return 1;
            // Debug.Log(setLayerString + "유닛입니다, 업 유닛");
        }
    }
    public void ResetSpeed()
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
    public void SetNuckbackFlag(bool st)
    {
        nuckBackFlag = st;
    }
    public void SetThornsFlag(bool st)
    {
        thornsFlag = st;
    }
    public void CreateHitPartiCle()
    {
        var par = Instantiate(hitParticle, transform.position, Quaternion.identity);
    }

    public void RpcCallTrapCoolReset()
    {
        pv.RPC("TrapCoolReset", RpcTarget.All);
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
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + (throwPos.x * (dir == true ? -1 : 1)), transform.position.y + throwPos.y), 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + (spawnPos.x * (dir == true ? -1 : 1)), transform.position.y + spawnPos.y), 0.1f);
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
            stream.SendNext(hasLionBuffFlag);
            stream.SendNext(hasOldHumanBuffFlag);
            stream.SendNext(hasPoisionFlag);
            stream.SendNext(hasThornsBuffFlag);
            stream.SendNext(hasGoldBananaFlag);
            stream.SendNext(sp.flipX);
            stream.SendNext(trapEnhanceFlag);
            stream.SendNext(attackSpeed);
        }
        else
        {
            dir = (bool)stream.ReceiveNext();
            mhp = (float)stream.ReceiveNext();
            hp = (float)stream.ReceiveNext();
            whatIsLayer = (int)stream.ReceiveNext();
            gameObject.layer = (int)stream.ReceiveNext();
            flystate = (int)stream.ReceiveNext();
            dropMoney = (int)stream.ReceiveNext();
            redPoint = (int)stream.ReceiveNext();
            bluePoint = (int)stream.ReceiveNext();
            myName = (string)stream.ReceiveNext();
            hasLionBuffFlag = (bool)stream.ReceiveNext();
            hasOldHumanBuffFlag = (bool)stream.ReceiveNext();
            hasPoisionFlag = (bool)stream.ReceiveNext();
            hasThornsBuffFlag = (bool)stream.ReceiveNext();
            hasGoldBananaFlag = (bool)stream.ReceiveNext();
            sp.flipX = (bool)stream.ReceiveNext();
            trapEnhanceFlag = (bool)stream.ReceiveNext();
            attackSpeed = (float)stream.ReceiveNext();
        }
    }
    [PunRPC]
    public void TrapCoolReset()
    {
        var trapScript = GetComponent<TrapScript>();
        trapScript.nowCool = 0;
    }

    [PunRPC]
    public void rpcTest()
    {
        Debug.Log("hi");
    }
    [PunRPC]
    public void GetThorns(float deBuffSpeed, float perTime)
    {
        if (pv.IsMine)
        {
            if (gameObject.tag != "monster")
            {
                return;
            }
            thornsSpeed = deBuffSpeed;
            if (thornsBuff != null)
            {
                StopCoroutine(thornsBuff);
            }
            thornsBuff = StartCoroutine(ThornsState(perTime));

        }
    }
    IEnumerator ThornsState(float perTime)
    {
        hasThornsBuffFlag = true;
        yield return new WaitForSeconds(perTime);
        hasThornsBuffFlag = false;
        thornsSpeed = 1;
        //float t = 0;
        //while (true)
        //{
        //    t += Time.deltaTime;
        //    if (t >= perTime)
        //    {
        //    thornsBuffFlag = false;
        //    yield break;
        //    }
        //    yield return null;
        //}
    }
    [PunRPC]
    public void GetPoision(float damage, float deBuffSpeed)
    {
        if (pv.IsMine)
        {
            if (gameObject.tag != "monster" && gameObject.tag != "Test")
            {
                return;
            }
            var dam = mhp * damage;
            oldHumanBuffSpeed = deBuffSpeed;
            if (hasPoisionFlag == false)
            {
                hasPoisionFlag = true;
                StartCoroutine(PoisionState(dam));
            }
        }
    }
    IEnumerator PoisionState(float damage)
    {
        float t = 0;
        var tick = Mathf.Ceil(damage / 5);
        while (true)
        {
            //Debug.Log("0.2초마다 데미지를 받습니다");
            RpcCallGetDamage((int)tick, 0, !myPlayer.dir, 1);
            while (true)
            {
                t += Time.deltaTime;
                if (t >= 0.2)
                {
                    t = 0;
                    break;
                }
                yield return null;
            }
        }
    }
    [PunRPC]
    public void PointCal(int damage, bool team)
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
    public void CrowdControl(int masterDir, float xForce, float yForce)
    {
        if (pv.IsMine)
        {
            if (gameObject.tag != "monster")
            {
                return;
            }
            rigid.AddForce(Vector2.up * yForce, ForceMode2D.Impulse);
            //0 : 맞는 대상의 방향의 넉백
            if (masterDir == 0)
            {
                rigid.AddForce((dir == true ? Vector2.left : Vector2.right) * xForce, ForceMode2D.Impulse);
            }
            else if (masterDir == 2)
            {
                rigid.AddForce(Vector2.left * xForce, ForceMode2D.Impulse);
                //2:공격자와 맞는자의 거리기반으로 넉백
            }
            else if (masterDir == 1)
            {
                rigid.AddForce(Vector2.right * xForce, ForceMode2D.Impulse);
                //1:오른쪽으로 넉백
            }
            else if (masterDir == -1)
            {
                rigid.AddForce(Vector2.left * xForce, ForceMode2D.Impulse);
                //-1:왼쪽으로 넉백
            }
        }
    }
    public void SetToDie()
    {
        hp = 0;
        killTarget = false;
    }
    public void RpcCallGetDamage(int damage, int dieMoneyGet, bool dir, int type = 0)
    {
        pv.RPC("GetDamage", RpcTarget.All, damage, dieMoneyGet, dir, type, 0f, 0f);
    }
    public void RpcCallGetDamage(int damage, int dieMoneyGet, bool dir, int type, float x, float y)
    {
        pv.RPC("GetDamage", RpcTarget.All, damage, dieMoneyGet, dir, type, x, y);
    }
    [PunRPC]
    public void GetDamage(int damage, int moneyGet, bool requestDir, int type, float x, float y)
    {
        //type :0:기본타입 1>>독공격
        //moneyGet=1 이면 중립공격(돈 획득 불가) 0 이면 상대방의 공격(돈 획득)
        //requestDir => true 왼쪽(red) false 오른쪽(blue)
        isHitted = true;
        if (gameObject.tag == "monster")
        {
            StartCoroutine(colorCo());
        }
        Vector2 attackPos = new Vector2(x, y);
        if (attackPos == Vector2.zero)
        {
            attackPos = transform.position;
        }
        if (pv.IsMine)
        {
            //내 몬스터가 공격 받을때 , 적이 때렸을때
            if (gameObject.tag == "boss" || gameObject.tag == "Test")
            {
                if (type == 0)
                {
                    if (requestDir == NetworkMaster.Instance.dir)
                    {
                        DamageObjectPool.Instacne.Pop(attackPos, damage, new Color(255, 255, 255));
                    }
                    else
                    {
                        DamageObjectPool.Instacne.Pop(attackPos, damage, new Color(1, 0, 0));
                    }
                }
                else if (type == 1)
                {
                    DamageObjectPool.Instacne.TickPop(attackPos, damage, new Color(0, 1, 0));
                }
            }
            else
            {
                if (type == 0)
                {
                    DamageObjectPool.Instacne.Pop(attackPos, damage, new Color(1, 0, 0));
                }
                else if (type == 1)
                {
                    DamageObjectPool.Instacne.TickPop(attackPos, damage, new Color(0, 1, 0));
                }
            }

            pv.RPC("PointCal", RpcTarget.All, damage, requestDir);
            if (gameObject == NetworkMaster.player)
            {
                MainGameManager.mainGameManager.AttackAlam();
            }
            if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["isTest"] || (NetworkMaster.otherPlayer && NetworkMaster.otherPlayer.GetComponent<monsterScript>().hp > 0))
            {
                //상대방이 살아있고
                if (hp - damage < 0)
                {
                    if (moneyGet == 1)
                    {
                        //죽기전 받은 공격의 소유자가 중립(보스 등)일 경우
                        //killTarget은 false가 되고 상대방은 돈을 받을 수 없다.
                        killTarget = false;
                    }
                    else
                    {
                        //죽기전 받은 공격의 소유자가 상대방일 경우
                        //killTarget은 true가 되고 상대방은 돈을 받을 수 있다.
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
        else
        {
            //내 몬스터가 아닌것이 공격 받을때 == 내가 때린것이 맞을때

            if (type == 0)
            {
                if (gameObject.tag == "boss" && requestDir != NetworkMaster.Instance.dir)
                {
                    DamageObjectPool.Instacne.Pop(attackPos, damage, new Color(1, 0, 0));
                }
                else
                {
                    DamageObjectPool.Instacne.Pop(attackPos, damage, new Color(255, 255, 255));
                }
            }
            else if (type == 1)
            {
                DamageObjectPool.Instacne.TickPop(attackPos, damage, new Color(0, 1, 0));
            }
        }
    }

    [PunRPC]
    public void MonsterDie(bool isKillByEnemy = false)
    {
        if (myPlayer.gameObject == NetworkMaster.player && gameObject.tag=="monster"&& NetworkMaster.Instance.GetMode()=="AI")
        {
            if (isKillByEnemy == true)
            {
                //Debug.Log("AI 매니저 돈 획득:" + dropMoney);
                AIManager.Instance.CalMoney((int)(dropMoney*MainGameManager.mainGameManager.dropGoldEff));
            }
        }
        if (myPlayer.dir != NetworkMaster.Instance.dir && gameObject.tag == "monster")
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
                        NetworkMaster.Instance.SendMainMsgFunc($"<color=red>RED</color>팀이 보스에게 더많은 피해를 입혔습니다.\n보스 처치 보상:<color=yellow>{GetMoney}Gold</color> 획득", 1);
                    }
                    else
                    {
                        NetworkMaster.Instance.SendMainMsgFunc($"<color=blue>BLUE</color>팀이 보스에게 더많은 피해를 입혔습니다.\n보스 처치 보상:<color=yellow>{GetMoney}Gold</color> 획득", 1);
                    }
                    //NetworkMaster.Instance.SendGameMsgFunc($"<color=blue>BLUE:</color> {GetMoney}<color=yellow>G</color>  <color=red>RED:</color> {dropMoney - GetMoney}<color=yellow>G</color>", 1);
                    Debug.Log($"{myName} 다음 보스를 소환합니다.");
                    NetworkMaster.Instance.SetNextStage(myName);
                }
            }
            MainGameManager.mainGameManager.CreatGoldEffect(transform.position, GetMoney); // 드랍머니를 공격 비율만큼 곱한값으로 설정해야한다
            if (NetworkMaster.Instance.GetMode() == "AI")
            {
                AIManager.Instance.CalMoney(dropMoney - GetMoney);
            }
        }
        if (pv.IsMine)
        {
            if (gameObject.tag != "Player")
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    #endregion
    #endregion
}
