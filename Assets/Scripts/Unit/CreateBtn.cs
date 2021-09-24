using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CreateBtn : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Text mytxt;
    public string myname;
    public bool setImg;
    public GameObject img;
    int Index;
    public GameObject blind;
    public int cost;
    public Button myBtn;
    public int createType;
    // Start is called before the first frame update
    void Start()
    {
        myBtn = GetComponent<Button>();
        blind = transform.Find("blind").gameObject;
        if (mytxt == null)
        {
            createType = 0;
            mytxt = GetComponentInChildren<Text>();
        }
        else
        {
            createType = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        cost = int.Parse(NetworkMaster.Instance.GetMonsterOption(myname, "cost"));
        //mytxt.text = NetworkMaster.Instance.GetMonsterOption(Index, 6) + "\n"+ NetworkMaster.Instance.GetMonsterOption(Index, 1)+"원";
        mytxt.text =cost.ToString()+"G";
        if (cost > MainGameManager.mainGameManager.GetMoney())
        {
            blind.SetActive(true);
        }
        else
        {
            blind.SetActive(false);
        }
    }
    public void Create()
    {
        NetworkMaster.Instance.CreatMonster(myname,0,0,-1,NetworkMaster.player);
    }
    public void SetMyName(string s)
    {
        myname = s;
    }
    public void CreatePopUp()
    {
        var nav = MainGameManager.mainGameManager.RightNav.GetComponent<RightNav>();
        if (nav.GetMyName() != myname)
        {
            nav.OpenNav();
            nav.ResetPopUp(0);
            nav.SetMyName(myname);
        }
        else
        {
            nav.CloseNav();
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cost <= MainGameManager.mainGameManager.GetMoney())
        {
            setImg = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (setImg ==true)
        {
            img.GetComponent<Image>().enabled = true;
            img.GetComponent<Image>().sprite= GetComponent<Image>().sprite;
            setImg = false;
        }
     
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(null);
        img.GetComponent<Image>().enabled = false;
        if (img.GetComponent<creatImgScript>().redLineCreateLayer==1 || img.GetComponent<creatImgScript>().redLineCreateLayer == 2)
        {
            /*
             * redLineCreateLayer=1 : down 
             * redLineCreateLayer=2 : up
             */
            if (createType == 0)
            {
                if (SpawnManager.IsListFull(NetworkMaster.Instance.GetLayer()))
                {
                    NetworkMaster.Instance.SendGameMsgFunc("대기열이 꽉 찼습니다.", 0);
                    return;
                }
                if (!MainGameManager.mainGameManager.SpentGold(int.Parse(SceneVarScript.Instance.GetOptionByName(myname, "cost", SceneVarScript.Instance.monsterOption))))
                {
                    return;
                }
                SpawnManager.AddSpawnerList(myname,NetworkMaster.Instance.GetLayer());
            }
            else if(createType==1)
            {
                NetworkMaster.Instance.CreateTrap(myname,NetworkMaster.player,Camera.main.ScreenToWorldPoint(Input.mousePosition),NetworkMaster.Instance.GetLayer());
            }
        }
        

    }
    public void CanNotSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
