using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
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
		}
	}
	public static SceneVarScript Instance;
	public IDictionary[] monsterOption;
	public IDictionary userInfo;
	public bool isDataConnect;
	public string authCode;

	public bool isUserGetting;
    private void Awake()
    {
		Instance = this;
		DontDestroyOnLoad(gameObject);
		//StartCoroutine(ReRequst());
	}
    // Start is called before the first frame update
    void Start()
    {
		RequestMonsterDB();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	IEnumerator ReRequst()
	{
		for (; ; )
		{
			RequestMonsterDB();
			yield return new WaitForSeconds(10f);
		}
	}

	public void LoginCourseFun(string authCode)
    {
        if (string.IsNullOrEmpty(authCode))
        {
			Debug.Log("AuthCode 할당이 올바르지 않습니다. Network를 확인하고 재접속 해주세요.");
			return;
        }
		userInfo = null;
		StartCoroutine(LoginCourse(authCode));
	}
	IEnumerator LoginCourse(string authCode)
    {
		while (true)
		{
			//1.해당 코드로 유저 정보를 불러오기
			if (isUserGetting == false)
			{
				isUserGetting = true;
				StartCoroutine(GetUserByAuthCode(authCode));
			}
			yield return new WaitUntil(() => isUserGetting == false);
			//2.유저정보가 없다면 회원가입 실행
			if (userInfo == null)
			{
				Debug.Log("NoUser, CreateUser");
				CreateUser(authCode);
			}
			else
			{
				Debug.Log("User:" + userInfo["username"]);
				break;
			}
		}
		yield return null;
	}
	public void CreateUser(string authCode)
	{
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		User user = new User("민준", authCode);
		string json = JsonUtility.ToJson(user);
		reference.Child("users").Child(authCode).SetRawJsonValueAsync(json);
		Debug.Log("회원가입 성공");
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
		isUserGetting = true; // 코루티 종료 알리미
		bool loadEnd = false; // 비동기 함수 종료 알리미
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		reference.Child("users").Child(authCode).GetValueAsync().ContinueWith(task =>
		{
			loadEnd = true;
			if (task.IsFaulted)
			{
				Debug.Log("DB 연결 실패, Network 확인바람");
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
				userInfo = (IDictionary)snapshot.Value;
				// Do something with snapshot...
			}
		});
		yield return new WaitUntil(() => loadEnd == true);
		Debug.Log("GetUserByAuthCode, End");
		isUserGetting = false;
		yield return null;
	}
	public void RequestMonsterDB()
    {
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		FirebaseDatabase.DefaultInstance
		.GetReference("MonsterDB")
		.GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted)
			{
				isDataConnect = false;
				Debug.Log("DB 연결 실패");
				// Handle the error...
			}
			else if (task.IsCompleted)
			{
				int index = 0;
				isDataConnect = true;
				Debug.Log("찾았어요!!");
				DataSnapshot snapshot = task.Result;
				monsterOption = new IDictionary[snapshot.ChildrenCount];
				foreach (DataSnapshot data in snapshot.Children)
				{
					//받은 데이터들을 하나씩 잘라 string 배열에 저장
					monsterOption[index] = (IDictionary)data.Value;
					Debug.Log(monsterOption[index]["name"]+":"+ monsterOption[index]["icon"]);
					index++;
				}
				// Do something with snapshot...
			}
		});
	}
	public void SetAuthCode(string s)
    {
		authCode = s;
    }
	public string GetAuthCode()
    {
		return authCode;
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
