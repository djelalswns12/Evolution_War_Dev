using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;
    public string userName;

    Dictionary<string, List<GameObject>> enemyMonsterList;

    [SerializeField]
    private int money;

    public int allMoney;
    public GameObject player;

    public float getMoneyTime;
    public float perMoney;

    public int playerBuliding;
    public int touchLevel;
    public int touchDropGold; // 보스 터치 공격시 발생하는 추가 돈

    [Header("몬스터 보너스 옵션")]
    public float attackSpeed;
    public float bonusDamage;
    public float goldEff;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (NetworkMaster.Instance.GetMode() == "AI")
        {
            enemyMonsterList = MainGameManager.GetMonsterList();
            userName = "Compute" + Random.Range(10000, 99999);
            player = NetworkMaster.otherPlayer;
            Auto();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMaster.Instance.GetMode() != "AI")
        {
            return;
        }

        player = NetworkMaster.otherPlayer;
        GetMoneyPerTime();
    }
    public void Auto()
    {
        //건물 업그레이드 무한반복
        StartCoroutine(AI_UpgradeBuilding());

        //시대가 2단계일때부터 덫 설치
        //10~25초 간격 자동 설치
        //금전 부족으로 설치 실패시 다음 틱으로
        //금전 가능하다면 성공할때 까지 반복
        StartCoroutine(AI_CreateTrap());

        //터치 공격은 0.5초에 한번식 무한으로 반복한다.
        //보스를 10초간 공격한다.
        //이후 2초간 적 트랩을 탐색한다.
        //적 트랩이 존재할 경우 5초간 공격한다.
        StartCoroutine(AI_TouchAttack());

        //터치 업그레이드
        //10레벨 전 10초 간격 자동
        //29레벨 전 23초 간격 자동
        //29레벨 이후 60초 간격 자동
        StartCoroutine(AI_TouchUpgrade());

        //--건물이 현재 시대랑 맞을 경우
        //시대별 몬스터를 자동 소환한다 1.5~4초 간격
        //한틱에 소환을 하지 못했을 경우 현재 시대 바로 밑에시대 몬스터를 하나 소환한다.
        //바로 아래에서도 소환 못했을 경우 넘어간다.
        //--다를 경우에는
        //시대별 몬스터를 자동 소환한다 5~8초 간격
        //한틱에 소환을 하지 못했을 경우 현재 시대 바로 밑에시대 몬스터를 하나 소환한다.
        //바로 아래에서도 소환 못했을 경우 넘어간다.
        StartCoroutine(AI_CreateMonster());
    }
    public void TargetAttack(GameObject obj)
    {
        obj.GetComponent<monsterScript>().RpcCallGetDamage(GetTouchDamge(), 0, !NetworkMaster.Instance.dir);
    }
    IEnumerator AI_TouchUpgrade()
    {
        while (true)
        {
            if (SpentGold(GetNextTouchCost())){
                touchLevel++;
            }
            if (touchLevel < 10)
            {
                yield return new WaitForSeconds(10);
            }else if (touchLevel < 29)
            {
                yield return new WaitForSeconds(23);
            }
            else
            {
                yield return new WaitForSeconds(60+(10*(touchLevel-30)));
            }
        }
    }
    IEnumerator AI_TouchAttack()
    {
        List<string> trapList = new List<string>();
        foreach (var item in SceneVarScript.Instance.trapOption)
        {
            if (item.Contains("name"))
            {
                trapList.Add(item["name"].ToString());
            }
        }
        float waitTime = 1f;
        int bossAttackCount = 0;
        int trapAttackCount = 0;
        int trapFindCount = 0;

        GameObject target=null;
        Vector3 pos= NetworkMaster.player.transform.position;
        while (true)
        {
            if (bossAttackCount < 10)
            {
                bossAttackCount++;
                TouchAttack();
            }
            else if(trapFindCount<3 && target==null)
            {
                trapFindCount++;
                for (int i = 0; i < trapList.Count; i++)
                {
                    if (enemyMonsterList.ContainsKey(trapList[i]))
                    {
                        if (enemyMonsterList[trapList[i]].Count > 0)
                        {
                            if (NetworkMaster.Instance.dir)
                            {
                                if (enemyMonsterList[trapList[i]][0].transform.position.x > pos.x - 3.5f)
                                {
                                    target = enemyMonsterList[trapList[i]][0];
                                    break;
                                }
                            }
                            else
                            {
                                if (enemyMonsterList[trapList[i]][0].transform.position.x < pos.x + 3.5f)
                                {
                                    target = enemyMonsterList[trapList[i]][0];
                                    break;
                                }
                            }
                        }
                    }
                }
                //트랩탐색
            }
            else if(target!=null && trapAttackCount<5)
            {
                trapAttackCount++;
                TargetAttack(target);
            }
            else
            {
                bossAttackCount = 0;
                trapAttackCount = 0;
                trapFindCount = 0;
                continue;
            }
            waitTime = Random.Range(0.5f, 1);
            yield return new WaitForSeconds(waitTime);
        }
    }
    IEnumerator AI_CreateMonster()
    {
        //Dictionary<int,List<string>> monsterList = new Dictionary<int,List<string>>();
        //foreach (var item in SceneVarScript.Instance.trapOption)
        //{
        //    if (item.Contains("stage"))
        //    {
        //        if (monsterList.ContainsKey(int.Parse(item["stage"].ToString())))
        //        {
        //            monsterList[int.Parse(item["stage"].ToString())].Add(item["stage"].ToString());
        //        }
        //        else {
        //            monsterList.Add(int.Parse(item["stage"].ToString()), new List<string> { item["name"].ToString() });
        //        }
        //    }
        //}
        float waitTime;
        while (true)
        {
            if (GetPlayerBuliding() - 1 >= NetworkMaster.Instance.gameStage)
            {
                //건물 업그레이드 충분
                waitTime = Random.Range(3f, 5f);
                //targetName = monsterList[Mathf.Clamp(NetworkMaster.Instance.gameStage, 0, monsterList.Count-1)][Random.Range(0,3)];

                if (playerBuliding == 0)
                {
                    if (CreatePalaeozoic())
                    {
                        waitTime = Random.Range(3f, 5f);
                    }
                    else
                    {
                        waitTime = Random.Range(3f, 5f);
                    }
                }
                else if (playerBuliding == 1)
                {
                    if (CreateMesozoic())
                    {
                        waitTime = Random.Range(3f, 5f);
                    }
                    else if(CreatePalaeozoic()) {
                        waitTime = Random.Range(3f, 5f);
                    }
                    else
                    {
                        waitTime = Random.Range(3f, 5f);
                    }
                }
                else if (playerBuliding > 1)
                {
                    if (CreateCenozoic())
                    {
                        waitTime = Random.Range(3f, 5f);
                    }
                    else if (CreateMesozoic())
                    {
                        waitTime = Random.Range(3f, 5f);
                    }
                    else if(CreatePalaeozoic()){
                        waitTime = Random.Range(3f, 5f);
                    }
                }
            }
            else
            {
                //건물 업그레이드 부족
                waitTime = Random.Range(5f, 8f);

                if (playerBuliding == 0)
                {
                    if (CreatePalaeozoic())
                    {
                        waitTime = Random.Range(5f, 8f);
                    }
                    else
                    {
                        waitTime = Random.Range(5f, 8f);
                    }
                }
                else if (playerBuliding == 1)
                {
                    if (CreateMesozoic())
                    {
                        waitTime = Random.Range(5f, 8f);
                    }
                    else if (CreatePalaeozoic())
                    {
                        waitTime = Random.Range(5f, 8f);
                    }
                    else
                    {
                        waitTime = Random.Range(5f, 8f);
                    }
                }
                else if (playerBuliding > 1)
                {
                    if (CreateCenozoic())
                    {
                        waitTime = Random.Range(5f, 8f);
                    }
                    else if (CreateMesozoic())
                    {
                        waitTime = Random.Range(5f, 8f);
                    }
                    else if (CreatePalaeozoic())
                    {
                        waitTime = Random.Range(5f, 8f);
                    }
                }

            }
            yield return new WaitForSeconds(waitTime);
        }
    }
    IEnumerator AI_CreateTrap()
    {
        List<string> trapList = new List<string>();
        foreach (var item in SceneVarScript.Instance.trapOption)
        {
            if (item.Contains("name"))
            {
                trapList.Add(item["name"].ToString());
            }
        }
        while (NetworkMaster.Instance.endPoint == 0)
        {
            string trapName = trapList[Random.Range(0, trapList.Count)];
            int aggression = int.Parse(SceneVarScript.Instance.GetOptionByName(trapName, "aggression", SceneVarScript.Instance.trapOption));
            float range;
            switch (aggression)
            {
                case 0:
                    if (NetworkMaster.Instance.dir)
                    {
                        range = Random.Range(-22f, 0f);
                    }
                    else
                    {
                        range = Random.Range(0, 22f);
                    }
                    break;
                case 1:
                    if (NetworkMaster.Instance.dir)
                    {
                        range = Random.Range(-22f, 22f);
                    }
                    else
                    {
                        range = Random.Range(-22f, 22f);
                    }
                    break;
                case 2:
                    if (NetworkMaster.Instance.dir)
                    {
                        range = Random.Range(6f, 22f);
                    }
                    else
                    {
                        range = Random.Range(-22, -6f);
                    }
                    break;
                default:
                    range = Random.Range(-22f, 22f);
                    break;
            }
            CreateTrap(trapName, range, Random.Range(0, 2));
            yield return new WaitForSeconds(Random.Range(10, 25));
        }
    }
    IEnumerator AI_UpgradeBuilding()
    {
        while (NetworkMaster.Instance.endPoint == 0)
        {
            UpgradeBuild();
            yield return new WaitForSeconds(1);
        }
    }

    public float GetBonusDamage(GameObject player)
    {
        if (this.player == player)
        {
            return bonusDamage;
        }
        else
        {
            return 0;
        }
    }
    public float GetAttackSpeedBonus()
    {
        return attackSpeed;
    }
    public int GetTouchDropGold()
    {
        return touchDropGold;
    }
    public int GetNextTouchCost()
    {
        return (int)(touchLevel * touchLevel) + 10;
    }
    public int GetTouchDamge()
    {
        return MainGameManager.mainGameManager.TouchDamageTheory(touchLevel);
    }
    public void TouchAttack()
    {
        if (MainGameManager.mainGameManager.GetNowBoss() != null)
        {
            MainGameManager.mainGameManager.GetNowBoss().GetComponent<BossScript>().TouchObj(MainGameManager.mainGameManager.GetNowBoss().transform.position, player);
        }
    }
    public void GetMoneyPerTime()
    {
        var nowPlayerName = player.GetComponent<monsterScript>().myName;
        perMoney = int.Parse(SceneVarScript.Instance.GetOptionByName(nowPlayerName, "perMoney", SceneVarScript.Instance.playerOption));
        if (NetworkMaster.Instance.endState == 0)
        {
            getMoneyTime += Time.deltaTime;
            if (getMoneyTime >= MainGameManager.mainGameManager.getPerTime)
            {
                getMoneyTime -= MainGameManager.mainGameManager.getPerTime;
                CalMoney((int)perMoney);
                Debug.Log("돈 획득:" + perMoney);
            }
        }
    }
    public bool SpentGold(int num)
    {
        if (GetMoney() >= num)
        {
            CalMoney(-num);
            return true;
        }
        else
        {
            Debug.Log("골드가 부족합니다");
            return false;
        }
    }
    public void UpgradeBuild()
    {
        NetworkMaster.Instance.UpgradeBuildByPlayer(player);
    }
    public void SetPlayerBuliding(int nowBuild)
    {
        playerBuliding = nowBuild;
    }
    public int GetPlayerBuliding()
    {
        return playerBuliding;
    }
    public bool CreatePalaeozoic()
    {
        List<string> monsters = new List<string>();
        for (int i = 0; i < 3; i++)
        {
            monsters.Add(SceneVarScript.Instance.GetOptionByIndex(i.ToString(), "name", SceneVarScript.Instance.monsterOption));
        }
        string target = monsters[Random.Range(0, 3)];
        if (SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(target, "cost", SceneVarScript.Instance.monsterOption))))
        {
            CreateMonster(target, 0, 0, Random.Range(0, 2));
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CreateMesozoic()
    {
        List<string> monsters = new List<string>();
        for (int i = 3; i < 6; i++)
        {
            monsters.Add(SceneVarScript.Instance.GetOptionByIndex(i.ToString(), "name", SceneVarScript.Instance.monsterOption));
        }
        string target = monsters[Random.Range(0, 3)];
        if (SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(target, "cost", SceneVarScript.Instance.monsterOption))))
        {
            CreateMonster(target, 0, 0, Random.Range(0, 2));
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool CreateCenozoic()
    {
        List<string> monsters = new List<string>();
        for (int i = 6; i < 9; i++)
        {
            monsters.Add(SceneVarScript.Instance.GetOptionByIndex(i.ToString(), "name", SceneVarScript.Instance.monsterOption));
        }
        string target = monsters[Random.Range(0, 3)];
        if (SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(target, "cost", SceneVarScript.Instance.monsterOption))))
        {
            CreateMonster(target, 0, 0, Random.Range(0, 2));
            return true;
        }
        else
        {
            return false;
        }
    }

    //name : 몬스터 이름
    //type : 특정 위치 소환 여부
    //type_x : 특정 위치
    //layer : 0 - down 1 - up
    public void CreateMonster(string name, int type, int type_x, int layer)
    {
        NetworkMaster.Instance.CreatMonster(name, type, type_x, layer, player);
    }
    public void CreateTrap(string name, float type_x, int layer)
    {
        //트랩 x 범위는 -22~22 
        // EX ) CreateTrap("BambooSpear", Random.Range(-22f,22f),Random.Range(0,1));
        NetworkMaster.Instance.CreateTrap(name, player, new Vector2(type_x, player.transform.position.y), layer);
    }
    public void SetMoney(int num)
    {
        money = num;
    }
    public int GetMoney()
    {
        return money;
    }
    public void CalMoney(int num)
    {
        num = (int)(num * (1 + goldEff));
        if (num >= 0)
        {
            allMoney += num;
        }
        money += num;
    }
}
