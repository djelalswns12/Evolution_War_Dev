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
    public int touchDropGold; // ���� ��ġ ���ݽ� �߻��ϴ� �߰� ��

    [Header("���� ���ʽ� �ɼ�")]
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
        //�ǹ� ���׷��̵� ���ѹݺ�
        StartCoroutine(AI_UpgradeBuilding());

        //�ô밡 2�ܰ��϶����� �� ��ġ
        //10~25�� ���� �ڵ� ��ġ
        //���� �������� ��ġ ���н� ���� ƽ����
        //���� �����ϴٸ� �����Ҷ� ���� �ݺ�
        StartCoroutine(AI_CreateTrap());

        //��ġ ������ 0.5�ʿ� �ѹ��� �������� �ݺ��Ѵ�.
        //������ 10�ʰ� �����Ѵ�.
        //���� 2�ʰ� �� Ʈ���� Ž���Ѵ�.
        //�� Ʈ���� ������ ��� 5�ʰ� �����Ѵ�.
        StartCoroutine(AI_TouchAttack());

        //��ġ ���׷��̵�
        //10���� �� 10�� ���� �ڵ�
        //29���� �� 23�� ���� �ڵ�
        //29���� ���� 60�� ���� �ڵ�
        StartCoroutine(AI_TouchUpgrade());

        //--�ǹ��� ���� �ô�� ���� ���
        //�ô뺰 ���͸� �ڵ� ��ȯ�Ѵ� 1.5~4�� ����
        //��ƽ�� ��ȯ�� ���� ������ ��� ���� �ô� �ٷ� �ؿ��ô� ���͸� �ϳ� ��ȯ�Ѵ�.
        //�ٷ� �Ʒ������� ��ȯ ������ ��� �Ѿ��.
        //--�ٸ� ��쿡��
        //�ô뺰 ���͸� �ڵ� ��ȯ�Ѵ� 5~8�� ����
        //��ƽ�� ��ȯ�� ���� ������ ��� ���� �ô� �ٷ� �ؿ��ô� ���͸� �ϳ� ��ȯ�Ѵ�.
        //�ٷ� �Ʒ������� ��ȯ ������ ��� �Ѿ��.
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
                //Ʈ��Ž��
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
                //�ǹ� ���׷��̵� ���
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
                //�ǹ� ���׷��̵� ����
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
                Debug.Log("�� ȹ��:" + perMoney);
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
            Debug.Log("��尡 �����մϴ�");
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

    //name : ���� �̸�
    //type : Ư�� ��ġ ��ȯ ����
    //type_x : Ư�� ��ġ
    //layer : 0 - down 1 - up
    public void CreateMonster(string name, int type, int type_x, int layer)
    {
        NetworkMaster.Instance.CreatMonster(name, type, type_x, layer, player);
    }
    public void CreateTrap(string name, float type_x, int layer)
    {
        //Ʈ�� x ������ -22~22 
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
