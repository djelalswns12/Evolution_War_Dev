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
    public Text uiDamage,uiHp,uiSpeed,uiDesc;

    [Header("Type1:터치 업그레이드 팝업 내용")]
    public Text uiTouchName; 
    public Text uiTouchNowDamage;
    public Text uiTouchNowGold, uiTouchNowLevel, uiTouchNextDamage,uiTouchNextGold,uiTouchCost;

    [Header("Type2:건물진화 팝업 내용")]
    public Image uiBuildImg;
    public Text uiBuildName;
    public Text uiBuildHp, uiBuildNextName,uiBuildCost;

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
        CloseBtn.transform.SetAsLastSibling();
    }

    // Update is called once per frame
    void Update()
    {
        if (isON)
        {
            if (openedType == 0)
            {
                uiImage.GetComponent<Image>().sprite = imageList[int.Parse(NetworkMaster.Instance.GetMonsterOption(myname, "icon"))].sprite;
                uiName.text = NetworkMaster.Instance.GetMonsterOption(myname, "name");
                uiDamage.text = "DAMAGE : " + NetworkMaster.Instance.GetMonsterOption(myname, "damge");
                uiHp.text = "HP : " + NetworkMaster.Instance.GetMonsterOption(myname, "mhp");
                uiSpeed.text = "SPEED : " + NetworkMaster.Instance.GetMonsterOption(myname, "speed");
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
                uiBuildNextName.text = NetworkMaster.Instance.GetMonsterOption(nextPlayerName, "nickname")!="null"? NetworkMaster.Instance.GetMonsterOption(nextPlayerName, "nickname"):"-";
                uiBuildCost.text =MainGameManager.mainGameManager.StringDot(NetworkMaster.Instance.GetMonsterOption(nowPlayerName, "cost")) + " G";

            }
            else if (openedType == 3)
            {
                //덫 소환

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
}
