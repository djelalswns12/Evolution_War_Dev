using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RightNav : MonoBehaviour
{
    public AudioSource myAudio;
    public AudioClip[] audioClips;

    public string myname;
    public Image[] imageList;
    public GameObject uiImage;
    public GameObject[] popUpList;
    
    [Header("Type0:유닛소환 팝업 내용")]
    public Text uiName; 
    public Text uiDamage,uiHp,uiSpeed,uiSpawnTime,uiDesc;

    [Header("Type1:터치 업그레이드 팝업 내용")]
    public Text uiTouchName; 
    public Text uiTouchNowDamage;
    public Text uiTouchNowGold, uiTouchNowLevel, uiTouchNextDamage,uiTouchNextGold,uiTouchCost;

    [Header("Type2:건물진화 팝업 내용")]
    public Image uiBuildImg,uiBuildNextImg;
    public Text uiBuildName,uiBuildPerMoney;
    public Text uiBuildHp, uiBuildNextName, uiBuildNextHp, uiBuildNextPerMoney,uiBuildCost;

    [Header("Type4:트랩세부옵션 팝업 내용")]
    public Text uiTrapName, uiTrapCost;
    public Image uiTrapImg,uiTrapBar;
    public Text uiTrapAllHp, uiTrapDesc;
    [Header("Type5:스킬세부옵션 팝업 내용")]
    public Text uiSkillName, uiSkillNeed, uiSkillDesc;
    public Image uiSkillImg, uiSkillMask;
    public RectTransform skillDescRayout;
    public RectTransform skillNeedDescRayout;
    public GameObject uiSkillActiveBtn;
    [Header("기타")]
    public bool isRend;
    public bool isON;
    public GameObject CloseBtn;
    public Vector3 startPosX,endPosX;
    public float speed;
    public int openedType;
    // Start is called before the first frame update
    private void Awake()
    {
        startPosX = transform.localPosition;
        endPosX = startPosX;
        endPosX.x += 600;
        transform.localPosition = endPosX;
    }
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        CloseBtn.transform.SetAsLastSibling();//하이라이키 가장 마지막으로 순서 변경
    }

    // Update is called once per frame
    void Update()
    {
        if (isON)
        {
            if (openedType == 0)
            {
                uiImage.GetComponent<Image>().sprite = imageList[int.Parse(NetworkMaster.Instance.GetMonsterOption(myname, "icon"))].sprite;
                uiName.text = NetworkMaster.Instance.GetMonsterOption(myname, "nickname");
                uiDamage.text = "Damage : " + NetworkMaster.Instance.GetMonsterOption(myname, "damge");
                uiHp.text = "Hp : " + NetworkMaster.Instance.GetMonsterOption(myname, "mhp");
                uiSpeed.text = "Spedd : " + NetworkMaster.Instance.GetMonsterOption(myname, "speed");
                uiSpawnTime.text = "Spawn Time : " + SceneVarScript.Instance.GetOptionByName(myname,"cool", SceneVarScript.Instance.monsterOption)+" / s";
                uiDesc.text = NetworkMaster.Instance.GetMonsterOption(myname, "desc");
            }
            else if (openedType == 1)
            {
                //터치공격
                uiTouchName.text = "Touch Attack";

                uiTouchNowLevel.text = "Level : "+(MainGameManager.mainGameManager.touchLevel+1);
                uiTouchNowGold.text = "Gold : " + MainGameManager.mainGameManager.GetTouchDropGold();
                uiTouchNowDamage.text = "Damage : " + MainGameManager.mainGameManager.GetTouchDamge();

                uiTouchNextDamage.text = "Damage + " +(MainGameManager.mainGameManager.GetNextTouchDamge()- MainGameManager.mainGameManager.GetTouchDamge());
                uiTouchNextGold.text = "Gold + " + 0;


                uiTouchCost.text= MainGameManager.mainGameManager.StringDot(MainGameManager.mainGameManager.GetNextTouchCost()) + " G";
            }
            else if (openedType == 2)
            {
                var nowPlayerName = NetworkMaster.player.GetComponent<monsterScript>().myName;
                var nextPlayerName = "Player"+(int.Parse(NetworkMaster.Instance.GetMonsterOption(NetworkMaster.player.GetComponent<monsterScript>().myName, "icon"))-3000+2);
                //건물진화
                
                uiBuildImg.sprite = MainGameManager.mainGameManager.buildIconList[int.Parse(NetworkMaster.Instance.GetMonsterOption(nowPlayerName, "icon")) - 3000];
                uiBuildName.text = NetworkMaster.Instance.GetMonsterOption(nowPlayerName, "nickname");
                uiBuildHp.text ="Hp : "+NetworkMaster.Instance.GetMonsterOption(nowPlayerName, "mhp");
                uiBuildPerMoney.text = "Gold/s : " + SceneVarScript.Instance.GetOptionByName(nowPlayerName, "perMoney",SceneVarScript.Instance.playerOption);
                uiBuildNextName.text = NetworkMaster.Instance.GetMonsterOption(nextPlayerName, "nickname")!="null"? NetworkMaster.Instance.GetMonsterOption(nextPlayerName, "nickname"):"-";
                uiBuildNextHp.text = "Hp : " + (SceneVarScript.Instance.GetOptionByName(nextPlayerName, "mhp",SceneVarScript.Instance.monsterOption) != "null" ? SceneVarScript.Instance.GetOptionByName(nextPlayerName, "mhp", SceneVarScript.Instance.monsterOption) : "-");
                uiBuildNextPerMoney.text= "Gold/s : " + (SceneVarScript.Instance.GetOptionByName(nextPlayerName, "perMoney", SceneVarScript.Instance.playerOption) != "null" ? SceneVarScript.Instance.GetOptionByName(nextPlayerName, "perMoney", SceneVarScript.Instance.playerOption): "-");
                uiBuildCost.text =MainGameManager.mainGameManager.StringDot(NetworkMaster.Instance.GetMonsterOption(nowPlayerName, "cost")) + " G";
                uiBuildNextImg.sprite = NetworkMaster.Instance.GetMonsterOption(nextPlayerName, "icon")!="null"? MainGameManager.mainGameManager.buildIconList[int.Parse(NetworkMaster.Instance.GetMonsterOption(nextPlayerName, "icon")) - 3000]:uiBuildImg.sprite;
            }
            else if (openedType == 3)
            {
                //덫 소환 창

            }
            else if (openedType == 4)
            {
                if (MainGameManager.mainGameManager.GetNowMonster() != null)
                {
                    //내 트랩 선택창
                    var monster = MainGameManager.mainGameManager.GetNowMonster().GetComponent<monsterScript>();
                    uiTrapName.text = NetworkMaster.Instance.GetMonsterOption(monster.myName, "nickname");
                    uiTrapImg.sprite = MainGameManager.mainGameManager.trapIconList[int.Parse(NetworkMaster.Instance.GetMonsterOption(monster.myName, "icon")) - 1000];
                    uiTrapBar.fillAmount = monster.hp / monster.mhp;
                    uiTrapAllHp.text = monster.hp + "/" + monster.mhp;
                    uiTrapDesc.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(monster.myName, "desc", SceneVarScript.Instance.trapOption));
                    uiTrapCost.text = SceneVarScript.Instance.GetOptionByName(monster.myName, "upgradeCost", SceneVarScript.Instance.trapOption) =="null"?"-": SceneVarScript.Instance.GetOptionByName(monster.myName, "upgradeCost", SceneVarScript.Instance.trapOption);
                }
                else
                {
                    popUpList[4].SetActive(false);
                }
            }
            else if (openedType == 5)
            {
                if (MainGameManager.mainGameManager.GetNowSkill() != null)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(skillDescRayout);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(skillNeedDescRayout);
                    //스킬창
                    var skillName = MainGameManager.mainGameManager.GetNowSkill().GetComponent<SkillBtn>().myName;
                    uiSkillName.text = SceneVarScript.Instance.GetOptionByName(skillName, "nickname", SceneVarScript.Instance.skillOption);
                    uiSkillDesc.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(skillName, "desc", SceneVarScript.Instance.skillOption));
                    uiSkillNeed.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(skillName, "needDesc", SceneVarScript.Instance.skillOption));
                    uiSkillImg.sprite = SceneVarScript.Instance.skillIcon[int.Parse(SceneVarScript.Instance.GetOptionByName(skillName, "icon", SceneVarScript.Instance.skillOption))];
                    if (SceneVarScript.Instance.GetOptionByName(skillName, "state", SceneVarScript.Instance.skillOption) == "Active") {
                        uiSkillActiveBtn.SetActive(true);
                        uiSkillMask.enabled = !SkillManager.Instance.skillActiveList[int.Parse(SceneVarScript.Instance.GetOptionByName(skillName, "index", SceneVarScript.Instance.skillOption))];
                    }
                    else
                    {
                        uiSkillActiveBtn.SetActive(false);
                    }
                    
                }
                else
                {
                    popUpList[5].SetActive(false);
                }
            }
        }
    }
    public void SetMyName(string str)
    {
        myname = str;
    }
    public string GetMyName()
    {
        return myname;
    }
    public void CloseNav()
    {
        if (isON == true)
        {
            myname = "";
            isON = false;
            turnPageSoundPlay();
            StartCoroutine(Closing());
        }
    }
    public void OpenNav()
    {
        gameObject.SetActive(true);
        if (isON == false)
        {
            isON = true;
            turnPageSoundPlay();
            StartCoroutine(Opening());
        }
    }
    public void ResetPopUp(int n)
    {
        for (int i = 0; i < popUpList.Length; i++)
        {
            if (i != n)
            {
                popUpList[i].SetActive(false);
            }
            else
            {
                openedType = n;
                popUpList[i].SetActive(true);
            }
        }
    }
    public void turnPageSoundPlay()
    {
        myAudio.PlayOneShot(audioClips[0]);
    }
    IEnumerator Opening()
    {
        if (isRend == false)
        {
            SetUIPosByDir();
        }
        isRend = true;
        while (Mathf.Abs(startPosX.x - transform.localPosition.x) > 1)
        {
            transform.localPosition = Vector3.Lerp( transform.localPosition, startPosX, Time.deltaTime*speed);
            if (isON == false)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator Closing()
    {
        while (Mathf.Abs(endPosX.x - transform.localPosition.x) > 1)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition,endPosX, Time.deltaTime*speed);
            if (isON == true)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }
    public void SetUIPosByDir() {
        if (NetworkMaster.Instance.dir == true)
        {
            endPosX = new Vector3(startPosX.x + 600f, startPosX.y, startPosX.z);
        }
        else
        {
            var pos = CloseBtn.transform.localPosition;
            pos.x*=-1;
            CloseBtn.transform.localPosition = pos;
            startPosX.x *= -1;
            endPosX = new Vector3(startPosX.x - 600f, startPosX.y, startPosX.z);
        }
        transform.localPosition = endPosX;
    }
}
