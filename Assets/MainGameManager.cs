using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MainGameManager : MonoBehaviour
{
    public static MainGameManager mainGameManager;
    [SerializeField]
    public float money;
    private float getMoneyTime;
    public GameObject hgold; // 머니 이펙트 리소스
    public int perMoney; //getPerTime초당 들어오는 돈의 양
    public float getPerTime;//초의 기준
    public Text goldText; 
    public float dropGoldEff; // 처치 골드 효율
    public GameObject focus; // 시야 포커스
    public GameObject nowBoss; // 현재 소환된 보스
    public GameObject nowMonster; // 클릭된 몬스터
    public GameObject nowSkill; // 클릭된 스킬
    public GameObject bossUI;
    public GameObject RightNav;
    public GameObject moveUI;
    RightNav RightNavScript;
    public bool canTouchAttack;
    public Image touchAttackBlind;
    public Image redBar, blueBar, bossHpBar;
    public TextMeshProUGUI bossHp,redPointPer,bluePointPer;
    public Text redGetMoney, blueGetMoney;
    public int playerBuliding,touchLevel;
    public int touchDropGold;
    public float touchAttackCool,touchAttackNowCool;
    public SetPopUP popUpMaster;
    public Sprite[] bossIconList;//보스 아이콘
    public Sprite[] buildIconList;//건물 아이콘
    public Sprite[] trapIconList;//트랩 아이콘
    public Sprite[] buffIconList;//버프 아이콘
    
    public Image bossIcon;
    public AudioSource[] myAudio;
    public AudioClip timeChange,buySound;
    public Dictionary<string,List<GameObject>> monsterList;
    [SerializeField]
    private int touchDamge;
    private monsterScript nowBossScript;

    SkillManager skillManager;
    public List<string> useSkill;
    // Start is called before the first frame update
    void Start()
    {
        useSkill = new List<string>();
        skillManager = GetComponent<SkillManager>();
        monsterList = new Dictionary<string, List<GameObject>>();
        popUpMaster = GetComponent<SetPopUP>();
        focus = null;
        mainGameManager = this;
        RightNavScript = RightNav.GetComponent<RightNav>();
    }

    // Update is called once per frame
    void Update()
    {
        //SkillSlot("1");
        //SkillSlot("2");
        //SkillSlot("3");
        //골드 획득량 DB연동
        var nowPlayerName = NetworkMaster.player.GetComponent<monsterScript>().myName;
        SetPerMoney(int.Parse(SceneVarScript.Instance.GetOptionByName(nowPlayerName,"perMoney",SceneVarScript.Instance.playerOption)));
     
        //터치 데미지 공식으로 적용 되도록함
        SetTouchDamge(TouchDamageTheory());

        //시간별 골드 획득
        getMoneyTime += Time.deltaTime;
        if (getMoneyTime >= getPerTime)
        {
            getMoneyTime-=getPerTime;
            Vector3 pos = NetworkMaster.player.transform.position;
            pos.y += 2;
            CreatGoldEffect(pos,(int)(perMoney));
        }
        goldText.text = StringDot(GetMoney().ToString())+" Gold";

        //선택 자동해제
        UISelecterClear();

        //보스 UI 관련
        if (nowBoss != null)
        {
            //Debug.Log(NetworkMaster.Instance.GetMonsterOption(nowBoss.GetComponent<monsterScript>().myName, "icon"));
            bossIcon.sprite = bossIconList[int.Parse(NetworkMaster.Instance.GetMonsterOption(nowBoss.GetComponent<monsterScript>().myName,"icon"))-1000];
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
        if (GetCanTouchAttack() == false)
        {
            touchAttackBlind.fillAmount = touchAttackNowCool / touchAttackCool;
            touchAttackNowCool -= Time.deltaTime;
            if (touchAttackNowCool <= 0)
            {
                touchAttackNowCool = 0;
                canTouchAttack = true;
            }
        }
        else
        {
            touchAttackBlind.fillAmount = 0f;
        }
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    CreatGoldEffect(Camera.main.ScreenToWorldPoint(Input.mousePosition), Random.Range(10, 5000));
        //}
    }
    public void UISelecterClear()
    {
        if (RightNavScript.openedType != 4 || RightNavScript.isON == false)
        {
            SetNowMonster(null);
        }
        if (RightNavScript.openedType != 5 || RightNavScript.isON == false)
        {
            SetNowSkill(null);
        }
    }
    public void SkillSlot(string slotNum)
    {
        var skillNum = SceneVarScript.Instance.GetUserOption("skill" + slotNum);
        if (skillNum != "-1")
        {
            //Debug.Log(skillNum + "번 스킬 사용 요청!");
           // skillManager.UseSkill(skillNum,slotNum);
        }
    }
    public static Dictionary<string,List<GameObject>> GetMonsterList()
    {
        return mainGameManager.monsterList;
    }
    public static void SetMonsterList(string myName,GameObject monster,bool isSet=true/*isSet:true-> 삽입요청 , false->삭제요청 */)
    {
        if (mainGameManager.monsterList.ContainsKey(myName))
        {
            if (isSet)
            {
                mainGameManager.monsterList[myName].Add(monster);
            }
            else
            {
                //몬스터리스트에서 해당 몬스터를 찾아서 제거한다.
                for(int i = mainGameManager.monsterList[myName].Count-1;i>=0 ; i--)
                {
                    if (mainGameManager.monsterList[myName][i] == monster)
                    {
                        mainGameManager.monsterList[myName].RemoveAt(i);
                    }
                }
            }
        }
        else
        {
            var data = new List<GameObject>();
            data.Add(monster);
            mainGameManager.monsterList.Add(myName,data);
        }
 
    }
    public void CreatDamageEffect(Vector2 pos,int damage)
    {

    }
    public void CreatGoldEffect(Vector2 pos,int gotGold)
    {
        GoldObjectPool.Instance.Pop(pos, gotGold);
    }
    public void SetPerMoney(int cost)
    {
        perMoney = cost;
    }
    public int GetMoney()
    {
        return (int)money;
    }
    public void CoolTouchAttack()
    {
        touchAttackNowCool = touchAttackCool;
        canTouchAttack = false;
    }
    void SetMoney(float value)
    {
        money=value;
    }
    public void CountMoney(float value)
    {

        SetMoney(money + value);
    }
    public string StringDot<T>(T str)
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
    public bool GetCanTouchAttack()
    {
        return canTouchAttack;
    }
    public int GetNextTouchDamge()
    {
        return TouchDamageTheory(touchLevel+1);
    }
    public void SetTouchDamge(int n)
    {
        touchDamge = n;
    }
    public int TouchDamageTheory(int n=-1/*is null*/)
    {
        
        if (n == -1)
        {
            return (int)Mathf.Ceil(touchLevel * touchLevel * 0.2f)+touchLevel + 10;
            //return 20 + (touchLevel * 30);
        }
        else
        {
            return (int)Mathf.Ceil(n * n * 0.2f) + touchLevel + 10;
        }
    }
    public bool SpentGold(int num)
    {
        PlaySoundBuySound();
        if (GetMoney() >= num)
        {
            CountMoney(-num);
            return true;
        }
        else
        {
            NetworkMaster.Instance.SendGameMsgFunc("골드가 부족합니다", 0);
            return false;
        }
    }

    public void touchDamgeUp()
    {
        
        if (SpentGold(GetNextTouchCost()) /*GetMoney() >=GetNextTouchCost()*/)
        {
            PlaySoundBuySound();
            touchLevel++;
        }
    }
    public int GetNextTouchCost()
    {
        return (int)(touchLevel * touchLevel) + 10;
    }
    public int GetTouchDropGold()
    {
        return touchDropGold;
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
    public void PlaySoundTimeChange()
    {
        myAudio[0].PlayOneShot(timeChange);
    }
    public void PlaySoundBuySound()
    {
        myAudio[1].PlayOneShot(buySound);
    }
    public void SetNowMonster(GameObject obj)
    {
        nowMonster = obj;
    }
    public GameObject GetNowMonster()
    {
        return nowMonster;
    }
    public void SetNowSkill(GameObject obj)
    {
        nowSkill = obj;
    }
    public GameObject GetNowSkill()
    {
        return nowSkill;
    }
}
