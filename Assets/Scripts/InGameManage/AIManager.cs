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
        player = NetworkMaster.otherPlayer;
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            
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
                Debug.Log("µ· È¹µæ:"+perMoney);
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
            Debug.Log("°ñµå°¡ ºÎÁ·ÇÕ´Ï´Ù");
            CalMoney(5000);
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
    public void CreateMonster(string name,int type ,int type_x,int layer)
    {
        NetworkMaster.Instance.CreatMonster(name, type, type_x, layer, player);
    }
    public void CreateTrap(string name,float type_x, int layer)
    {
        //Æ®·¦ x ¹üÀ§´Â -22~22 
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
        if (num >= 0)
        {
            allMoney += num;
        }
        money += num;
    }
}
