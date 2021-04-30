using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class touch : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField]
    CameraScript camerascript;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMaster.player == null)
            return;

        camerascript = CameraScript.Instance;
    }
    public void OnPointerDown(PointerEventData eventData)
    {

        if (NetworkMaster.player == null)
            return;
        if (eventData.position.x > Screen.width / 2)
        {

            camerascript.right = true;
        }
        else
        {
            camerascript.left=true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (NetworkMaster.player == null)
            return;
        camerascript.MoveStop();

    }
}
