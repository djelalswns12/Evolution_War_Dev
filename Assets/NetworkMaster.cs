using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using Firebase.Database;
using System.Collections;
#pragma warning disable 649
public class hihi
{
	public static int a;
}
public class NetworkMaster : MonoBehaviourPunCallbacks
{
	//RoomOptions.CleanupCacheOnLeave 룸설정을 이렇게 해놓으면 클라이언트 나가도 삭제하지 않는다.
	//photonView.TransferOwnership() 오브젝트의 소유권을 이전한다
	#region Public Fields

	static public NetworkMaster Instance;

    #endregion

    #region Private Fields

	private void ReadJson()
    {
	
    }
	
	private GameObject instance;
	// 0이름, 1비용 , 2드랍골드,3공중여부,4데미지 ,5최대체력 , 6이름,7스피드 
	
	[Tooltip("The prefab to use for representing the player")]
	[SerializeField]
	private GameObject playerPrefab;
	public static GameObject player = null,otherPlayer;
	public PhotonView pv;
	public GameObject background;
	public int creatnumber; //유닛 생성 번호 위치에 따라 추후 변경되기도 함
	public float editorSpeed; // 빠른 디버깅을 위하여 에디터에서만 캐릭터 이동속도 증가하는곳에 쓰임 
	public int setLayer; // 유닛 생성시 레이어 설정
	public int playTest; // 1 : 왼쪽플레이어 생성 , -1:오른쪽 플레이어 생성
	public float CreatposY;
	public bool dir;
	public int endPoint; // 0 : 게임중  1: hp 계산완료 결과창 드랍 2:종료상태
	public GameObject EndingMsgBox;
	public Text EndingMsg;
	public bool otherPlayerHasBeen; // 접속하여 생성되었는지 확인

	public IDictionary[] monsterOption;
    #endregion

    #region MonoBehaviour CallBacks
    void Awake()
    {
	
	}
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
	{
		monsterOption = SceneVarScript.Instance.monsterOption;
		endPoint = 0;
		Instance = this;
		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.IsConnected)
		{
			//SceneManager.LoadScene("PunBasics-Launcher");
			return;
		}

		if (playerPrefab == null)
		{ // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

			//Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
		}
		else
		{


			if (player == null)
			{
				int playerX = PhotonNetwork.IsMasterClient ? 1 : 0;//1마스터 : 0일반유저;
				dir = playerX==1 ? true : false;
                if (playTest == -1)
                {
					dir = false;
                }
				//int playerX = PhotonNetwork.CurrentRoom.PlayerCount % 2;
				Debug.LogFormat("해당 씬에 플레이어 생성합니다. {0}", SceneManagerHelper.ActiveSceneName);

				// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
				float playerpos = background.GetComponent<SpriteRenderer>().bounds.size.x / 2 * 0.85f;
				Vector3 setCameraPosition=new Vector3(playerX == (1* playTest) ? -playerpos : playerpos, 0f, Camera.main.transform.position.z);
				Camera.main.GetComponent<CameraScript>().SetCameraMove(setCameraPosition);
				
				player = PhotonNetwork.Instantiate("Player", new Vector3(playerX == (1*playTest) ? -playerpos : playerpos, CreatposY, 0f), Quaternion.identity, 0);
				player.GetComponent<monsterScript>().creatnumber = creatnumber++;
				player.GetComponent<PlayerScript>().dir = dir;
				player.layer =  LayerMask.NameToLayer("centerunit");
                if (photonView.IsMine)
                {
					StartCoroutine(SetBoss("HumanBoss"));
				}
			}
			else
			{
				Debug.LogFormat("해당씬에 플레이어생성이 무시됨 : {0}", SceneManagerHelper.ActiveSceneName);
			}


		}

	}
	void Update()
	{
		monsterOption = SceneVarScript.Instance.monsterOption;
		//Debug.Log(MainGameManager.mainGameManager.GetMoney());
		if (PhotonNetwork.IsConnected == false)
		{
			SceneManager.LoadScene("LobbyScean");
			//SceneManager.LoadScene("LoginScean");
		}
		float playerpos = background.GetComponent<SpriteRenderer>().bounds.size.x / 2 * 0.85f;
		Vector2 focusPos = GradiantPos.Instance.par.position;
		focusPos.x = dir ? -playerpos * 1.2f : playerpos * 1.2f;
		GradiantPos.Instance.par.position = focusPos;
		if (pv.IsMine)
        {
			if (PhotonNetwork.IsMasterClient == true)
            {
				//내플레이어 체력이 0인지 확인하는 rpc를 보낸다
				//0이라면 당신이 이겼다라는 rpc를 보낸다
				if (endPoint == 0)
				{
					if (player.GetComponent<monsterScript>().hp <= 0)
					{
						endPoint = 1;
						pv.RPC("Win", RpcTarget.All, !dir);
					}
					if ((otherPlayer && otherPlayer.GetComponent<monsterScript>().hp <= 0))
					{
						endPoint = 1;
						pv.RPC("Win", RpcTarget.All, dir);
					}
                    if (otherPlayer == null && otherPlayerHasBeen)
                    {
						endPoint = 1;
						pv.RPC("Win", RpcTarget.All, dir);
					}
				}
			}
		}
	}

	#endregion

	#region Photon Callbacks

	/// <summary>
	/// Called when a Photon Player got connected. We need to then load a bigger scene.
	/// </summary>
	/// <param name="other">Other.</param>
	public override void OnPlayerEnteredRoom(Player other)
	{
		
	}

	/// <summary>
	/// Called when a Photon Player got disconnected. We need to load a smaller scene.
	/// </summary>
	/// <param name="other">Other.</param>
	public override void OnPlayerLeftRoom(Player other)
	{

	}


	/// <summary>
	/// Called when the local player left the room. We need to load the launcher scene.
	/// </summary>
	public override void OnLeftRoom()
	{
		SceneManager.LoadScene("LobbyScean");
	}

	#endregion
	#region Public Methods

	public string GetMonsterOption(string index0, string index1)
	{
		if(monsterOption!=null)
		foreach(IDictionary option in monsterOption)
        {
            if (option["name"].ToString() == index0)
            {
				return option[index1].ToString();
            }
        }
		return "null";
	}
	
	public bool DiscountCreatMoney(string name)
	{
		if (MainGameManager.mainGameManager.GetMoney() >= int.Parse(GetMonsterOption(name, "cost")))
		{
			MainGameManager.mainGameManager.CountMoney(-int.Parse(GetMonsterOption(name, "cost")));
			return true;
		}
		else
		{
			return false;
		}
	}
	public void CreatThrow(string name,Vector2 creatpos,int damage,monsterScript creatmonster,GameObject targetmonster)
    {
		GameObject throwObject = PhotonNetwork.Instantiate(name, creatpos, Quaternion.identity, 0);
		throwObject.GetComponent<ThrowScript>().target = targetmonster.transform.position;
		throwObject.GetComponent<ThrowScript>().damage = damage;
		throwObject.GetComponent<ThrowScript>().dir = creatmonster.dir;
		throwObject.GetComponent<ThrowScript>().whatIsLayer2 = creatmonster.whatIsLayer2;
	}
	public void CreatMonster(string name)
	{
		int creatIndex = GetMonsterOption(name, "name") == "null" ? -1 : 1;
		if (creatIndex == -1)
		{
			Debug.Log($"존재하지 않는 몬스터입니다 요청 몬스터:{name}");
			return;
		}
		if (!DiscountCreatMoney(name)) 
		{
			Debug.Log($"소환에 필요한 돈이 부족합니다.");
			return;
		}
		GameObject monster;
		Vector3 creatpos = player.transform.position;
		SpriteRenderer sp=((GameObject)Resources.Load(name)).GetComponent<SpriteRenderer>();
		creatpos.y += sp.bounds.size.y / 2 - (player.GetComponent<SpriteRenderer>().bounds.size.y / 2) + (setLayer == 0 ? 0.8f : 2.1f);


		if (player.GetComponent<PlayerScript>().dir == true)
        {
			//왼쪽 진영 소환
			creatpos.x += 3+(setLayer == 0 ? 0 : 1.0f);
			monster=PhotonNetwork.Instantiate(name, creatpos, Quaternion.identity, 0);
        }
        else
        {
			//오른쪽 진영 소환
			creatpos.x += -3 + (setLayer == 0 ? 0 : -1.0f);
			monster = PhotonNetwork.Instantiate(name, creatpos, Quaternion.identity, 0);
		}
		//공중 여부 결정
		if (GetMonsterOption(name, "flystate") == "0")
		{
			monster.layer = setLayer == 0 ? LayerMask.NameToLayer("downunit") : LayerMask.NameToLayer("upunit");

		}
		else
        {
			monster.layer = setLayer == 0 ? LayerMask.NameToLayer("downflyunit") : LayerMask.NameToLayer("upflyunit");
		}
		SetCreatureInfo(monster, name);

	}
	void SetCreatureInfo(GameObject monster, string name)
	{
		monsterScript instanceMonster = monster.GetComponent<monsterScript>();
		instanceMonster.creatnumber = creatnumber++;
		instanceMonster.dropMoney = int.Parse(GetMonsterOption(name, "dropcost"));
		instanceMonster.flystate = int.Parse(GetMonsterOption(name, "flystate"));
		instanceMonster.damage = int.Parse(GetMonsterOption(name, "damge"));
		instanceMonster.mhp = int.Parse(GetMonsterOption(name, "mhp"));
		instanceMonster.speed = int.Parse(GetMonsterOption(name, "speed")) * 0.1f;
		instanceMonster.hp = monster.GetComponent<monsterScript>().mhp;
	}
	public IEnumerator SetBoss(string name)
    {
		if (PhotonNetwork.IsMasterClient && pv.IsMine)
		{
			int dataParse = 0;
			for (;dataParse==0 ; )
			{
				if (player == null)
				{
					Debug.Log("플레이어가 존재하지 않습니다. 보스 소환을 위해 재요청합니다.");
					yield return new WaitForSeconds(0.1f);
				}
				int creatIndex = GetMonsterOption(name, "name") == "null" ? -1 : 1;
				if (creatIndex == -1)
				{
					Debug.Log($"존재하지 않는 몬스터입니다. 요청 몬스터:{name} 보스 소환을 위해 재요청합니다.");
					yield return new WaitForSeconds(0.1f);
                }
                else
                {
					dataParse = 1;
                }
			}
			Vector3 pos = new Vector3(background.transform.position.x,CreatposY,background.transform.position.z);
			GameObject monster=PhotonNetwork.Instantiate(name, pos, Quaternion.identity, 0);
			monster.layer = LayerMask.NameToLayer("upunit");
			SetCreatureInfo(monster, name);
		}

	}

	//위아래 층 정하는 함수
	public void SetLayer(int layernum)
    {
		if (Application.isEditor)
			MainGameManager.mainGameManager.CountMoney(500000);
		setLayer = layernum;
    }
	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void QuitApplication()
	{
		Application.Quit();
	}
	public void EndAction(string result)
	{
		endPoint = 2;
		if (result == "Win")
		{
			EndingMsgBox.SetActive(true);
			EndingMsg.text = "승리";
		}
		else if (result == "Lose")
		{
			EndingMsgBox.SetActive(true);
			EndingMsg.text = "패배";
		}
	}
	#endregion

	[PunRPC]
	public void Win(bool Winner)
    {
        if (Winner == dir)
        {
			EndAction("Win");
        }
        else
        {
			EndAction("Lose");
		}
    }


}
