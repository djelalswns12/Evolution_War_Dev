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
            //마우스 클릭시 나를 클릭한거라면
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
                    //Debug.Log("마우스클릭 잘함");
                    active = true;
                    break;
                }
            }
            if (active == false)
            {
                //Debug.Log("마우스클릭 잘못함");
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
