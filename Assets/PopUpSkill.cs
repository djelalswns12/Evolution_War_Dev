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
            //���콺 Ŭ���� ���� Ŭ���ѰŶ��
            PointerEventData useSkillBtn = new PointerEventData(EventSystem.current);
            useSkillBtn.position = Input.mousePosition;
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(useSkillBtn, result);
            bool active = false;
            foreach (var item in result)
            {
                if (item.gameObject == gameObject)
                {
                    //Debug.Log("���콺Ŭ�� ����");
                    active = true;
                    break;
                }
            }
            if (active == false)
            {
                //Debug.Log("���콺Ŭ�� �߸���");
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
