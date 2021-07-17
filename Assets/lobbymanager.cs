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
    public GameObject matchingObj;
    public Text nametext;
    public Text infotext;
    public Button joinBtn, StopBtn;
    public TextMeshProUGUI timeStamp;

    public TextMeshProUGUI tearTxt, winPointTxt, losePointTxt;
    public Text goldTxt;
    public Text nameTxt;

    public bool testUser, isStart;
    public float FindTime;

    public GameObject[] uiList;
    public Vector3[] uiStartScaleList;

    public float scalePower;

    public GameObject skillScrollInner; // 생성될 스크롤 객체
    public GameObject skillBtn; // 생성할 버튼 프리펩
    public GameObject skillTarget; // 스킬버튼 드래그시 나타날 아이콘

    [Header("skill Desc UI")]
    public string selectedSkill;
    public Image skillImageUI;
    public Text skillOptionUI, skillNeedDescUI,skillDescUI;

    public SkillBtn[] useSkillList;

    // Start is called before the first frame update
    public static readonly string gameVersion = "1";
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

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinBtn.interactable = false;
        infotext.text = "연결중";

        /*-***************************/
        SceneVarScript.Instance.LoginCourseFun(SceneVarScript.Instance.GetAuthCode());
        // 구글플레이 > 파이어베이스 > 파이어베이스 데이터베이스 순서 이므로 이 과정도 auth에서 해결하고 lobby로 오도록 해야할듯
        /*-***************************/

        SetUserInfoUI();
        SetSkillList(); // 모든 스킬 렌더링 ( 오브젝트 복사로)
    }

    // Update is called once per frame
    void Update()
    {
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
            matchingObj.SetActive(true);
            joinBtn.interactable = false;
            StopBtn.interactable = true;
            FindTime += Time.deltaTime;
            timeStamp.text = $"( {(Mathf.Floor(FindTime / 60) < 10 ? "0" + Mathf.Floor(FindTime / 60).ToString("N0") : Mathf.Floor(FindTime / 60).ToString("N0"))} : {(Mathf.Floor(FindTime % 60) < 10 ? "0" + Mathf.Floor(FindTime % 60).ToString("N0") : Mathf.Floor(FindTime % 60).ToString("N0"))} )"; ;
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                infotext.text = "잠시후 게임이 시작 됩니다.";
                if (PhotonNetwork.IsMasterClient && isStart == false)
                {
                    isStart = true;
                    StartCoroutine(IntoTheRoom());
                    return;
                }

            }
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
        foreach (var item in SceneVarScript.Instance.skillOption)
        {
            var obj = Instantiate(skillBtn, skillScrollInner.transform, false);
            obj.GetComponent<SkillBtn>().myName = item["name"].ToString();
        }
    }
    public void SetSkillDescUI()
    {
        if (selectedSkill.Length > 0)
        {
            skillImageUI.sprite = SceneVarScript.Instance.skillIcon[int.Parse(SceneVarScript.Instance.GetOptionByName(selectedSkill, "icon", SceneVarScript.Instance.skillOption))];
            skillOptionUI.text = $"[{SceneVarScript.Instance.GetOptionByName(selectedSkill, "state", SceneVarScript.Instance.skillOption)}]\n{SceneVarScript.Instance.GetOptionByName(selectedSkill, "nickname", SceneVarScript.Instance.skillOption)}";
            skillNeedDescUI.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(selectedSkill, "needDesc", SceneVarScript.Instance.skillOption));
            skillDescUI.text = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(selectedSkill, "desc", SceneVarScript.Instance.skillOption));
        }
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
                nameTxt.text = "ID:" + SceneVarScript.Instance.GetFirebaseUser().DisplayName;
            }
            tearTxt.text = SceneVarScript.Instance.GetUserOption("tear");
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
        if ((bool)CP["isTest"] == false)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //방 온/오프 
            PhotonNetwork.CurrentRoom.IsVisible = false; //방리스트에서 보여지는것
        }
        Debug.Log("1초뒤 입장합니다.");
        yield return new WaitForSeconds(1);

        PhotonNetwork.LoadLevel("GameScean");
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
        SceneVarScript.Instance.RequestAllDB();
        //데이타베이스 연결되었을때만 실행
        if (SceneVarScript.Instance.isDataConnect == false)
        {
            SceneVarScript.Instance.RequestAllDB();
            Debug.Log("Error03, Check your Network!");
            return;
        }

        if (PhotonNetwork.IsConnected)
        {
            joinBtn.interactable = false;
            //infotext.text = "랜덤한 방으로 들어가는중입니다.";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            infotext.text = "서버 연결 실패로 재접속 시도 합니다.";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public void RoomFindStop()
    {
        StopBtn.interactable = false;
        PhotonNetwork.LeaveRoom();

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
        StartCoroutine(TransScale(obj));
    }
    public void UiClsoing(GameObject obj)
    {
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
        obj.SetActive(true);
        obj.transform.localScale *= 0.3f;
        var scaleAddPower = scalePower;
        while (uiStartScaleList[index].x > obj.transform.localScale.x)
        {
            scaleAddPower += Time.deltaTime;
            obj.transform.localScale *= 1 + (scaleAddPower * Time.deltaTime);
            yield return null;
        }
        if (uiStartScaleList[index].x > obj.transform.localScale.x)
        {
            obj.transform.localScale = uiStartScaleList[index];
        }
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
        while (uiStartScaleList[index].x * 0.7 < obj.transform.localScale.x)
        {
            obj.transform.localScale *= 1 - (scalePower * Time.deltaTime);
            yield return null;
        }
        obj.SetActive(false);
    }
}
