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
        mytxt.text =cost.ToString()+" G";
        if (cost >= MainGameManager.mainGameManager.GetMoney())
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
        NetworkMaster.Instance.CreatMonster(myname);
    }
    public void TrapCreate()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 bossPos;
        if (MainGameManager.mainGameManager.nowBoss!=null)
        {
            bossPos = MainGameManager.mainGameManager.nowBoss.transform.position;
            if (Mathf.Abs(mousePos.x - bossPos.x) < 3&& NetworkMaster.Instance.GetLayer()==1)
            {
                NetworkMaster.Instance.SendGameMsgFunc("보스 근처에는 생성할 수 없습니다.", 0);
                return;
            }
        }
        if (Mathf.Abs(mousePos.x - NetworkMaster.player.transform.position.x)<3 || (NetworkMaster.otherPlayer!=null&& Mathf.Abs(mousePos.x - NetworkMaster.otherPlayer.transform.position.x) < 3))
        {
            NetworkMaster.Instance.SendGameMsgFunc("플레이어 근처에는 생성할 수 없습니다.", 0);
            return;
        }
        LayerMask mask = NetworkMaster.Instance.GetLayer() == 1? LayerMask.GetMask("upTrap"): LayerMask.GetMask("downTrap");
        Collider2D[] otherTraps = Physics2D.OverlapBoxAll(mousePos,new Vector2(2,2),0,mask);
        if (otherTraps.Length > 0)
        {
            NetworkMaster.Instance.SendGameMsgFunc("근처에 이미 다른 트랩이 존재합니다.", 0);
            return;
        }
        if (NetworkMaster.Instance.dir == true)
        {
            if (GradiantPos.Instance.transform.position.x > mousePos.x)
            {
                NetworkMaster.Instance.CreatMonster(myname, 1, mousePos.x);
            }
            else
            {
                NetworkMaster.Instance.SendGameMsgFunc("시야가 없습니다.", 0);
            }
        }
        else
        {
            if (GradiantPos.Instance.transform.position.x < mousePos.x)
            {
                NetworkMaster.Instance.CreatMonster(myname, 1, mousePos.x);
            }
            else
            {
                NetworkMaster.Instance.SendGameMsgFunc("시야가 없습니다.", 0);
            }
        }
   
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
        if (img.GetComponent<creatImgScript>().redLineCreateLayer==1 || img.GetComponent<creatImgScript>().redLineCreateLayer == 2)
        {
            if (createType == 0)
            {
                Create();
            }
            else if(createType==1)
            {
                TrapCreate();
            }
        }
        img.GetComponent<Image>().enabled = false;

    }
    public void CanNotSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
