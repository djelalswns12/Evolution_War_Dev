using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour,IUnityAdsListener
{
    public static AdsManager Instance;
    const string androidID = "4285053";
    const string bannerID = "EvolutionWarBanner";
    const string interstitialID = "Interstitial_Android";
    const string rewardedVideoID = "Rewarded_Android";
    public Button rewardBtn,interstitalBtn;
    public float playTime;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        playTime = SceneVarScript.AD_COOL;
        Advertisement.Initialize(androidID,Application.isEditor);
        Advertisement.IsReady(bannerID);
        //interstitalBtn.interactable= Advertisement.IsReady(interstitialID);
        //rewardBtn.interactable= Advertisement.IsReady(rewardedVideoID);
        Advertisement.AddListener(this);
        //rewardBtn.onClick.AddListener(ShowRewardedVideo);
        //interstitalBtn.onClick.AddListener(ShowInterstitial);
    }

    // Update is called once per frame
    void Update()
    {
        playTime += Time.deltaTime;
    }
    public void ShowInterstitial()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show(interstitialID);
        }
    }
    public void ShowRewardedVideo()
    {
        if (lobbymanager.Instance.isAds == false)
        {
            lobbymanager.Instance.SetLobbyMsg("매칭중에는 불가능합니다.");
            return;
        }
        if (!Advertisement.IsReady(rewardedVideoID))
        {
            lobbymanager.Instance.SetLobbyMsg("광고 할당중입니다. 잠시후 다시 시도해주세요.");
            return;
        }
        if (SceneVarScript.AD_COOL<=playTime)
        {
            Advertisement.Show(rewardedVideoID);
        }
        else
        {
            lobbymanager.Instance.SetLobbyMsg($"보상 획득까지 {(int)(SceneVarScript.AD_COOL-playTime)}초 남았습니다.");
        }
    }
    public void HideBanner()
    {
        Advertisement.Banner.Hide();
    }
    public void ShowBanner()
    {
        Advertisement.Banner.Show();
    }
    public void OnUnityAdsReady(string placementId)
    {
        if (placementId == rewardedVideoID)
        {
           // rewardBtn.interactable = true;
        }
        if (placementId == interstitialID)
        {
           // interstitalBtn.interactable = true;
        }
        if (placementId == bannerID)
        {
            Debug.Log("배너 온");
            Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_LEFT);
            Advertisement.Banner.Show(bannerID);
        }

    }

    public void OnUnityAdsDidError(string message)
    {
        //Show or log the error here
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        //Do this ads Start
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == rewardedVideoID)
        {
            if (showResult == ShowResult.Finished)
            {
                int money = Random.Range(SceneVarScript.AD_MIN_MONEY, SceneVarScript.AD_MAX_MONEY);
                SceneVarScript.Instance.SetWinLose(money, "money");
                lobbymanager.Instance.SetLobbyMsg($"{money}골드를 획득 하였습니다.");
                playTime = 0;
            }
            else if (showResult == ShowResult.Skipped)
            {
                lobbymanager.Instance.SetLobbyMsg("광고시청에 오류가 발생하였습니다. Error1");
                Debug.Log("광고 패스");
            }else if (showResult == ShowResult.Failed)
            {
                lobbymanager.Instance.SetLobbyMsg("광고시청에 오류가 발생하였습니다. Error2");
                Debug.Log("광고 실패");
            }
        }
    }
}
