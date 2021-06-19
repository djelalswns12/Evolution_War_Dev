using Firebase;
using Firebase.Auth;

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using TMPro;
using System.Net;
using System.IO;
public class AuthScript : MonoBehaviour
{
    public bool IsFirebaseReady;
    public bool IsSignInOnProgess;

    public Text logo, logo2, logo3;

    public GameObject tabToPlay;

    public InputField id;
    public InputField pw;
    public Button signinbtn;

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;

    public static FirebaseUser user;

    public string authCode;
    public string Mycraw(string url)
    {
        // Create a request for the URL.
        WebRequest request = WebRequest.Create(url);
        // If required by the server, set the credentials.
        request.Credentials = CredentialCache.DefaultCredentials;

        // Get the response.
        WebResponse response = request.GetResponse();
        // Display the status.
        //Console.WriteLine(((HttpWebResponse)response).StatusDescription);

        // Get the stream containing content returned by the server.
        // The using block ensures the stream is automatically closed.
        using (Stream dataStream = response.GetResponseStream())
        {
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            response.Close();
            return responseFromServer;
        }

        // Close the response.

    }
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Screen.SetResolution(1280 ,1024, false);
         
    }
    // Start is called before the first frame update
    void Start()
    {
       
        //string monsterOption=Mycraw("https://waroforigin-default-rtdb.firebaseio.com/Monster.json");

        
        authCode = null;
        
        //플레이게임 실행코드
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false /* Don't force refresh */)
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();


        //signinbtn.interactable = false;
        //파이어베이스 준비코드
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var result = task.Result;
            if (result != DependencyStatus.Available)
            {
                Debug.Log(result.ToString());
            }
            else
            {
                IsFirebaseReady = true;
                firebaseApp = FirebaseApp.DefaultInstance;
                firebaseAuth = FirebaseAuth.DefaultInstance;
            }
        });
        googleLogin();
    }

    // Update is called once per frame
    void Update()
    {
        logo3.text = SceneVarScript.Instance.GetAuthCode();
        if (user != null)
        {
            SceneManager.LoadScene("LobbyScean"); 
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (authCode != null)
            {
                GoogleToFireBase();
            }else if (Application.isEditor)
            {
                SceneManager.LoadScene("LobbyScean");
            }
        }
        if (authCode != null)
        {
            tabToPlay.SetActive(true);
            Debug.Log("구글플레이 회원id를 받아왔습니다. 이값을 이제 파이어베이스로 넘기세요!");
        }
    }

    public void SignIn()
    {
        if(IsFirebaseReady==false || IsSignInOnProgess==true || user != null)
        {
            return;
        }
        IsSignInOnProgess = true;
        signinbtn.interactable = false;


        string myid = id.text=="" ? "djelalswns12@naver.com" : id.text;
        string mypw = pw.text=="" ? "alswns12" : pw.text;
        firebaseAuth.SignInWithEmailAndPasswordAsync(myid,mypw).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("로그인 실패 이유:"+task.Exception);//로그인 실패시
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("로그인 중 취소됨");
            }
            else
            {
                user = task.Result;
                IsSignInOnProgess = false;
                signinbtn.interactable = true;
                Debug.Log("로그인 성공");
            }

        });

    }
    public void CreateUser()
    {
        firebaseAuth.CreateUserWithEmailAndPasswordAsync(id.text, pw.text).ContinueWith(task =>
         {
             if (task.IsCanceled)
             {
                 Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                 return;
             }
             if (task.IsFaulted)
             {
                 //이미 있는 계정 또는 형식이 맞지 않음
                 Debug.LogError("사용중인 이메일 또는 형식이 맞지 않음" + task.Exception);
                 return;
             }
             //Firebase.Auth.FirebaseUser newUser = task.Result;
             Debug.Log("회원가입 성공");
         });
     }


    public void googleLogin()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                logo.text = "구글플레이게임 로그인 및 활성 성공!";
                authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                //signinbtn.interactable = IsFirebaseReady;
            }
            else
            {
                if (Application.isEditor)
                {
                    SceneVarScript.Instance.SetAuthCode("test");
                    tabToPlay.SetActive(true);
                }

                logo3.text = SceneVarScript.Instance.GetAuthCode();
                logo.text = "구글플레이게임 로그인 및 활성 실패!";
            }

        });
    }
    public void googleLogOut()
    {
        if(tabToPlay.activeSelf)
        tabToPlay.SetActive(false);
        else
            tabToPlay.SetActive(true);
    }
    public void getAuth()
    {
        authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
        logo3.text = authCode;
    }
    public void GoogleToFireBase()
            {
            signinbtn.interactable = false;
            firebaseAuth = Firebase.Auth.FirebaseAuth.DefaultInstance;
                Firebase.Auth.Credential credential =Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
                firebaseAuth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        tabToPlay.GetComponent<Text>().text = "Login Error01, Please restart or Check your Network";
                        Debug.LogError("SignInWithCredentialAsync was canceled.");
                        signinbtn.interactable = IsFirebaseReady;
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        tabToPlay.GetComponent<Text>().text = "Login Error02, Please restart or Check your Network";
                        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        signinbtn.interactable = IsFirebaseReady;
                        return;
                    }
                        Firebase.Auth.FirebaseUser newUser = task.Result;
                        logo2.text = "파이어베이스 성공" + newUser.DisplayName + newUser.UserId;
                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                    user = newUser;
                    SceneVarScript.Instance.SetAuthCode(user.UserId);
                });
        }
        }
