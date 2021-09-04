using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBtnInShop : MonoBehaviour
{
    public string shopIndex;
    private string skillIndex;
    public Image icon;
    public Text cost;
    public Button buyBtn;
    public Button optionBtn;
    public string iconIndex;
    public bool dataComplete;
    // Start is called before the first frame update
    void Start()
    {
        buyBtn.onClick.AddListener(Buy);
        optionBtn.onClick.AddListener(Option);
    }

    // Update is called once per frame
    void Update()
    {
        if (shopIndex != "null" && shopIndex.Length > 0)
        {

            skillIndex = SceneVarScript.Instance.GetOptionByIndex(shopIndex, "skillIndex", SceneVarScript.Instance.skillShopOption);
            iconIndex = SceneVarScript.Instance.GetOptionByIndex(skillIndex, "icon", SceneVarScript.Instance.skillOption);
            if (iconIndex != "null")
            {
                icon.sprite = SceneVarScript.Instance.skillIcon[int.Parse(iconIndex)];
                cost.text = lobbymanager.Instance.StringDot(SceneVarScript.Instance.GetOptionByIndex(shopIndex, "cost", SceneVarScript.Instance.skillShopOption));
                dataComplete = true;
            }
            else
            {
                dataComplete = false;
            }
        }
        else
        {
            dataComplete =false;
        }
    }
    public void SetIndex(string index)
    {
        skillIndex = index;
    }
    public void Option()
    {
        if (dataComplete == false)
        {
            lobbymanager.Instance.SetLobbyMsg("데이터로드에 실패하였습니다.\n재접속 해주세요.");
            return;
        }
        lobbymanager.Instance.SetSkillOptionUI(SceneVarScript.Instance.GetOptionByIndex(shopIndex, "skillIndex", SceneVarScript.Instance.skillShopOption));
        lobbymanager.Instance.UiOpening(lobbymanager.Instance.uiList[6]);
    }
    public void Buy()
    {
        if (dataComplete == false)
        {
            lobbymanager.Instance.SetLobbyMsg("데이터로드에 실패하였습니다.\n재접속 해주세요.");
            return;
        }
        StartCoroutine(BuyAction(shopIndex));
    }
    IEnumerator BuyAction(string index)
    {
        lobbymanager.Instance.SetLobbyYesOrNoMsg("구매하시겠습니까?");
        yield return new WaitUntil(() => lobbymanager.Instance.uiList[4].activeSelf == false);
        if (lobbymanager.Instance.yesOrNoInput == 0)
        {

        }else if (lobbymanager.Instance.yesOrNoInput == 1)
        {

            //돈 계산
            SceneVarScript.Instance.BuySkillFunc(shopIndex);
        }
    }
}
