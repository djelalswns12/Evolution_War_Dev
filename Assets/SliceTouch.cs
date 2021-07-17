using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class SliceTouch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    bool moveBySpeed;
    public float CameraSpeed,cameraDeSpeed,toZeroSpeed;
    CameraScript camerascript;
    public Vector2 startPos,cameraStartPos,slicePos;
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

        if (Mathf.Abs(CameraSpeed) > toZeroSpeed && moveBySpeed==true)
        {
            if (camerascript.Rightfunc() == true && camerascript.Leftfunc() == true)
            {
                var newPos = Vector3.left * CameraSpeed * Time.deltaTime;
                if (newPos.x>0)
                {
                    newPos.x+= camerascript.Rightfunc(newPos.x+Camera.main.transform.position.x);
                }
                else
                {
                    newPos.x += camerascript.Leftfunc(newPos.x + Camera.main.transform.position.x);
                }
                Camera.main.transform.position += newPos;
                CameraSpeed = Mathf.Lerp(CameraSpeed, 0, Time.deltaTime * cameraDeSpeed);
                //Debug.Log("°ª:"+CameraSpeed);
            }
            else
            {
                CameraSpeed = 0f;
            }
        }

    }
    public void OnPointerDown(PointerEventData eventData)
    {

       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
     
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        moveBySpeed = false;
        CameraSpeed = 0;
        camerascript.moveStautu = 0;
        cameraStartPos = Camera.main.transform.position;
        startPos =eventData.position;
        slicePos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.position.x - startPos.x < 0 )
        {
            if (camerascript.Rightfunc() == true)
            {
                var newPos=new Vector3(cameraStartPos.x-(Camera.main.ScreenToWorldPoint(eventData.position).x - Camera.main.ScreenToWorldPoint(startPos).x),0,-10);
                newPos.x += camerascript.Rightfunc(newPos.x);
                Camera.main.transform.position = newPos;
                cameraStartPos = Camera.main.transform.position;
                startPos = eventData.position;
            }
        }
        else
        {
            if (camerascript.Leftfunc() == true)
            {
                var newPos = new Vector3(cameraStartPos.x - (Camera.main.ScreenToWorldPoint(eventData.position).x - Camera.main.ScreenToWorldPoint(startPos).x), 0, -10);
                newPos.x += camerascript.Leftfunc(newPos.x);
                Camera.main.transform.position = newPos;
                cameraStartPos = Camera.main.transform.position;
                startPos = eventData.position;
            }
        }
        CameraSpeed = (Camera.main.ScreenToWorldPoint(eventData.position).x - Camera.main.ScreenToWorldPoint(slicePos).x) / Time.deltaTime;
        slicePos = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       //Debug.Log($"cameraSpeed:{CameraSpeed}");
        moveBySpeed = true;
    }
}
