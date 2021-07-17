using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopUpSkill : MonoBehaviour
{
    SkillBtn skillBtn;
    // Start is called before the first frame update
    void Start()
    {
        skillBtn = GetComponent<SkillBtn>();
    }

    // Update is called once per frame
    void Update()
    {
        ClickCheck();
    }
    void ClickCheck()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //마우스 클릭시 나를 클릭한거라면
            PointerEventData useSkillBtn = new PointerEventData(EventSystem.current);
            useSkillBtn.position = Input.mousePosition;
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(useSkillBtn, result);
            bool active = false;
            foreach (var item in result)
            {
                if (item.gameObject == gameObject)
                {
                    //Debug.Log("마우스클릭 잘함");
                    active = true;
                    break;
                }
            }
            if (active == false)
            {
                //Debug.Log("마우스클릭 잘못함");
                return;
            }
            if (MainGameManager.mainGameManager.GetNowSkill() != gameObject)
            {
                MainGameManager.mainGameManager.SetNowSkill(gameObject);
                MainGameManager.mainGameManager.popUpMaster.RefreshPopUpContents(5);

            }
            else
            {
                MainGameManager.mainGameManager.SetNowSkill(null);
                MainGameManager.mainGameManager.popUpMaster.UpPopUp(5);
            }
        }
    }
}
