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
    // Start is called before the first frame update
    void Start()
    {
        blind = transform.Find("blind").gameObject;
        mytxt = GetComponentInChildren<Text>();
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
    public void CreatePopUp()
    {

        //NetworkMaster.Instance.CreatMonster(name);
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
        if(img.GetComponent<creatImgScript>().redLineCreateLayer==1 || img.GetComponent<creatImgScript>().redLineCreateLayer == 2)
        {
            Create();
        }
        img.GetComponent<Image>().enabled = false;

    }
}
