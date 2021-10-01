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
        //건물 업그레이드 무한반복

        //시대가 2단계일때부터 덫 설치
            //10~25초 간격 자동 설치
            //금전 부족으로 설치 실패시 다음 틱으로
            //금전 가능하다면 성공할때 까지 반복

        //터치 공격은 0.5초에 한번식 무한으로 반복한다.
            //보스를 10초간 공격한다.
            //이후 2초간 적 트랩을 탐색한다.
            //적 트랩이 존재할 경우 5초간 공격한다.


        //터치 업그레이드
            //10레벨 전 10초 간격 자동
            //29레벨 전 23초 간격 자동
            //29레벨 이후 60초 간격 자동

        //건물이 현재 시대랑 맞을 경우
            //시대별 몬스터를 자동 소환한다 1.5~4초 간격
            //한틱에 소환을 하지 못했을 경우 현재 시대 바로 밑에시대 몬스터를 하나 소환한다.
            //바로 아래에서도 소환 못했을 경우 넘어간다.
        //다를 경우에는
            //시대별 몬스터를 자동 소환한다 5~8초 간격
            //한틱에 소환을 하지 못했을 경우 현재 시대 바로 밑에시대 몬스터를 하나 소환한다.
            //바로 아래에서도 소환 못했을 경우 넘어간다.

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
                Debug.Log("돈 획득:"+perMoney);
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

    //name : 몬스터 이름
    //type : 특정 위치 소환 여부
    //type_x : 특정 위치
    //layer : 0 - down 1 - up
    public void CreateMonster(string name,int type ,int type_x,int layer)
    {
        NetworkMaster.Instance.CreatMonster(name, type, type_x, layer, player);
    }
    public void CreateTrap(string name,float type_x, int layer)
    {
        //트랩 x 범위는 -22~22 
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
