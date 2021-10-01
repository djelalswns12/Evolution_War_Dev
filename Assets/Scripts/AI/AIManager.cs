using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;
    public string userName;

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
        userName = "Compute" + Random.Range(10000, 99999);
        player = NetworkMaster.otherPlayer;
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

        //�ô밡 2�ܰ��϶����� �� ��ġ
            //10~25�� ���� �ڵ� ��ġ
            //���� �������� ��ġ ���н� ���� ƽ����
            //���� �����ϴٸ� �����Ҷ� ���� �ݺ�

        //��ġ ������ 0.5�ʿ� �ѹ��� �������� �ݺ��Ѵ�.
            //������ 10�ʰ� �����Ѵ�.
            //���� 2�ʰ� �� Ʈ���� Ž���Ѵ�.
            //�� Ʈ���� ������ ��� 5�ʰ� �����Ѵ�.


        //��ġ ���׷��̵�
            //10���� �� 10�� ���� �ڵ�
            //29���� �� 23�� ���� �ڵ�
            //29���� ���� 60�� ���� �ڵ�

        //�ǹ��� ���� �ô�� ���� ���
            //�ô뺰 ���͸� �ڵ� ��ȯ�Ѵ� 1.5~4�� ����
            //��ƽ�� ��ȯ�� ���� ������ ��� ���� �ô� �ٷ� �ؿ��ô� ���͸� �ϳ� ��ȯ�Ѵ�.
            //�ٷ� �Ʒ������� ��ȯ ������ ��� �Ѿ��.
        //�ٸ� ��쿡��
            //�ô뺰 ���͸� �ڵ� ��ȯ�Ѵ� 5~8�� ����
            //��ƽ�� ��ȯ�� ���� ������ ��� ���� �ô� �ٷ� �ؿ��ô� ���͸� �ϳ� ��ȯ�Ѵ�.
            //�ٷ� �Ʒ������� ��ȯ ������ ��� �Ѿ��.

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
        MainGameManager.mainGameManager.GetNowBoss().GetComponent<BossScript>().TouchObj(MainGameManager.mainGameManager.GetNowBoss().transform.position,player);
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
                Debug.Log("�� ȹ��:"+perMoney);
            }
        }
    }
    public bool SpentGold(int num)
    {
        if (GetMoney()>= num)
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
    public void CreatePalaeozoic()
    {
        List<string> monsters = new List<string>();
        for (int i = 0; i < 3; i++) {
          monsters.Add(SceneVarScript.Instance.GetOptionByIndex(i.ToString(), "name", SceneVarScript.Instance.monsterOption));
        }
        string target = monsters[Random.Range(0, 3)];
        if (SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(target, "cost", SceneVarScript.Instance.monsterOption))))
        {
            CreateMonster(target, 0, 0, Random.Range(0, 2));
        }
    }
    public void CreateMesozoic()
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
        }
    }
    public void CreateCenozoic()
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
        }
    }

    //name : ���� �̸�
    //type : Ư�� ��ġ ��ȯ ����
    //type_x : Ư�� ��ġ
    //layer : 0 - down 1 - up
    public void CreateMonster(string name,int type ,int type_x,int layer)
    {
        NetworkMaster.Instance.CreatMonster(name, type, type_x, layer, player);
    }
    public void CreateTrap(string name,float type_x, int layer)
    {
        //Ʈ�� x ������ -22~22 
        // EX ) CreateTrap("BambooSpear", Random.Range(-22f,22f),Random.Range(0,1));
        NetworkMaster.Instance.CreateTrap(name, player,new Vector2(type_x,player.transform.position.y),layer);
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
        num =(int)(num*(1+goldEff));
        if (num >= 0)
        {
            allMoney += num;
        }
        money += num;
    }
}
