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

public class AuthScript : MonoBehaviour
{
    public bool IsFirebaseReady;
    public bool IsSignInOnProgess;

    public Text logo,logo2,logo3;

    public GameObject tabToPlay;

    public InputField id;
    public InputField pw;
    public Button signinbtn;

    public static FirebaseApp firebaseApp;
    public static FirebaseAuth firebaseAuth;

    public static FirebaseUser user;

    public string authCode;
    private void Awake()
    {
        Screen.SetResolution(1920, 1080,true);
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //Screen.SetResolution(1280 ,1024, false);

    }
    // Start is called before the first frame update
    void Start()
    {
        authCode = null;
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false /* Don't force refresh */)
            .Build();

        PlayGamesPlatform.InitializeInstance(config);

        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();


        //signinbtn.interactable = false;
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
                logo.text = "성공!";
                authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                tabToPlay.SetActive(true);
                logo3.text = authCode;
                //signinbtn.interactable = IsFirebaseReady;
            }
            else
            {
                if (Application.isEditor)
                {
                    tabToPlay.SetActive(true);
                }
                logo.text = "실패!";
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
                        logo2.text = "파이어 실패1";
                        Debug.LogError("SignInWithCredentialAsync was canceled.");
                        signinbtn.interactable = IsFirebaseReady;
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        logo2.text = "파이어 실패2";
                        Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        signinbtn.interactable = IsFirebaseReady;
                        return;
                    }
                        Firebase.Auth.FirebaseUser newUser = task.Result;
                        logo2.text = "파이어베이스 성공" + newUser.DisplayName + newUser.UserId;
                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
                    user = newUser;
                    });
        }
        }
