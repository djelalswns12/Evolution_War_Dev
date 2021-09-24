using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.SceneManagement;

public class lobbymanager : MonoBehaviourPunCallbacks
{
    public static lobbymanager Instance;
    public int gameMode;
    public GameObject matchingObj;
    public Text nametext;
    public Text infotext;
    public Button joinBtn, StopBtn,testJoinBtn;
    public Text timeStamp;

    public TextMeshProUGUI tearTxt, winPointTxt, losePointTxt;
    public Text goldTxt;
    public Text nameTxt;

    public bool testUser, isStart,isAds;
    public float FindTime;
    public int RoomWait;

    public int yesOrNoInput = 0; // No:0 Yes:1
    public int noticeIndex;
    public GameObject[] uiList;
    public Vector3[] uiStartScaleList;

    public float scalePower;

    public GameObject skillScrollInner; // 생성될 스크롤 객체
    public GameObject skillBtn; // 생성할 스킬버튼 프리펩
    public GameObject skillTarget; // 스킬버튼 드래그시 나타날 아이콘

    public GameObject skillShopScrollInner; // 생성될 상점스크롤 객체
    public GameObject skillShopBtn; // 생성할 상점버튼 프리펩

    [Header("skill Desc UI")]
    public string selectedSkill;
    public Image skillImageUI;
    public Text skillOptionUI, skillNeedDescUI,skillDescUI,uiLobbyMsg,uiLobbyYesOrNoMsg;

    [Header("skillOption Desc UI")]
    public Image skillOptionImageUI;
    public Text skillOptionOptionUI, skillOptionNeedDescUI, skillOptionDescUI;

    public SkillBtn[] useSkillList;
    public Stack<SkillBtn> hasSkillList=new Stack<SkillBtn>();

    
    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
        if (SceneVarScript.Instance == null)
        {
            SceneManager.LoadScene("LoginScean");
            return;
        }
    }
    void Start()
    {
        Debug.Log("Lobby is Loading");
        if (!Application.isEditor)
        {
            testJoinBtn.gameObject.SetActive(false);
        }
        AdsManager.Instance.ShowBanner();
        RoomWait = 0;
        if (SceneVarScript.Instance == null)
        {
            SceneManager.LoadScene("LoginScean");
            return;
        }
  
        uiStartScaleList = new Vector3[uiList.Length];
        for (int i = 0; i < uiList.Length; i++)
        {
            uiStartScaleList[i] = uiList[i].transform.localScale;
            uiList[i].SetActive(false);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        if (AuthScript.user != null)
        {
            nametext.text = $"Hello! {AuthScript.user.Email}";
        }
        else
        {
            nametext.text = "유저이메일이 존재하지 않습니다.";
        }

        //PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinBtn.interactable = false;
        infotext.text = "연결중";

        /*-***************************/

        // 구글플레이 > 파이어베이스 > 파이어베이스 데이터베이스 순서 이므로 이 과정도 auth에서 해결하고 lobby로 오도록 해야할듯
        /*-***************************/
        BlindManager.Instance.CloseBlind();
        LobbySoundManager.Instance.BGMSoundPlay();
        SetUserInfoUI();
        SetShopSkillList();
        isAds = true;
        SceneVarScript.Instance.SetEnterLog();
    }

    // Update is called once per frame
    void Update()
    {
  
        SetSkillList(); // 모든 스킬 렌더링 ( 오브젝트 복사로)
        SetSkillDescUI();
        SetUseSkillList();
        SetUserInfoUI();
        //Temp_authId.text = SceneVarScript.Instance.GetAuthCode();
        if (PhotonNetwork.CurrentRoom == null)
        {
            StopBtn.interactable = false;
            matchingObj.SetActive(false);
        }
        else
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1 )
            {
                if (isStart == false)
                {
                    isStart = true;
                    LobbySoundManager.Instance.BGMSoundStop();
                    infotext.text = "잠시후 게임이 시작 됩니다.";
                    LobbySoundManager.Instance.MatchingSoundPlay();
                    timeStamp.text = "잠시후 시작됩니다.";
                    StopBtn.gameObject.SetActive(false);
                    StartCoroutine(IntoTheRoom());
                    return;
                }
            }
            else
            {
                matchingObj.SetActive(true);
                joinBtn.interactable = false;
                StopBtn.interactable = true;
                FindTime += Time.deltaTime;
                timeStamp.text = $"( {(Mathf.Floor(FindTime / 60) < 10 ? "0" + Mathf.Floor(FindTime / 60).ToString("N0") : Mathf.Floor(FindTime / 60).ToString("N0"))} : {(Mathf.Floor(FindTime % 60) < 10 ? "0" + Mathf.Floor(FindTime % 60).ToString("N0") : Mathf.Floor(FindTime % 60).ToString("N0"))} )"; ;

            }
        }
        if (RoomWait == 1)
        {
            //매칭완료 3초후 입장상태
            matchingObj.SetActive(true);
            joinBtn.interactable = false;
            StopBtn.gameObject.SetActive(false);
            StopBtn.interactable = false;
        }
    }
    public int SetUseSkillNumByDrag(GameObject obj)
    {
        for(int i = 0; i < useSkillList.Length; i++)
        {
            if (useSkillList[i].gameObject == obj)
            {
                return i;
            }
        }
        return -1;
    }
    public void SetUseSkillList()
    {

        for (int i = 0; i < 3; i++)
        {
            useSkillList[i].myName =SceneVarScript.Instance.GetOptionByIndex(SceneVarScript.Instance.GetUserOption("skill" + (i + 1)),"name",SceneVarScript.Instance.skillOption);
            useSkillList[i].myNum = i;
        }
    }
    public void SetSkillList()
    {
        var skill=SceneVarScript.Instance.GetUserOption("Inventory");
      
        var skills=skill.Split('/');
        if (hasSkillList.Count < skills.Length-1)
            {
                while (hasSkillList.Count != skills.Length-1)
                {
                    var obj = Instantiate(skillBtn, skillScrollInner.transform, false).GetComponent<SkillBtn>();
                    hasSkillList.Push(obj);
                    //obj.GetComponent<SkillBtn>().myName = SceneVarScript.Instance.GetOptionByIndex(item,"name",SceneVarScript.Instance.skillOption);
                }
            }
        else if (hasSkillList.Count > skills.Length-1)
        {
            while (hasSkillList.Count != skills.Length-1)
            {
                Destroy(hasSkillList.Pop().gameObject);
                    //obj.GetComponent<SkillBtn>().myName = SceneVarScript.Instance.GetOptionByIndex(item,"name",SceneVarScript.Instance.skillOption);
                }
            }
        int skillindex = 0;
        foreach(SkillBtn item in hasSkillList)
        {
            item.myName = SceneVarScript.Instance.GetOptionByIndex(skills[skillindex], "name", SceneVarScript.Instance.skillOption);
            skillindex++;
        }
        }
    public void SetShopSkillList()
    {
        foreach (var item in SceneVarScript.Instance.skillShopOption)
        {
            var obj = Instantiate(skillShopBtn, skillShopScrollInner.transform, false).GetComponent<SkillBtnInShop>();
            obj.shopIndex = item["index"].ToString();
        }

    }
    public void SetSkillDescUI()
    {
        if (selectedSkill.Length > 0 && selectedSkill!="null")
        {
            skillImageUI.sprite = SceneVarScript.Instance.skillIcon[int.Parse(SceneVarScript.Instance.GetOptionByName(selectedSkill, "icon", SceneVarScript.Instance.skillOption))];
            skillOptionUI.text = $"[{SceneVarScript.Instance.GetOptionByName(selectedSkill, "state", SceneVarScript.Instance.skillOption)}]\n{SceneVarScript.Instance.GetOptionByName(selectedSkill, "nickname", SceneVarScript.Instance.skillOption)}";
            skillNeedDescUI.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(selectedSkill, "needDesc", SceneVarScript.Instance.skillOption));
            skillDescUI.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(selectedSkill, "desc", SceneVarScript.Instance.skillOption));
        }
        else
        {
            skillImageUI.sprite = SceneVarScript.Instance.noneSkillIcon;
            skillOptionUI.text = "스킬을 드래하여 장착하세요.";
            skillNeedDescUI.text = "";
            skillDescUI.text = "";

        }
    }
    public void SetSkillOptionUI(string selectedSkill)
    {
        selectedSkill = SceneVarScript.Instance.GetOptionByIndex(selectedSkill, "name", SceneVarScript.Instance.skillOption);
        if (selectedSkill.Length > 0 && selectedSkill != "null")
        {
            skillOptionImageUI.sprite = SceneVarScript.Instance.skillIcon[int.Parse(SceneVarScript.Instance.GetOptionByName(selectedSkill, "icon", SceneVarScript.Instance.skillOption))];
            skillOptionOptionUI.text = $"[{SceneVarScript.Instance.GetOptionByName(selectedSkill, "state", SceneVarScript.Instance.skillOption)}]\n{SceneVarScript.Instance.GetOptionByName(selectedSkill, "nickname", SceneVarScript.Instance.skillOption)}";
            skillOptionNeedDescUI.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(selectedSkill, "needDesc", SceneVarScript.Instance.skillOption));
            skillOptionDescUI.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(selectedSkill, "desc", SceneVarScript.Instance.skillOption));
        }
        else
        {
            skillOptionImageUI.sprite = SceneVarScript.Instance.noneSkillIcon;
            skillOptionOptionUI.text = "";
            skillOptionNeedDescUI.text = "";
            skillOptionDescUI.text = "";

        }
    }
    public void SetLobbyMsg(string text)
    {
        UiOpening(uiList[3]);
        uiLobbyMsg.text = text;
    }
    public void SetLobbyYesOrNoMsg(string text)
    {
        UiOpening(uiList[4]);
        uiLobbyYesOrNoMsg.text = text;
    }
    public void YesOrNoInput(int num)
    {
        yesOrNoInput = num;
        UiClsoing(uiList[4]);
    }
    public void SetUserInfoUI()
    {
        if (SceneVarScript.Instance != null && SceneVarScript.Instance.userInfo != null)
        {
            if (Application.isEditor)
            {
                nameTxt.text = "ID:" + SceneVarScript.Instance.GetUserOption("username");
            }
            else
            {
                nameTxt.text = "ID:" + SceneVarScript.Instance.GetUserOption("username");
                //nameTxt.text = "ID:" + SceneVarScript.Instance.GetFirebaseUser().DisplayName;
            }
            tearTxt.text = "RATING : " + SceneVarScript.Instance.GetUserOption("rating");
            winPointTxt.text = "WIN : " + SceneVarScript.Instance.GetUserOption("win");
            losePointTxt.text = "LOSE : " + SceneVarScript.Instance.GetUserOption("lose");
            goldTxt.text = StringDot(SceneVarScript.Instance.GetUserOption("money")) + " G";
        }
        else
        {
            nameTxt.text = "-";
            tearTxt.text = "-";
            winPointTxt.text = "WIN : ";
            losePointTxt.text = "LOSE : ";
            goldTxt.text = "-";
        }
    }
    IEnumerator IntoTheRoom()
    {
       
            Hashtable CP = PhotonNetwork.CurrentRoom.CustomProperties;
            if ((bool)CP["isTest"] != false)
            {
                //테스트버전일때 조작할것
            }
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //방 온/오프 
            PhotonNetwork.CurrentRoom.IsVisible = false; //방리스트에서 보여지는것
        }
            Debug.Log("3초뒤 입장합니다.");
            RoomWait = 1;
            yield return new WaitForSeconds(3);
            BlindManager.Instance.OnBlind();
            RoomWait = 2;
            yield return new WaitForSeconds(1);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScean");
        }
    }
    public override void OnConnectedToMaster()
    {
        infotext.text = $"연결완료";
        joinBtn.interactable = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        //infotext.text = $"연결 종료됨 (이유:{cause.ToString()})";
    }

    public void Connect()
    {
        joinBtn.interactable = false;
        StartCoroutine(Connecting());
    }
    IEnumerator Connecting()
    {
        SceneVarScript.Instance.RequestAllDB();
        yield return new WaitForSeconds(1.5f);
        if (SceneVarScript.Instance.isDataConnect == false)
        {
            SetLobbyMsg("재접속 하거나 네트워크 연결을 확인하세요.");
            yield break;
        }
        Debug.Log("데이터 확인 완료");
        bool noneSkill=false;
        yesOrNoInput = 0;
        for (int i = 0; i < SceneVarScript.MAX_SKILL_COUNT; i++) {
            if(SceneVarScript.Instance.GetUserOption("skill" + i) == "-1")
            {
                noneSkill = true;
                break;
            }
         }
        if (noneSkill)
        {
            SetLobbyYesOrNoMsg("미장착된 스킬 슬롯이 있습니다.\n스킬을 장착하시겠습니까?");
            yield return new WaitUntil(() => uiList[4].activeSelf == false);
        }
        if (yesOrNoInput == 1)
        {
            joinBtn.interactable = true;
            UiOpening(uiList[2]);
            yield break;
        }
        if (PhotonNetwork.IsConnected)
        {
            isAds = false;
            //infotext.text = "랜덤한 방으로 들어가는중입니다.";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            SetLobbyMsg("네트워크 상태가 불안정합니다.");
            infotext.text = "서버 연결 실패로 재접속 시도 합니다.";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void RoomFindStop()
    {
        StopBtn.interactable = false;
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            
        }
    }
    public override void OnLeftRoom()
    {
        isAds = true;
    }
    public void TestUser()
    {
        testUser = true;
        Connect();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //빈방이 없을때 실행되는 함수 JoinRandomRoom이 실패할경우 실행된다.
        infotext.text = "빈방이 없으므로 방을 하나 새로 만듭니다.";
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.CleanupCacheOnLeave = false;
        roomOptions.CustomRoomProperties = new Hashtable { { "isTest", testUser } };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        FindTime = 0;
        if (testUser == true)
        {
            StartCoroutine(IntoTheRoom());
            testUser = false;
        }
    }
    public string StringDot<T>(T str)
    {
        string answer = "";
        string copy = str.ToString();
        for (int i = copy.Length - 1; i >= 0; i--)
        {
            answer = copy[i] + answer;
            if (((copy.Length - 1) - i) % 3 == 2 && i > 0)
            {
                answer = "," + answer;
            }
        }

        return answer;
    }
    public void UiOpening(GameObject obj)
    {
        LobbySoundManager.Instance.BtnClickSoundPlay();
        LobbySoundManager.Instance.UiOpenSoundPlay();
        StartCoroutine(TransScale(obj));
    }
    public void UiClsoing(GameObject obj)
    {
        LobbySoundManager.Instance.BtnClickSoundPlay();
        LobbySoundManager.Instance.UiCloseSoundPlay();
        StartCoroutine(ReduceScale(obj));
    }
    IEnumerator TransScale(GameObject obj)
    {
        int index = -1;
        for (int i = 0; i < uiList.Length; i++)
        {
            if (obj == uiList[i])
            {
                index = i;
                obj.transform.localScale = uiStartScaleList[i];
                break;
            }
        }
        if (index == -1)
        {
            yield break;
        }
        obj.transform.localScale *= 0.3f;
        obj.SetActive(true);
        var scaleAddPower = scalePower;
        float timer = 0;
        while (uiStartScaleList[index].x > obj.transform.localScale.x)
        {
            timer += Time.deltaTime;
            scaleAddPower += Time.deltaTime;
            obj.transform.localScale *= 1 + (scaleAddPower * Time.deltaTime);
            if (timer > 1)
            {
                Debug.Log("오류 걸려서 그냥 탈출 확대");
                break;
            }
            yield return null;
        }
            obj.transform.localScale = uiStartScaleList[index];
    }
    IEnumerator ReduceScale(GameObject obj)
    {
        int index = -1;
        for (int i = 0; i < uiList.Length; i++)
        {
            if (obj == uiList[i])
            {
                index = i;
                break;
            }
        }
        if (index == -1)
        {
            yield break;
        }
        float timer = 0;
        while (uiStartScaleList[index].x * 0.7 < obj.transform.localScale.x)
        {
            timer += Time.deltaTime;
            obj.transform.localScale *= 1 - (scalePower * Time.deltaTime);
            if (timer > 1)
            {
                Debug.Log("오류 걸려서 그냥 탈출 축소");
                break;
            }
            yield return null;
        }
        obj.SetActive(false);
    }
    public void ShowRewardAds()
    {
        AdsManager.Instance.ShowRewardedVideo();
    }
}
