using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
public class SceneVarScript : MonoBehaviour
{
    public static readonly string gameVersion = "v4";
    public class User
    {
        public string username;
        public string authCode;
        public string money;
        public int win;
        public int lose;
        public int rating;
        public string skill1, skill2, skill3;
        public Dictionary<string, object> skill;
        public User()
        {
        }

        public User(string username, string authCode)
        {
        
            this.username = username;
            this.authCode = authCode;
            this.money = "0";
            this.win = 0;
            this.lose = 0;
            this.rating = 1000;
            this.skill1 = skill2 = skill3 = "-1";
        }
    }
    private FirebaseUser myUser;
    public static SceneVarScript Instance;
    public IDictionary[] monsterOption, trapOption, bossOption, playerOption, skillOption,usersOption,noticeOption,skillShopOption;
    public IDictionary userInfo;
    public bool isVersionCheck,isDataConnect, monsterDBConnecting, trapDBConnecting, bossDBConnecting, playerDBConnecting, skillDBConnecting,usersDBConnecting,noticeDBConnecting,skillShopDBConnecting;
    public string authCode;

    public bool tryConnect;
    public bool isUserGetting;
    public bool isUserCreating;
    public bool SetWinLoseGoldProcessing;
    public bool hasUser = false;
    public bool noticeReset=false;
    public bool usersReset=false;

    public int tmpRating;
    public static int MAX_SKILL_COUNT = 3; //�������� ��ų��
    public static int START_RATING_DISCOUNT = 10; //�������� ��ų��
    public static int MAX_REWARD= 4500; //��� ȹ�濡 ���� ���󰡴� ��
    public static int WIN_REWARD= 2500; //�¸��� �ִ� ���
    public static int LOSE_REWARD= 1000; //�й�� �ִ� ���
    public static int REWARD_TIME= 300; //�ִ� ������ ���� �÷��̽ð�
    public static int AD_MIN_MONEY = 500;
    public static int AD_MAX_MONEY = 2000;
    public static int AD_COOL = 180;

    [Header("skill Icon")]
    public Sprite[] skillIcon;
    public Sprite noneSkillIcon;
    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
        //StartCoroutine(ReRequst());
    }
    // Start is called before the first frame update
    void Start()
    {
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        RequestAllDB();

        //reference.ValueChanged += HandleValueChanged;
        //reference.ChildAdded += HandleChildAdded;
        //reference.ChildRemoved += HandleChildRemoved;
        //reference.ChildMoved += HandleChildMoved;
    }
    public void AddDataListner()
    {
        Debug.Log("������ �߰�!!");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("users").Child(GetAuthCode());
        reference.ChildChanged += HandleChildChanged;
        
    }
    
    public void AddNoticeListner()
    {
        Debug.Log("�������� ������ �߰�!!");
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference("NoticeDB");
        reference.ChildAdded += HandleNoticeChildChanged;
        reference.ChildChanged += HandleNoticeChildChanged;
        reference.ChildRemoved += HandleNoticeChildChanged;
    }
    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("change1");
        //SnapshotAllDataRead(args.Snapshot);
    }

    //users �������� ��������� ���� �߰��Ǹ� ����
    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("change2");
        //SnapshotDataRead(args.Snapshot);
    }


    //users �������� ��������� ���� ����Ǹ� ����
    void HandleChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log(args.Snapshot.Key + "/" + args.Snapshot.Value+"//"+ args.Snapshot.ChildrenCount);
        RequestusersDB();//��ŷ ������ ����
        StartCoroutine(SetUserDataByName(args.Snapshot.Key , args.Snapshot.Value));
    }
    void HandleNoticeChildChanged(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("���������� ���� �մϴ�.");
        RequestNoticeDB();
    }
    //users �������� ��������� ���� ���ŵǸ� ����
    void HandleChildRemoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("change4");
        //Set(args.Snapshot);
    }

    //users �������� ��������� ���� �̵��ϸ� ����
    void HandleChildMoved(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Debug.Log("change5");
        //SnapshotDataRead(args.Snapshot);
    }
    IEnumerator SetUserDataByName(string myName, object data)
    {
        yield return new WaitUntil(() => isUserGetting == false);
        yield return new WaitUntil(() => SetWinLoseGoldProcessing == false);
        userInfo[myName] = data;
        Debug.Log(myName + "���� �Ϸ�");
    }
    public void SetUserData(DataSnapshot snapshot)
    {
        
        if (Application.isEditor)
        {
            foreach (var item in snapshot.Children)
            {
                Debug.Log(item.Key + ":" + item.Value);
            }
        }
        userInfo = (IDictionary)snapshot.Value;

        hasUser = true;
    }

    public void SnapshotAllDataRead(DataSnapshot Snapshot)
    {
        return;
        foreach (var item in Snapshot.Children)
        {
            Debug.Log(item.Child("skill1").Value + " / " + item.Child("skill2").Value + " / " + item.Child("skill3").Value);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Application.isEditor|| GetUserOption("Master")=="1")
        {
            if (SceneManager.GetActiveScene().name == "GameScean" && Input.GetKeyDown(KeyCode.A))
            {
                test();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                BlindManager.Instance.OnBlind();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                SetNewSkill(UnityEngine.Random.Range(0, 5));
                BlindManager.Instance.CloseBlind();
            }
        }
        if (isVersionCheck&&monsterDBConnecting && trapDBConnecting && playerDBConnecting && bossDBConnecting && skillDBConnecting&& noticeDBConnecting)
        {
            //usersDBConnecting�� ����
            isDataConnect = true;
      
        }
        else
        {
            //Debug.Log("������ �ε� ����");
            if (tryConnect == false)
            {
                tryConnect = true;
                StartCoroutine(ReRequst());
            }
            isDataConnect = false;
        }
    }
    IEnumerator ReRequst()
    {
        yield return new WaitForSeconds(2f);
        for (; isDataConnect == false;)
        {
            RequestAllDB();
            yield return new WaitForSeconds(3f);
        }
    }

    public void LoginCourseFun(string authCode)
    {
        if (string.IsNullOrEmpty(authCode))
        {
            Debug.Log("AuthCode �Ҵ��� �ùٸ��� �ʽ��ϴ�. Network�� Ȯ���ϰ� ������ ���ּ���.");
            return;
        }
        userInfo = null;
        StartCoroutine(LoginCourse(authCode));
    }
    IEnumerator LoginCourse(string authCode)
    {
        while (true)
        {
            //1.�ش� �ڵ�� ���� ������ �ҷ�����
            if (isUserGetting == false)
            {
                isUserGetting = true;
                StartCoroutine(GetUserByAuthCode(authCode));
            }
            yield return new WaitUntil(() => isUserGetting == false);
            //2.���������� ���ٸ� ȸ������ ����
            if (userInfo == null)
            {
                Debug.Log("NoUser, CreateUser");
                isUserCreating = true;
                StartManager.Instance.PopUpNaming();
                yield return new WaitUntil(() => isUserCreating == false);
            }
            else
            {
                //AuthScript.Instance.tabToPlay.SetActive(true);
                Debug.Log("���̾�̽� �����ͺ��̽��� ���� ���� ȹ��! User:" + userInfo["username"]);
                AddNoticeListner();
                AddDataListner();
                break;
            }
        }
        yield break;
    }
    public IEnumerator CreateUser(string name)
    {
        bool isSuccess = false;
        bool isEnd = false;
        bool isChecked = true;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("users ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot data in snapshot.Children)
                {
                    string loadName = (string)(data.Child("username").Value);
                    if (name.Equals(loadName))
                    {
                        isChecked = false;
                        isEnd = true;
                        break;
                    }
                }
                if (isChecked == true)
                {
                    User user = new User(name, authCode);
                    string json = JsonUtility.ToJson(user);
                    reference.Child("users").Child(authCode).SetRawJsonValueAsync(json).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Debug.Log("���� ���� ����");
                        }
                        if (task.IsCompleted)
                        {
                            Debug.Log("���� �Ϸ�");
                            SetNewSkill(0);
                            isSuccess = true;

                        }
                        isEnd = true;
                    });
                }
                // Do something with snapshot...
            }
        });
        yield return new WaitUntil(() => isEnd == true);
        isUserCreating = false;
        if (isSuccess == true)
        {
            StartManager.Instance.PopUpCloseNaming();
        }
        else
        {
            StartManager.Instance.OverLapNickName();
        }
        Debug.Log("ȸ������ ����");
    }
    public void BuySkillFunc(string shopIndex)
    {
        StartCoroutine(BuySkill(shopIndex));
    }
    private IEnumerator BuySkill(string shopIndex)
    {
        yield return new WaitUntil(() => isUserGetting == false);
        GetUserByAuthCodeFun(GetAuthCode());
        yield return new WaitUntil(() => isUserGetting == false);
        string skillIndex = GetOptionByIndex(shopIndex, "skillIndex", skillShopOption);
        //�̹� �����ϴ� ��ų���� Ȯ��
        var inventory=GetUserOption("Inventory").Split('/');
        for(int i = 0; i < inventory.Length - 1; i++)
        {
            if (inventory[i] == skillIndex)
            {
                lobbymanager.Instance.SetLobbyMsg("�̹� �������� ��ų�Դϴ�.");
                yield break;
            }
        }

        //����� �� ���� Ȯ��
        if (int.Parse(GetOptionByIndex(shopIndex, "cost", skillShopOption)) <= int.Parse(GetUserOption("money")))
        {
            
            if (skillIndex == "null")
            {
                Debug.Log(skillIndex);
            }
            
            StartCoroutine(SetWinLoseGoldByAsync(-int.Parse(GetOptionByIndex(shopIndex, "cost", skillShopOption)), "money"));
            yield return new WaitUntil(() => isUserGetting == false);
            SetNewSkill(int.Parse(skillIndex));
            LobbySoundManager.Instance.CoinSoundPlay();
        }
        else
        {
            lobbymanager.Instance.SetLobbyMsg("��尡 �����մϴ�.");
            yield break;
        }

    }
    public void GetUserByAuthCodeFun(string authCode)
    {
        if (isUserGetting == false)
        {
            isUserGetting = true;
            StartCoroutine(GetUserByAuthCode(authCode));
        }
    }
    IEnumerator GetUserByAuthCode(string authCode)
    {
        isUserGetting = true; // �ڷ�Ƽ ���� �˸���
        bool loadEnd = false; // DBȣ���� ���� �񵿱� �Լ� ���� �˸���
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("users").Child(authCode).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("DB ���� ����, Network Ȯ�ιٶ�");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null)
                {
                    Debug.Log("GetUser Faild , No Data");
                }
                else
                {
                    Debug.Log("GetUser Success, Set userData");
                    SetUserData(snapshot);
                }
                // Do something with snapshot...
            }
            loadEnd = true;
        });
        yield return new WaitUntil(() => loadEnd == true);
        Debug.Log("GetUserByAuthCode, End");
        isUserGetting = false;
        yield break; ;
    }
    public void RequestMonsterDB()
    {
        monsterDBConnecting = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("MonsterDB")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                monsterDBConnecting = false;
                Debug.Log("MonsterDB ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int index = 0;
                monsterDBConnecting = true;
                Debug.Log("ã�Ҿ��!!");
                DataSnapshot snapshot = task.Result;
                monsterOption = new IDictionary[snapshot.ChildrenCount];
                foreach (DataSnapshot data in snapshot.Children)
                {
                    //���� �����͵��� �ϳ��� �߶� string �迭�� ����
                    monsterOption[index] = (IDictionary)data.Value;
                    //Debug.Log(monsterOption[index]["name"]+":"+ monsterOption[index]["icon"]);
                    index++;
                }
                // Do something with snapshot...
            }
        });
    }
    public void RequestNoticeDB()
    {
        noticeDBConnecting = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("NoticeDB").OrderByChild("priority")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                noticeDBConnecting = false;
                Debug.Log("NoticeDB ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int index = 0;
                noticeDBConnecting = true;
                Debug.Log("ã�Ҿ��!!");
                DataSnapshot snapshot = task.Result;
                noticeOption = new IDictionary[snapshot.ChildrenCount];
                foreach (DataSnapshot data in snapshot.Children)
                {
                    //���� �����͵��� �ϳ��� �߶� string �迭�� ����
                    noticeOption[index] = (IDictionary)data.Value;
                    //Debug.Log(noticeOption[index]["thema"]);
                    //Debug.Log(monsterOption[index]["name"]+":"+ monsterOption[index]["icon"]);
                    index++;
                }
                noticeReset = false;
               
                // Do something with snapshot...
            }
        });
    }
    public void RequestSkillShopDB()
    {
        skillShopDBConnecting = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("ShopDB").Child("SkillShop")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                skillShopDBConnecting = false;
                Debug.Log("skillShopDB ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int index = 0;
                skillShopDBConnecting = true;
                Debug.Log("ã�Ҿ��!!");
                DataSnapshot snapshot = task.Result;
                skillShopOption = new IDictionary[snapshot.ChildrenCount];
                foreach (DataSnapshot data in snapshot.Children)
                {
                    //���� �����͵��� �ϳ��� �߶� string �迭�� ����
                    skillShopOption[index] = (IDictionary)data.Value;
                    //Debug.Log("skill is:"+skillShopOption[index]["index"]);
                    index++;
                }
                // Do something with snapshot...
            }
        });
    }
    public void RequestAllDB()
    {
        usersDBConnecting = false;
        monsterDBConnecting = false;
        trapDBConnecting = false; 
        playerDBConnecting = false; 
        bossDBConnecting = false;
        skillDBConnecting = false;
        skillShopDBConnecting = false;
        isVersionCheck = false;
        RequestGameVersion();
        RequestMonsterDB();
        RequestTrapDB();
        RequestBossDB();
        RequestPlayerDB();
        RequestSkillDB();
        RequestusersDB();
        RequestNoticeDB();
        RequestSkillShopDB();
    }
    public void test()
    {
           
            MainGameManager.mainGameManager.AttackAlam();
            SpawnManager.Instance.spawnSpeed += 7;
            NetworkMaster.Instance.gameStage++;
            MainGameManager.mainGameManager.CountMoney(5000);
       
    }

    public string GetOptionByIndex(string index, string colume, IDictionary[] db)
    {
        if (db != null)
            foreach (IDictionary option in db)
            {
                if (option.Contains("index") == false)
                {
                    continue;
                }
                if (option["index"].ToString() == index)
                {
                    if (option.Contains(colume) == false)
                    {
                        break;
                    }
                    return option[colume].ToString();
                }
            }
        return "null";
    }
    public string GetOptionByName(string name, string colume, IDictionary[] db)
    {
        if (db != null)
            foreach (IDictionary option in db)
            {
                if (option.Contains("name") == false)
                {
                    continue;
                }
                if (option["name"].ToString() == name)
                {
                    if (option.Contains(colume) == false)
                    {
                        break;
                    }
                    return option[colume].ToString();
                }
            }
        return "null";
    }
    public string GetDBSource(string source)
    {
        string answer = "";
        var sourceList = source.Split('%');
        foreach (var str in sourceList)
        {
            string word = "";
            if (str.Length > 0)
            {
                if (str[0] == '@')
                {
                    var dbList = str.Split('.');
                    if (dbList[0] == "@trapDB")
                    {
                        word = GetOptionByIndex(dbList[1], dbList[2], trapOption);
                    }
                    else if (dbList[0] == "@playerDB")
                    {
                        word = GetOptionByIndex(dbList[1], dbList[2], playerOption);
                    }
                    else if (dbList[0] == "@bossDB")
                    {
                        word = GetOptionByIndex(dbList[1], dbList[2], bossOption);
                    }
                    else if (dbList[0] == "@monsterDB")
                    {
                        word = GetOptionByIndex(dbList[1], dbList[2], monsterOption);
                    }
                    else if (dbList[0] == "@skillDB")
                    {
                        word = GetOptionByIndex(dbList[1], dbList[2], skillOption);
                    }
                 
                }
                else
                {
                    word = str;
                }
            }
            answer += word;
        }
        return answer;
    }
    public void RequestGameVersion()
    {
        isVersionCheck = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("Version")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot data = task.Result;
                foreach(var item in data.Children)
                {
                    Debug.Log(item.Key);
                    if (item.Key.ToString() == gameVersion)
                    {
                        if ((bool)item.Value == true)
                        {
                            isVersionCheck = true;
                            return;
                        }
                        else
                        {
                            isVersionCheck = false;
                            return;
                        }
                    }
                }
            }
            if (task.IsFaulted)
            {

            }
        });
        }
    public void RequestTrapDB()
    {
        trapDBConnecting = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("TrapDB")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                trapDBConnecting = false;
                Debug.Log("TrapDB ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int index = 0;
                trapDBConnecting = true;
                Debug.Log("ã�Ҿ��!!");
                DataSnapshot snapshot = task.Result;
                trapOption = new IDictionary[snapshot.ChildrenCount];
                foreach (DataSnapshot data in snapshot.Children)
                {
                    //���� �����͵��� �ϳ��� �߶� string �迭�� ����
                    trapOption[index] = (IDictionary)data.Value;
                    //Debug.Log(monsterOption[index]["name"]+":"+ monsterOption[index]["icon"]);
                    index++;
                }
                // Do something with snapshot...
            }
        });
    }
    public void RequestusersDB()
    {
        usersDBConnecting = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("users").OrderByChild("rating")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                usersDBConnecting = false;
                Debug.Log("usersDB ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int index = 0;
                usersDBConnecting = true;
                Debug.Log("usersã�Ҿ��!!");
                DataSnapshot snapshot = task.Result;
                usersOption = new IDictionary[snapshot.ChildrenCount];
                foreach (DataSnapshot data in snapshot.Children)
                {
                    //���� �����͵��� �ϳ��� �߶� string �迭�� ����
                    usersOption[index] = (IDictionary)data.Value;
                    //Debug.Log(index+"�� :"+usersOption[index]["username"]+"/"+ usersOption[index]["rating"]);
                    index++;
                }
                usersReset = false;
                // Do something with snapshot...
            }
        });
    }
    public void RequestBossDB()
    {
        bossDBConnecting = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("BossDB")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                bossDBConnecting = false;
                Debug.Log("TrapDB ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int index = 0;
                bossDBConnecting = true;
                Debug.Log("ã�Ҿ��!!");
                DataSnapshot snapshot = task.Result;
                bossOption = new IDictionary[snapshot.ChildrenCount];
                foreach (DataSnapshot data in snapshot.Children)
                {
                    //���� �����͵��� �ϳ��� �߶� string �迭�� ����
                    bossOption[index] = (IDictionary)data.Value;
                    //Debug.Log(monsterOption[index]["name"]+":"+ monsterOption[index]["icon"]);
                    index++;
                }
                // Do something with snapshot...
            }
        });
    }
    public void RequestPlayerDB()
    {
        playerDBConnecting = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("PlayerDB")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                playerDBConnecting = false;
                Debug.Log("TrapDB ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int index = 0;
                playerDBConnecting = true;
                Debug.Log("ã�Ҿ��!!");
                DataSnapshot snapshot = task.Result;
                playerOption = new IDictionary[snapshot.ChildrenCount];
                foreach (DataSnapshot data in snapshot.Children)
                {
                    //���� �����͵��� �ϳ��� �߶� string �迭�� ����
                    playerOption[index] = (IDictionary)data.Value;
                    //Debug.Log(monsterOption[index]["name"]+":"+ monsterOption[index]["icon"]);
                    index++;
                }
                // Do something with snapshot...
            }
        });
    }
    public void RequestSkillDB()
    {
        skillDBConnecting = false;
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        FirebaseDatabase.DefaultInstance
        .GetReference("SkillDB")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                skillDBConnecting = false;
                Debug.Log("SkillDB ���� ����");
                // Handle the error...
            }
            else if (task.IsCompleted)
            {
                int index = 0;
                skillDBConnecting = true;
                Debug.Log("ã�Ҿ��!!");
                DataSnapshot snapshot = task.Result;
                skillOption = new IDictionary[snapshot.ChildrenCount];
                foreach (DataSnapshot data in snapshot.Children)
                {
                    //���� �����͵��� �ϳ��� �߶� string �迭�� ����
                    skillOption[index] = (IDictionary)data.Value;
                    //Debug.Log(monsterOption[index]["name"]+":"+ monsterOption[index]["icon"]);
                    index++;
                }
                // Do something with snapshot...
            }
        });
    }
    public void SetEnterLog()
    {
        //DateTime.Now.ToString(("yyyy-MM-dd")));
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.Child("Log").Child(DateTime.Now.ToString(("yyyy-MM-dd"))).Child(DateTime.Now.ToString((" HH��mm��tt"))).Child(GetUserOption("username")).SetValueAsync("����");
    }
    public void SetNewSkill(int index)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var key = reference.Child("users").Child(GetAuthCode()).Child("Inventory").Push().Key;
        reference.Child("users").Child(GetAuthCode()).Child("Inventory").Child(key).SetValueAsync(index);
    }
    public void SetSkillDB(int index, string skillName)
    {
        StartCoroutine(SetSkillByAsync(index, skillName));
    }
    IEnumerator SetSkillByAsync(int index, string skillName)
    {
        if (skillName != "-1")
        {
            Debug.Log("���� ���� ��û");
            GetUserByAuthCodeFun(GetAuthCode());
            yield return new WaitUntil(() => isUserGetting == false);
            Debug.Log("���� ���� ȹ�� �Ϸ�!!");
            for (int i = 0; i < MAX_SKILL_COUNT; i++)
            {
                if (GetUserOption("skill" + (i + 1)) == skillName)
                {
                    lobbymanager.Instance.SetLobbyMsg("�̹� ������ ��ų�Դϴ�.");
                    yield break;
                }
            }
        }
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var key = GetAuthCode();
        reference.Child("users").Child(key).Child("skill" + index).SetValueAsync(skillName)
        .ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("��ų���� ����, ��Ʈ��ũ �����Դϴ�.");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log("��ų���� ����");
            }
        });
    }
    public void SetRating(int point)
    {
        StartCoroutine(SetRatingByAsync(point));
    }
    IEnumerator SetRatingByAsync(int point)
    {
        yield return new WaitUntil(() => isUserGetting == false);
        Debug.Log("���� ���� ��û");
        GetUserByAuthCodeFun(GetAuthCode());
        yield return new WaitUntil(() => isUserGetting == false);
        Debug.Log("���� ���� ȹ�� �Ϸ�!!");
        int SetValue = point + int.Parse(GetUserOption("rating"));
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var key = GetAuthCode();
        reference.Child("users").Child(key).Child("rating").SetValueAsync(SetValue)
        .ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("���� ���� ����, ��Ʈ��ũ �����Դϴ�.");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log("���� ���� ����.");
            }
        });
    }
    public void SetWinLose(int point, string type)
    {
        StartCoroutine(SetWinLoseGoldByAsync(point, type));
    }
    public void AddGold(int point)
    {
        Debug.Log("��弳��"+point);
        MainGameManager.mainGameManager.getRewardMoney = point.ToString();
        StartCoroutine(SetWinLoseGoldByAsync(point, "money"));
    }
    IEnumerator SetWinLoseGoldByAsync(int point, string type)
    {
        yield return new WaitUntil(() => isUserGetting == false);
        SetWinLoseGoldProcessing = true;
        Debug.Log("���� ���� ��û");
        GetUserByAuthCodeFun(GetAuthCode());
        yield return new WaitUntil(() => isUserGetting == false);
        Debug.Log("���� ���� ȹ�� �Ϸ�!!");
        int SetValue = point + int.Parse(GetUserOption(type));
        if (SetValue <= 0)
        {
            SetValue = 0;
        }
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        var key = GetAuthCode();
        reference.Child("users").Child(key).Child(type).SetValueAsync(SetValue)
        .ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log($"{type} ���� ����, ��Ʈ��ũ �����Դϴ�.");
                return;
            }
            if (task.IsCompleted)
            {
                Debug.Log($"{type} ���� ����.");
            }
            SetWinLoseGoldProcessing = false;
        });
    }

    public string GetMonsterOption(string index0, string index1)
    {
        if (monsterOption != null)
            foreach (IDictionary option in monsterOption)
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
    public int GetWinPoint()
    {
        return START_RATING_DISCOUNT +2;
    }
    public void SetAuthCode(string s)
    {
        authCode = s;
    }
    public string GetAuthCode()
    {
        return authCode;
    }
    public void SetFirebaseUser(FirebaseUser user)
    {
        myUser = user;
    }
    public FirebaseUser GetFirebaseUser()
    {
        return myUser;
    }
    public string GetUserOption(string index)
    {
        if (userInfo != null)
        {
            if (userInfo.Contains(index))
            {
                if (index != "Inventory")
                {
                    return userInfo[index].ToString();
                }
                else
                {
                    string skills="";
                    var data = (Dictionary<string, object>)userInfo["Inventory"];
                    foreach (var item in data)
                    {
                        skills += item.Value.ToString() + "/";
                    }
                    return skills;
                }
            }
            else
            {
                return "null";
            }
        }
        return "null";
    }
}
