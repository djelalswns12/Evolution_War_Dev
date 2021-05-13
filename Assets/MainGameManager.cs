using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MainGameManager : MonoBehaviour
{
    public static MainGameManager mainGameManager;
    [SerializeField]
    public int money;
    private float getMoneyTime;
    public GameObject hgold; // 머니 이펙트 리소스
    public int perMoney; //getPerTime초당 들어오는 돈의 양
    public float getPerTime;//초의 기준
    public Text goldText; 
    public float dropGoldEff; // 처치 골드 효율
    public GameObject focus; // 시야 포커스
    public GameObject nowBoss; // 현재 소환된 보스
    public GameObject bossUI;
    public Image redBar, blueBar, bossHpBar;
    public TextMeshProUGUI bossHp,redPointPer,bluePointPer;
    public Text redGetMoney, blueGetMoney;
    public int playerBuliding;
    [SerializeField]
    private int touchDamge;

    private monsterScript nowBossScript;
    // Start is called before the first frame update
    void Start()
    {
        focus = null;
        mainGameManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        getMoneyTime += Time.deltaTime;
        if (getMoneyTime >= getPerTime)
        {
           
            getMoneyTime-=getPerTime;
            CountMoney(perMoney);
        }
        goldText.text = StringDot(GetMoney().ToString())+" Gold";

        if (nowBoss != null)
        {
            bossUI.SetActive(true);
            nowBossScript = nowBoss.GetComponent<monsterScript>();
            if (nowBossScript.redPoint + nowBossScript.bluePoint > 0)
            {
                redBar.fillAmount = (float)nowBossScript.redPoint / (nowBossScript.redPoint + nowBossScript.bluePoint);
                redPointPer.text = (redBar.fillAmount * 100).ToString("N2")+"%";
                blueBar.fillAmount = (float)nowBossScript.bluePoint / (nowBossScript.redPoint + nowBossScript.bluePoint);
                bluePointPer.text = (blueBar.fillAmount * 100).ToString("N2") + "%";

                redGetMoney.text = StringDot(Mathf.Floor(nowBossScript.dropMoney * redBar.fillAmount))+" Gold";
                blueGetMoney.text = StringDot(Mathf.Floor(nowBossScript.dropMoney* blueBar.fillAmount))+ " Gold";
            }
            else
            {
                redPointPer.text = "0%";
                bluePointPer.text = "0%";
                redGetMoney.text = "0 GOLD";
                blueGetMoney.text = "0 GOLD";
                redBar.fillAmount = 0;
                blueBar.fillAmount = 0;
            }
                bossHpBar.fillAmount = nowBossScript.hp / nowBossScript.mhp;
            bossHp.text = Mathf.Floor(nowBossScript.hp / nowBossScript.mhp * 100)+"%";
        }
        else
        {
            bossUI.SetActive(false);
        }
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    CreatGoldEffect(Camera.main.ScreenToWorldPoint(Input.mousePosition), Random.Range(10, 5000));
        //}
    }
    public void CreatGoldEffect(Vector2 pos,int gotGold)
    {
        GameObject GoldEffect=GameObject.Instantiate(hgold, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        GoldEffect.GetComponent<MoneyPos>().gotGold = gotGold;
        GoldEffect.GetComponent<MoneyPos>().txt.text = StringDot(gotGold)+"+";
    }
    public int GetMoney()
    {
        return money;
    }
    void SetMoney(int value)
    {
        money=value;
    }
    public void CountMoney(int value)
    {
        SetMoney(GetMoney() + value);
    }
    string StringDot<T>(T str)
    {
        string answer="";
        string copy=str.ToString();
        for(int i = copy.Length-1; i >= 0; i--)
        {
            answer = copy[i] + answer;
            if ( ((copy.Length - 1)-i)  % 3==2 && i>0)
            {
                answer = "," + answer;
            }
        }

        return answer;
    }
    public int GetTouchDamge()
    {
        return touchDamge;
    }
    public GameObject GetFocus()
    {
        return focus;
    }
    public void SetFocus(GameObject newOne)
    {
        this.focus = newOne;
    }
    public void SetNowBoss(GameObject obj)
    {
        nowBoss = obj;
    }
    public void SetPlayerBuliding(int n)
    {
        playerBuliding = n;
    }
    public int GetPlayerBuliding()
    {
        return playerBuliding;
    }
}
