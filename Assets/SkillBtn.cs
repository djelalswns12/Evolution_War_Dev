using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillBtn : MonoBehaviour
{
    public string myName;
    public string myDesc;

    public int myNum=-1;
    public Image myImg;
    private Sprite startSprite;
    public GameObject closeBtn;

    public Vector3 startScale;
    // Start is called before the first frame update
    void Start()
    {
        myImg = GetComponent<Image>();
        startSprite = myImg.sprite;
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (lobbymanager.Instance.selectedSkill == myName)
        {
            transform.localScale = startScale * 1.1f;
        }
        else
        {
            transform.localScale = startScale;
        }
        if (SceneVarScript.Instance.GetOptionByName(myName, "index", SceneVarScript.Instance.skillOption) != "null")
        {
            myImg.sprite = SceneVarScript.Instance.skillIcon[int.Parse(SceneVarScript.Instance.GetOptionByName(myName, "icon", SceneVarScript.Instance.skillOption))];
            myDesc = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByName(myName, "desc", SceneVarScript.Instance.skillOption));
            if (closeBtn)
            {
                closeBtn.SetActive(true);
            }
        }
        else
        {
            myImg.sprite = startSprite;
            if (closeBtn)
            {
                closeBtn.SetActive(false);
            }
        }
    }
    public void RemoveSkillData()
    {
        SceneVarScript.Instance.SetSkillDB((myNum+1), "-1");
    }
    public void BeginDragEvent()
    {
        LobbySoundManager.Instance.BtnClickSoundPlay();
        lobbymanager.Instance.skillTarget.GetComponent<Image>().sprite = myImg.sprite;
        lobbymanager.Instance.skillTarget.transform.position = Input.mousePosition;
        lobbymanager.Instance.skillTarget.SetActive(true);
    }
    public void DragEvent()
    {
        lobbymanager.Instance.skillTarget.transform.position = Input.mousePosition;

    }
    public void EndDrageEvent()
    {
        PointerEventData useSkillBtn = new PointerEventData(EventSystem.current);
        useSkillBtn.position = Input.mousePosition;
        List<RaycastResult> result = new List<RaycastResult>();
        EventSystem.current.RaycastAll(useSkillBtn, result);
        //Debug.Log("드래그 종료 리절트 갯수:" + result.Count);
        if (result.Count > 0)
        {
            for (int i = 0; i < result.Count; i++)
            {
                var num = lobbymanager.Instance.SetUseSkillNumByDrag(result[i].gameObject);
                if (num == -1)
                {
                    continue;
                }
                //Debug.Log($"{num + 1}번째 스킬을 {this.myName}로 설정합니다.");
                SceneVarScript.Instance.SetSkillDB((num + 1), SceneVarScript.Instance.GetOptionByName(this.myName,"index",SceneVarScript.Instance.skillOption));
                break;
            }
        }
        lobbymanager.Instance.skillTarget.SetActive(false);
    }
    public void ClickEvent()
    {
        LobbySoundManager.Instance.BtnClickSoundPlay();
        if (lobbymanager.Instance.selectedSkill != myName)
        {
            lobbymanager.Instance.selectedSkill = myName;
        }
        else
        {
            lobbymanager.Instance.selectedSkill = "";
        }
    }
    public string GetMyName()
    {
        return myName;
    }
}
