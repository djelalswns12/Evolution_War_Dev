using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CreateBtn : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Text mytxt;
    public string name;
    public bool setImg;
    public GameObject img;
    int Index;

    // Start is called before the first frame update
    void Start()
    {
        mytxt = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Index = NetworkMaster.Instance.SetMonsterNametoIndex(name);
        //mytxt.text = NetworkMaster.Instance.GetMonsterOption(Index, 6) + "\n"+ NetworkMaster.Instance.GetMonsterOption(Index, 1)+"원";
        mytxt.text = NetworkMaster.Instance.GetMonsterOption(Index, 1)+" G";
    
    
    }
    public void Create()
    {

        NetworkMaster.Instance.CreatMonster(name);
    }
    public void CreatePopUp()
    {

        //NetworkMaster.Instance.CreatMonster(name);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        setImg = true;
     
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
