using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using UnityEngine;
public class SceneVarScript : MonoBehaviour
{
	public class User
	{
		public string username;
		public string authCode;
		public string money;
		public string win;
		public string lose;
		public string tear;
		public string skill1,skill2,skill3;
		public User()
		{
		}

		public User(string username, string authCode)
		{
			this.username = username;
			this.authCode = authCode;
			this.money = "0";
			this.win = "0";
			this.lose = "0";
			this.tear = "none";
			this.skill1 = skill2=skill3="-1";
		}
	}
	private FirebaseUser myUser;
	public static SceneVarScript Instance;
	public IDictionary[] monsterOption,trapOption,bossOption,playerOption,skillOption;
	public IDictionary userInfo;
	public bool isDataConnect,monsterDBConnecting,trapDBConnecting,bossDBConnecting,playerDBConnecting,skillDBConnecting;
	public string authCode;

	public bool tryConnect;
	public bool isUserGetting;

	public static int MAX_SKILL_COUNT = 3; //�������� ��ų��
	[Header("skill Icon")]
	public Sprite[] skillIcon;
	private void Awake()
    {
		Instance = this;
		DontDestroyOnLoad(gameObject);
		//StartCoroutine(ReRequst());
	}
    // Start is called before the first frame update
    void Start()
    {
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
		Debug.Log("������ ���� �մϴ�.");
		Debug.Log(args.Snapshot.Key+"/"+ args.Snapshot.Value);
		SetUserDataByName(args.Snapshot.Key,args.Snapshot.Value.ToString());
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
	public void SetUserDataByName(string myName,string data)
    {
		userInfo[myName] = data;
    }
	public void SetUserData(DataSnapshot snapshot)
	{
		foreach (var item in snapshot.Children)
		{
			Debug.Log(item.Key+":" + item.Value);
		}
		userInfo = (IDictionary)snapshot.Value;
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
        if (Input.GetKeyDown(KeyCode.A))
        {
			test();
        }
        if(monsterDBConnecting && trapDBConnecting && playerDBConnecting && bossDBConnecting)
        {
			isDataConnect = true;
        }
        else
        {
			Debug.Log("������ �ε� ����");
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
		yield return new WaitForSeconds(3f);
		for (;isDataConnect==false ; )
		{
			RequestAllDB();
			yield return new WaitForSeconds(10f);
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
				CreateUser(authCode);
			}
			else
			{
				Debug.Log("���̾�̽� �����ͺ��̽��� ���� ���� ȹ��! User:" + userInfo["username"]);
				AddDataListner();
				break;
			}
		}
		yield break;
	}
	public void CreateUser(string authCode)
	{
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		User user = new User("����", authCode);
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
            }
		});
		Debug.Log("ȸ������ ����");
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
			loadEnd = true;
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
					return;
                }
				Debug.Log("GetUser Success, Set userData");
				SetUserData(snapshot);
				// Do something with snapshot...
			}
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
		.GetValueAsync().ContinueWith(task => {
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
	public void RequestAllDB()
    {
		RequestMonsterDB();
		RequestTrapDB();
		RequestBossDB();
		RequestPlayerDB();
		RequestSkillDB();
    }
	public void test()
    {
		Debug.Log("Test �Լ� ����");
    }
	public string GetOptionByIndex(string index, string colume,IDictionary[] db)
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
					if (option.Contains(colume)==false)
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
					if (option.Contains(colume)== false)
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
						word = GetOptionByIndex(dbList[1], dbList[2],trapOption);
					}else if (dbList[0] == "@playerDB")
					{
						word = GetOptionByIndex(dbList[1], dbList[2],playerOption);
					}else if (dbList[0] == "@bossDB")
					{
						word = GetOptionByIndex(dbList[1], dbList[2],bossOption);
					}else if (dbList[0] == "@monsterDB")
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
	public void RequestTrapDB()
	{
		trapDBConnecting = false;
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		FirebaseDatabase.DefaultInstance
		.GetReference("TrapDB")
		.GetValueAsync().ContinueWith(task => {
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
	public void RequestBossDB()
	{
		bossDBConnecting = false;
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		FirebaseDatabase.DefaultInstance
		.GetReference("BossDB")
		.GetValueAsync().ContinueWith(task => {
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
		.GetValueAsync().ContinueWith(task => {
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
		.GetValueAsync().ContinueWith(task => {
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
	public void SetSkillDB(int index,string skillName)
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
					Debug.Log("�̹� �����ϴ� ��ų�Դϴ�.");
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
			return userInfo[index].ToString();
        }
		return "null";
	}
}
