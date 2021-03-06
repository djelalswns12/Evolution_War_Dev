using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using Firebase.Database;
using System.Collections;
using Hashtable = ExitGames.Client.Photon.Hashtable;

#pragma warning disable 649
[System.Serializable]
public class MapList
{
    public GameObject[] map;
}
public class NetworkMaster : MonoBehaviourPunCallbacks
{


    //RoomOptions.CleanupCacheOnLeave 룸설정을 이렇게 해놓으면 클라이언트 나가도 삭제하지 않는다.
    //photonView.TransferOwnership() 오브젝트의 소유권을 이전한다
    #region Public Fields

    static public NetworkMaster Instance;

    #endregion

    #region Private Fields


    private GameObject instance;

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerPrefab;
    public Hashtable gameMode;
    public static GameObject player = null, otherPlayer;
    public PhotonView pv;
    public GameObject background;
    public int creatnumber; //유닛 생성 번호 위치에 따라 추후 변경되기도 함
    public float editorSpeed; // 빠른 디버깅을 위하여 에디터에서만 캐릭터 이동속도 증가하는곳에 쓰임 
    public int setLayer; // 유닛 생성시 레이어 설정
    public bool StartPlayerPos; // true : 왼쪽플레이어 생성 , false:른쪽 플레이어 생성
    public float CreatposY;
    public Vector2 upSetPos, downSetPos;
    public bool dir;
    public int endPoint; // 0 : 게임중  1: hp 계산완료 결과창 드랍 2:종료상태
    public int endState; // 0:미정 1 : 승리 2:패배
    public int setRatingState;//0:미완료 1:초기설정 완료
    public GameObject EndingMsgBox;
    public Text EndingMsg;
    public bool otherPlayerHasBeen; // 접속하여 생성되었는지 확인
    public TextExpress textExpress;
    public int gameStage;
    public float playTime;
    public GameObject[] spawnList;
    public MapList[] groundSp;

    [Header("플레이어 건물 갯수")]
    public int buildMaxCnt;
    #endregion
    #region MonoBehaviour CallBacks
    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel("LobbyScean");
            //SceneManager.LoadScene("PunBasics-Launcher");
            return;
        }
        gameMode = PhotonNetwork.CurrentRoom.CustomProperties;
        if (GetMode() == "AI")
        {
            var startLocate = Random.Range(0, 2);

            if (startLocate == 0)
            {
                StartPlayerPos = true;
            }
            else
            {
                StartPlayerPos = false;
            }
        }
    }
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {

        EndingMsgBox.SetActive(false);
        endPoint = 0;
        Instance = this;
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LoadLevel("LoginScean");
        }

        if (playerPrefab == null)
        { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

            //Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            MakePlayer(PhotonNetwork.IsMasterClient==true? StartPlayerPos:!StartPlayerPos);
            if (GetMode()== "AI")
            {
                MakePlayer(PhotonNetwork.IsMasterClient == false ? StartPlayerPos : !StartPlayerPos);
            }
            if (photonView.IsMine)
                {
                    StartCoroutine(SetBoss("DragonBoss"));
                }
        }

    }
    void Update()
    {
        playTime += Time.deltaTime;
        //시대별 소환 유닛 항목 표기
        for (int k = 0; k < spawnList.Length; k++)
        {
            if (k <= gameStage)
            {
                spawnList[k].SetActive(true);
            }
            else
            {
                spawnList[k].SetActive(false);
            }
        }

        //시대별 배경 변환
        for (int i = 0; i < groundSp.Length; i++)
        {
            if (gameStage == i)
            {
                foreach (var item in groundSp[i].map)
                {
                    item.SetActive(true);
                }
            }
            else
            {
                foreach (var item in groundSp[i].map)
                {
                    item.SetActive(false);
                }
            }
        }
        //Debug.Log(MainGameManager.mainGameManager.GetMoney());
        if (PhotonNetwork.IsConnected == false)
        {
            //SceneManager.LoadScene("LobbyScean");

            SceneManager.LoadScene("LoginScean");
        }
        float playerpos = background.GetComponent<SpriteRenderer>().bounds.size.x / 2 * 0.85f;
        Vector2 focusPos = GradiantPos.Instance.par.position;
        focusPos.x = dir ? -playerpos * 1.2f : playerpos * 1.2f;
        GradiantPos.Instance.par.position = focusPos;
        if (pv.IsMine)
        {
            if (PhotonNetwork.IsMasterClient == true)
            {
                //0이라면 당신이 이겼다라는 rpc를 보낸다
                if (endPoint == 0)
                {
                    if (player.GetComponent<monsterScript>().hp <= 0)
                    {
                        endPoint = 1;
                        pv.RPC("Win", RpcTarget.All, !dir,playTime);
                    }
                    if ((otherPlayer && otherPlayer.GetComponent<monsterScript>().hp <= 0))
                    {
                        //상대방 유저 체력이 0이하라면
                        endPoint = 1;
                        pv.RPC("Win", RpcTarget.All, dir, playTime);
                    }
                    if (otherPlayer && otherPlayer.GetComponent<PhotonView>().IsMine && otherPlayerHasBeen)
                    {
                        //상대방이 들어온적이 있는데 튕겨서 나가진거라면
                        if (GetMode() != "AI")
                        {
                            otherPlayer.GetComponent<monsterScript>().hp = 0;
                            endPoint = 1;
                            pv.RPC("Win", RpcTarget.All, dir, playTime);
                        }
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
        Debug.Log("Player is Left");
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
    public void UpgradeBuild()
    {
        UpgradeBuildByPlayer(player);
    }
    public bool UpgradeBuildByPlayer(GameObject player)
    {
        var playerScript = player.GetComponent<monsterScript>();
        var nowPlayerName = playerScript.myName;
        var nowBuild = int.Parse(NetworkMaster.Instance.GetMonsterOption(nowPlayerName, "icon")) - 3000 + 1;
        var buildCost = int.Parse(GetMonsterOption(nowPlayerName, "cost"));
        if (nowBuild - 1 < gameStage)
        {
            if (player == NetworkMaster.player)
            {
                if (!MainGameManager.mainGameManager.SpentGold(buildCost))
                {
                    return false;
                }
            }
            else
            {
                if (!AIManager.Instance.SpentGold(buildCost))
                {
                    return false;
                }
            }
            if (nowBuild + 1 < buildMaxCnt)
            {
                var nextPlayerName = "Player" + (int.Parse(NetworkMaster.Instance.GetMonsterOption(NetworkMaster.player.GetComponent<monsterScript>().myName, "icon")) - 3000 + 2);
                playerScript.hp += int.Parse(NetworkMaster.Instance.GetMonsterOption(nextPlayerName, "mhp")) * 0.5f;
                SetCreatureInfo(player, "Player" + (nowBuild + 1), player);
                if (player == NetworkMaster.player)
                {
                    MainGameManager.mainGameManager.SetPlayerBuliding(nowBuild);
                }
                else if (player == NetworkMaster.otherPlayer)
                {
                    AIManager.Instance.SetPlayerBuliding(nowBuild);
                }
                return true;
            }
            else
            {
                if (player == NetworkMaster.player) SendGameMsgFunc("더이상 업그레이드 할 수 없습니다.", 0);
                else Debug.Log("더이상 업그레이드 할 수 없습니다.");
                return false;
            }

        }
        else
        {
            if (player == NetworkMaster.player) { SendGameMsgFunc("알맞지 않은 시대라 업그레이드 할 수 없습니다.", 0); }
            else
            {
                //Debug.Log("알맞지 않은 시대라 업그레이드 할 수 없습니다.");
            }
            return false;
        }
    }
    public void UpgradeTrap()
    {
        UpgradeTrapByPlayer(player, MainGameManager.mainGameManager.GetNowMonster());
    }
    public void UpgradeTrapByPlayer(GameObject player,GameObject nowMonster)
    {
        var playerScript = player.GetComponent<monsterScript>();
        var nowPlayerName = playerScript.myName;
        var nowBuild = int.Parse(NetworkMaster.Instance.GetMonsterOption(nowPlayerName, "icon")) - 3000;
        if (nowMonster == null)
        {
            if (player == NetworkMaster.player)
            {
                SendGameMsgFunc("업그레이드할 트랩이 존재하지 않습니다.", 0);
            }else Debug.Log("업그레이드할 트랩이 존재하지 않습니다.");
            return;
        }
        var nowMonsterScript = nowMonster.GetComponent<monsterScript>();
        if (SceneVarScript.Instance.GetOptionByName(nowMonsterScript.myName, "canBuild", SceneVarScript.Instance.trapOption) == "null")
        {
            if (player == NetworkMaster.player)
            {
                SendGameMsgFunc("더이상 업그레이드 할 수 없습니다.", 0);
            }
            else Debug.Log("더이상 업그레이드 못함");
            return;
        }
        if (nowBuild < int.Parse(SceneVarScript.Instance.GetOptionByName(nowMonsterScript.myName, "canBuild", SceneVarScript.Instance.trapOption)))
        {
            if (player == NetworkMaster.player)
            {
                SendGameMsgFunc("건물 업그레이드가 부족합니다.", 0);
            }
            else Debug.Log("건물 업그레이드 부족");
            return;
        }
        if (player == NetworkMaster.player)
        {
            if (!MainGameManager.mainGameManager.SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(nowMonsterScript.myName, "upgradeCost", SceneVarScript.Instance.trapOption))))
            {
                return;
            }
             MainGameManager.mainGameManager.RightNav.GetComponent<RightNav>().CloseNav();
        }
        else
        {
            if (!AIManager.Instance.SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(nowMonsterScript.myName, "upgradeCost", SceneVarScript.Instance.trapOption))))
            {
                return;
            }
        }
            var nextIndex = SceneVarScript.Instance.GetOptionByName(nowMonsterScript.myName, "nextIndex", SceneVarScript.Instance.trapOption);
            var nextName = SceneVarScript.Instance.GetOptionByIndex(nextIndex, "name", SceneVarScript.Instance.trapOption);
            nowMonsterScript.hp = 0;
            CreatMonster(nextName, 1, nowMonster.transform.position.x, nowMonsterScript.GetLayerNum(),player);
           
    }
    public string GetMonsterOption(string index0, string index1)
    {
        var monsterDB = SceneVarScript.Instance.monsterOption;
        if (monsterDB != null)
            foreach (IDictionary option in monsterDB)
            {
                if (option["name"].ToString() == index0)
                {
                    if (option[index1] == null)
                    {
                        break;
                    }
                    return option[index1].ToString();
                }
            }
        return "null";
    }
    public void CreatThrow(string name, Vector2 creatpos, int damage, monsterScript creatmonster, GameObject targetmonster,monsterScript par, float bonusMoney=0)
    {
        GameObject throwObject = PhotonNetwork.Instantiate(name, creatpos, Quaternion.identity, 0);
        var script = throwObject.GetComponent<ThrowScript>();
        script.par = par;
        script.target = targetmonster.transform.position;
        script.damage = damage;
        script.dir = creatmonster.dir;
        script.whatIsLayer2 = creatmonster.whatIsLayer2;
        script.bonusMoney = bonusMoney;
    }
    public GameObject CreatMonster(string name, int createType /*0이 아닌경우는 특정위치에서 소환되어야 할때 */, float posX ,int setLayer,GameObject player)
    {
        //UpLayer:1
        //DownLayer:0
        if (setLayer == -1)
        {
            setLayer=GetLayer();
        }
        int creatIndex = GetMonsterOption(name, "name") == "null" ? -1 : 1;
        if (creatIndex == -1)
        {
            Debug.Log($"존재하지 않는 몬스터입니다 요청 몬스터:{name}");
            return null;
        }
        GameObject monster;
        Vector3 creatpos = player.transform.position;
        var script = ((GameObject)Resources.Load(name)).GetComponent<monsterScript>();
        creatpos.y = (setLayer == 0 ? downSetPos.y : upSetPos.y) + Mathf.Abs(script.spawnPos.y);
        if (createType == 0)
        {
            creatpos.x = CreatPosXOffset(player);
        }
        else if (createType == 1)
        {
            creatpos.x = posX;
        }
        monster = PhotonNetwork.Instantiate(name, creatpos, Quaternion.identity, 0);

        if (monster.tag != "trap")
        {
            //공중 여부 결정
            if (GetMonsterOption(name, "flystate") == "0")
            {
                monster.layer = setLayer == 0 ? LayerMask.NameToLayer("downunit") : LayerMask.NameToLayer("upunit");
            }
            else
            {
                monster.layer = setLayer == 0 ? LayerMask.NameToLayer("downflyunit") : LayerMask.NameToLayer("upflyunit");
            }
        }
        else if (monster.tag == "trap")
        {
            if (GetMonsterOption(name, "flystate") == "0")
            {
                monster.layer = setLayer == 0 ? LayerMask.NameToLayer("downTrap") : LayerMask.NameToLayer("upTrap");
            }
            else
            {
                monster.layer = setLayer == 0 ? LayerMask.NameToLayer("downTrap") : LayerMask.NameToLayer("upTrap");
            }
        }
        SetCreatureInfo(monster, name,player);
        return monster;
    }
    public float CreatPosXOffset(GameObject player)
    {
        Vector3 creatpos = player.transform.position;
        if (player.GetComponent<PlayerScript>().dir == true)
        {
            //왼쪽 진영 소환
            creatpos.x += 3 + (setLayer == 0 ? 0 : 1.0f);
        }
        else
        {
            //오른쪽 진영 소환
            creatpos.x += -3 + (setLayer == 0 ? 0 : -1.0f);
        }
        return creatpos.x;
    }
    void SetCreatureInfo(GameObject monster, string name,GameObject player)
    {
        monsterScript instanceMonster = monster.GetComponent<monsterScript>();
        instanceMonster.myPlayer = player.GetComponent<PlayerScript>();
        instanceMonster.creatnumber = creatnumber++;
        instanceMonster.myName = GetMonsterOption(name, "name").ToString();
        instanceMonster.dropMoney = int.Parse(GetMonsterOption(name, "dropcost"));
        instanceMonster.flystate = int.Parse(GetMonsterOption(name, "flystate"));
        instanceMonster.damage = int.Parse(GetMonsterOption(name, "damge"));
        instanceMonster.mhp = int.Parse(GetMonsterOption(name, "mhp"));
        instanceMonster.speed = int.Parse(GetMonsterOption(name, "speed")) * 0.1f;
        if (instanceMonster.tag != "Player")
        {
            instanceMonster.hp = monster.GetComponent<monsterScript>().mhp;
        }
    }
    public IEnumerator SetBoss(string name)
    {
        if (PhotonNetwork.IsMasterClient && pv.IsMine)
        {
            int dataParse = 0;
            for (; dataParse == 0;)
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
            if (name == "DragonBoss")
            {
                yield return new WaitForSeconds(1f);
                SendMainMsgFunc("- 고생대 -\n지구 태초의 생물들", 1);
                pv.RPC("SetStage", RpcTarget.All, 0);
                if (!Application.isEditor)
                {
                    yield return new WaitForSeconds(6f);
                    SendMainMsgFunc("10초후 고생대 보스가 등장합니다.", 1);
                    yield return new WaitForSeconds(10f);
                }
                else
                {
                    //yield return new WaitForSeconds(2f);
                    SendMainMsgFunc("1초후 고생대 보스가 등장합니다.", 1);
                    yield return new WaitForSeconds(1f);
                }
            }
            else if (name == "MomBoss")
            {
                SendMainMsgFunc("잠시후 중생대 보스가 등장합니다.", 1);
                if (!Application.isEditor)
                {
                    yield return new WaitForSeconds(10f);
                }
            }
            else if (name == "HumanBoss")
            {
                SendMainMsgFunc("잠시후 신생대 보스가 등장합니다.", 1);
                if (!Application.isEditor)
                {
                    yield return new WaitForSeconds(10f);
                }
            }
            if (!Application.isEditor)
            {
                SendMainMsgFunc("3", 1);
                yield return new WaitForSeconds(1f);
                SendMainMsgFunc("2", 1);
                yield return new WaitForSeconds(1f);
                SendMainMsgFunc("1", 1);
                yield return new WaitForSeconds(1f);
            }
            SendMainMsgFunc("보스가 등장했습니다.", 1);
            Vector3 pos = new Vector3(background.transform.position.x, upSetPos.y, background.transform.position.z);
            GameObject monster = PhotonNetwork.Instantiate(name, pos, Quaternion.identity, 0);
            monster.layer = LayerMask.NameToLayer("upunit");
            SetCreatureInfo(monster, name,player);
            pv.RPC("SetStartRating", RpcTarget.All);
            Debug.Log("게임 정상적으로 시작되어서 점수가 깍입니다.");
            pv.RPC("CameraMove", RpcTarget.All, monster.transform.position);
            
        }
    }
    [PunRPC]
    public void CameraMove(Vector3 pos)
    {
        CameraScript.Instance.CameraMoveToPos(pos);
    }
    public void SetNextStage(string name)
    {
        if (name == "DragonBoss")
        {
            StartCoroutine(WaitNextStage(name));
        }
        if (name == "MomBoss")
        {
            StartCoroutine(WaitNextStage(name));
        }
        if (name == "HumanBoss")
        {
            StartCoroutine(WaitNextStage(name));
        }
    }
    IEnumerator WaitNextStage(string name)
    {
        yield return new WaitForSeconds(3f);
        if (name == "DragonBoss")
        {
            SendMainMsgFunc("잠시후 중생대가 다가옵니다.", 1);
        }
        if (name == "MomBoss")
        {
            SendMainMsgFunc("잠시후 신생대가 다가옵니다.", 1);
        }
        if (name == "HumanBoss")
        {
            SendMainMsgFunc("잠시후 문명시대가 다가옵니다.", 1);
        }
        yield return new WaitForSeconds(6f);
        if (name == "DragonBoss")
        {
            SendMainMsgFunc("- 중생대 -\n포유류들의 시대 [ 포유류 소환 잠금 해제 완료]", 1);
            pv.RPC("SetStage", RpcTarget.All, 1);
            yield return new WaitForSeconds(10f);
            StartCoroutine(SetBoss("MomBoss"));
            yield return null;
        }
        if (name == "MomBoss")
        {
            SendMainMsgFunc("- 신생대 -\n태초의 인류 [ 인류 소환 잠금 해제 완료]", 1);
            pv.RPC("SetStage", RpcTarget.All, 2);
            yield return new WaitForSeconds(10f);
            StartCoroutine(SetBoss("HumanBoss"));
            yield return null;
        }
        if (name == "HumanBoss")
        {
            SendMainMsgFunc("- 문명시대 -\n상대방의 기지를 파괴하여 본인의 문명을 지키세요!", 1);
            pv.RPC("SetStage", RpcTarget.All, 3);
            yield return null;
        }

    }
    public bool CreateTrap(string myname, GameObject player, Vector2 mousePos, int layer)
    {
        Vector2 bossPos;
        if (MainGameManager.mainGameManager.GetNowBoss() != null)
        {
            bossPos = MainGameManager.mainGameManager.GetNowBoss().transform.position;
            if (Mathf.Abs(mousePos.x - bossPos.x) < 3 && layer == 1)
            {
                if (player == NetworkMaster.player) SendGameMsgFunc("보스 근처에는 생성할 수 없습니다.", 0);
                else
                    Debug.Log("근처 보스");
                return false;
            }
        }
        if (Mathf.Abs(mousePos.x - NetworkMaster.player.transform.position.x) < 3 || (otherPlayer != null && Mathf.Abs(mousePos.x - otherPlayer.transform.position.x) < 3))
        {
            if (player == NetworkMaster.player) SendGameMsgFunc("플레이어 근처에는 생성할 수 없습니다.", 0);
            else
                Debug.Log("플레이어 근처 생성");

            return false;
        }
        LayerMask mask = layer == 1 ? LayerMask.GetMask("upTrap") : LayerMask.GetMask("downTrap");
        Collider2D[] otherTraps = Physics2D.OverlapBoxAll(mousePos, new Vector2(2, 2), 0, mask);
        if (otherTraps.Length > 0)
        {
            if (player == NetworkMaster.player) SendGameMsgFunc("근처에 이미 다른 트랩이 존재합니다.", 0);
            else
                Debug.Log("근처 트랩");

            return false;
        }
        if (SceneVarScript.Instance.GetOptionByName(myname, "outsideFocus", SceneVarScript.Instance.trapOption) == "0")
        {
            if (player == NetworkMaster.player)
            {
                if (player.GetComponent<PlayerScript>().dir == true)
                {
                    if (GradiantPos.Instance.transform.position.x < mousePos.x)
                    {
                        if (player == NetworkMaster.player) NetworkMaster.Instance.SendGameMsgFunc("시야가 없습니다.", 0);
                        else
                            Debug.Log("시야 없음");
                        return false;
                    }
                }
                else
                {
                    if (GradiantPos.Instance.transform.position.x > mousePos.x)
                    {
                        if (player == NetworkMaster.player) NetworkMaster.Instance.SendGameMsgFunc("시야가 없습니다.", 0);
                        else
                            Debug.Log("시야 없음");
                        return false;
                    }
                }
            }
            else
            {
                //AI 소유 유닛중 AI플레이어와 가장 먼거리에 있는 유닛사이의 거리가 아니라면 시야가 없어서 설치 불가.
            }
        }
        if (player == NetworkMaster.player)
        {
            if (!MainGameManager.mainGameManager.SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(myname, "cost", SceneVarScript.Instance.monsterOption))))
            {
                return false;
            }
        }
        else
        {
            if (!AIManager.Instance.SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(myname, "cost", SceneVarScript.Instance.monsterOption))))
            {
                return false;
            }
        }
        CreatMonster(myname, 1, mousePos.x, layer, player);
        return true;
    }
    public void MakePlayer(bool dir)
    {
            Debug.LogFormat("해당 씬에 플레이어 생성합니다. {0}", SceneManagerHelper.ActiveSceneName);

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            float playerpos = background.GetComponent<SpriteRenderer>().bounds.size.x / 2 * 0.85f;
            Vector3 setCameraPosition = new Vector3(dir ? -playerpos : playerpos, 0f, Camera.main.transform.position.z);

        if (player == null)
        {
            Camera.main.GetComponent<CameraScript>().SetCameraMove(setCameraPosition);
            player = PhotonNetwork.Instantiate("Player", new Vector3(dir ? -playerpos : playerpos, downSetPos.y, 0f), Quaternion.identity, 0);
            player.GetComponent<PlayerScript>().dir = dir;
            SetCreatureInfo(player, "Player1",player);
            player.layer = LayerMask.NameToLayer("centerunit");
            player.GetComponent<PlayerScript>().myplayer = true;
            this.dir = dir;
        }
        else
        {
            otherPlayer = PhotonNetwork.Instantiate("Player", new Vector3(dir ? -playerpos : playerpos, downSetPos.y, 0f), Quaternion.identity, 0);
            otherPlayer.GetComponent<PlayerScript>().dir = dir;
            SetCreatureInfo(otherPlayer, "Player1",otherPlayer);
            otherPlayer.layer = LayerMask.NameToLayer("centerunit");
            otherPlayer.GetComponent<PlayerScript>().myplayer = false;
        }
    }
    public string GetMode()
    {
        return (string)gameMode["Mode"];
    }
    public void SetMode(string mode)
    {
        gameMode["Mode"]=mode;
    }
    public void SendGameMsgFunc(string s, int type=0)
    {
        if (type == 0)
        {
            //개인 메세지
            textExpress.setNewText(s);
        }
        else if (type == 1)
        {
            //공통 메세지
            pv.RPC("SendGameMsg", RpcTarget.All, s);
        }
    }
    public void SendGameMsgFunc(string s,Color color, int type)
    {
        if (type == 0)
        {
            //개인 메세지
            textExpress.setNewText(s,color);
        }
        else if (type == 1)
        {
            //공통 메세지
            pv.RPC("SendGameMsg", RpcTarget.All, s);
        }
    }
    public void SendMainMsgFunc(string s, int type)
    {
        if (type == 0)
        {
            //개인 메세지
            textExpress.setMainText(s);
        }
        else if (type == 1)
        {
            //공통 메세지
            pv.RPC("SendMainMsg", RpcTarget.All, s);
        }
    }
    //위아래 층 정하는 함수
    public void SetLayer(int layernum)
    {
        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MainGameManager.mainGameManager.CountMoney(50000);
            }
        }
        setLayer = layernum;
    }
    public int GetLayer()
    {
        return setLayer;
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
    IEnumerator EndAction(string result,float playTime)
    {
        endPoint = 2;

        if (playTime >= SceneVarScript.REWARD_TIME)
        {
            playTime = SceneVarScript.REWARD_TIME;
        }
        if (setRatingState == 0)
        {
            //초기 감소값 복구
            SceneVarScript.Instance.SetRating(SceneVarScript.START_RATING_DISCOUNT);
            SceneVarScript.Instance.SetWinLose(-1, "lose");
            EndingMsgBox.SetActive(true);
            EndingMsg.text = "네트워크 오류로 인해 비겼습니다.";
            yield break;
        }
        if (result == "Win")
        {
            endState = 1;
            SceneVarScript.Instance.SetRating(SceneVarScript.START_RATING_DISCOUNT +SceneVarScript.Instance.GetWinPoint());
            SceneVarScript.Instance.SetWinLose(-1, "lose");
            SceneVarScript.Instance.SetWinLose(1, "win");
            int getMoney = (int)MainGameManager.mainGameManager.allMoney;
            if (getMoney * 0.07 >= SceneVarScript.MAX_REWARD)
            {
                getMoney = SceneVarScript.MAX_REWARD +(int)(SceneVarScript.WIN_REWARD*(playTime/SceneVarScript.REWARD_TIME));
            }
            else
            {
                getMoney =(int)(getMoney*0.07) + (int)(SceneVarScript.WIN_REWARD * (playTime / SceneVarScript.REWARD_TIME));
            }
            SceneVarScript.Instance.AddGold(getMoney);
            yield return new WaitForSeconds(3);
            EndingMsgBox.SetActive(true);
            EndingMsg.text = "승리하셨습니다!";
        }
        else if (result == "Lose")
        {
            endState = 2;
            int getMoney = (int)MainGameManager.mainGameManager.allMoney;
            if (getMoney * 0.03 >= SceneVarScript.MAX_REWARD)
            {
                getMoney = SceneVarScript.MAX_REWARD + (int)(SceneVarScript.LOSE_REWARD * (playTime / SceneVarScript.REWARD_TIME));
            }
            else
            {
                getMoney = (int)(getMoney * 0.03) + (int)(SceneVarScript.LOSE_REWARD * (playTime / SceneVarScript.REWARD_TIME));
            }
            SceneVarScript.Instance.AddGold(getMoney);
            yield return new WaitForSeconds(3);
            EndingMsgBox.SetActive(true);
            EndingMsg.text = "패배하셨습니다.";
        }
    }
    #endregion
    [PunRPC]
    public void SendGameMsg(string s)
    {
        textExpress.setNewText(s);
    }
    [PunRPC]
    public void SendMainMsg(string s)
    {
        textExpress.setMainText(s);
    }
    [PunRPC]
    public void SetStartRating()
    {
        SceneVarScript.Instance.SetWinLose(1, "lose");
        SceneVarScript.Instance.SetRating(-SceneVarScript.START_RATING_DISCOUNT);
        setRatingState = 1;
    }
    [PunRPC]
    public void Win(bool Winner,float playTime)
    {
     
        if (Winner == dir)
        {
            pv.RPC("CameraMove", RpcTarget.All,otherPlayer.transform.position );
            StartCoroutine(EndAction("Win",playTime));
        }
        else
        {
            pv.RPC("CameraMove", RpcTarget.All, player.transform.position);
            StartCoroutine(EndAction("Lose", playTime));
        }
    }
    [PunRPC]
    public void SetStage(int n)
    {
        MainGameManager.mainGameManager.PlaySoundTimeChange();
        gameStage = n;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(0, upSetPos.y), new Vector3(10, 0.1f, 0));
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector2(0, downSetPos.y), new Vector3(10, 0.1f, 0));
    }

}
