using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public bool Check()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //���콺 Ŭ���� ���� Ŭ���ѰŶ��
            PointerEventData Btn = new PointerEventData(EventSystem.current);
            Btn.position = Input.mousePosition;
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(Btn, result);
            bool active = false;
            if (result.Count > 2)
            { 
                return false;
            }
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
                return false;
            }
           
            return true;
        }
        else
        {
            
            return false;
        }
    }
}
