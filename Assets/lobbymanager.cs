using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.SceneManagement;

public class lobbymanager  : MonoBehaviourPunCallbacks
{
    public GameObject matchingObj;
    public Text nametext;
    public Text infotext;
    public Button joinBtn,StopBtn;
    public Text Temp_authId;
    public TextMeshProUGUI timeStamp;

    public TextMeshProUGUI tearTxt,winPointTxt,losePointTxt,goldTxt;
    public Text nameTxt;

    public bool testUser,isStart;
    public float FindTime;
    // Start is called before the first frame update
    public static readonly string gameVersion = "1";
    void Start()
    {
        if(SceneVarScript.Instance==null){
            SceneManager.LoadScene("LoginScean");
            return;
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
        SceneVarScript.Instance.LoginCourseFun(SceneVarScript.Instance.GetAuthCode());
        SetUserInfoUI();
        
    }

    // Update is called once per frame
    void Update()
    {
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
            timeStamp.text = $"( {(Mathf.Floor(FindTime / 60) < 10 ? "0" + Mathf.Floor(FindTime / 60).ToString("N0") : Mathf.Floor(FindTime / 60).ToString("N0"))} : {(Mathf.Floor(FindTime%60)<10?  "0"+ Mathf.Floor(FindTime % 60).ToString("N0") : Mathf.Floor(FindTime % 60).ToString("N0"))} )"; ;
            if (PhotonNetwork.CurrentRoom.PlayerCount > 1 )
            {
                infotext.text = "잠시후 게임이 시작 됩니다.";
                if (PhotonNetwork.IsMasterClient && isStart==false)
                {
                    isStart = true;
                    IntoTheRoom();
                    return;
                }

            }
        }
    
    }
    public void SetUserInfoUI()
    {
        if (SceneVarScript.Instance!=null&& SceneVarScript.Instance.userInfo != null)
        {
            nameTxt.text = SceneVarScript.Instance.GetUserOption("username");
            tearTxt.text = SceneVarScript.Instance.GetUserOption("tear");
            winPointTxt.text = "WIN : " + SceneVarScript.Instance.GetUserOption("win");
            losePointTxt.text = "LOSE : " + SceneVarScript.Instance.GetUserOption("lose");
            goldTxt.text = "GOLD : " + SceneVarScript.Instance.GetUserOption("money") + " G";
        }
        else
        {
            nameTxt.text = "null";
            tearTxt.text = "null";
            winPointTxt.text = "WIN : " + 0;
            losePointTxt.text = "LOSE : " + 0;
            goldTxt.text = "GOLD : " + 0 + " G";
        }
    }
    public void IntoTheRoom()
    {
        Hashtable CP = PhotonNetwork.CurrentRoom.CustomProperties;
        if ((bool)CP["isTest"] == false)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; //방 온/오프 
            PhotonNetwork.CurrentRoom.IsVisible = false; //방리스트에서 보여지는것
        }
        PhotonNetwork.LoadLevel("GameScean");
    }
    public override void OnConnectedToMaster()
    {
        infotext.text = $"연결완료";
        joinBtn.interactable = true;
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        infotext.text = $"연결 종료됨 (이유:{cause.ToString()})";
    }

    public void Connect()
    {
        SceneVarScript.Instance.RequestMonsterDB();
        //데이타베이스 연결되었을때만 실행
        if (SceneVarScript.Instance.isDataConnect == false)
        {
            SceneVarScript.Instance.RequestMonsterDB();
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
        roomOptions.CustomRoomProperties =new Hashtable { {"isTest",testUser}};
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnJoinedRoom()
    {
        FindTime = 0;
        if (testUser == true)
        {
            IntoTheRoom();
            testUser = false;
        }
    }

}
