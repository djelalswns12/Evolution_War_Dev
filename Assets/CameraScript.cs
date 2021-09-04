using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public static CameraScript Instance;
    public bool right, left;
    public int moveStautu;

    public float speed;

    public float backspeed, setcameraspeed, setCameraSpeedToPos;
    public GameObject back, groundsize;
    public GameObject[] backList;
    public GameObject cameraSquareL, cameraSquareR;

    public Vector3 setCameraPosition;

    public SpriteRenderer groundSizeSp,backSp;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
    }
    private void FixedUpdate()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < backList.Length; i++)
        {
            if (NetworkMaster.Instance.gameStage == i)
            {
                back = backList[i];
                backSp = back.GetComponent<SpriteRenderer>();
                backList[i].SetActive(true);
            }
            else
            {
                backList[i].SetActive(false);
                var newBackPos = backList[i].transform.position;
                newBackPos.x = back.transform.position.x;
                backList[i].transform.position = newBackPos;
            }
        }
        //back boundary : +- (groundsize.GetComponent<SpriteRenderer>().bounds.size.x / 2- back.GetComponent<SpriteRenderer>().bounds.size.x / 2)
        float backbounday = groundSizeSp.bounds.size.x / 2 - backSp.bounds.size.x / 2;
        Vector2 backpos = new Vector2(backbounday * transform.position.x / cameraSquareR.transform.position.x, 0);
        backpos.y = back.transform.position.y;
        backpos.x = Mathf.Clamp(backpos.x, -Mathf.Abs(groundSizeSp.bounds.size.x / 2), Mathf.Abs(groundSizeSp.bounds.size.x / 2));
        if (Rightfunc() && Leftfunc())
        {
            back.transform.position = backpos;
        }
        if (moveStautu == 1)
        {
            Vector3 pos = Vector2.Lerp(transform.position, setCameraPosition, Time.deltaTime * setcameraspeed);
            pos.z = -10;

            if (Vector2.Distance(transform.position, setCameraPosition) < 0.5 || transform.position.x > cameraSquareL.transform.position.x == false || transform.position.x < cameraSquareR.transform.position.x == false)
            {
                setCameraPosition = Vector3.zero;
                moveStautu = 0;
            }
            else
            {
                transform.position = pos;
            }
        }
        else if (moveStautu == 2)
        {
             //if( transform.position.x > cameraSquareL.transform.position.x == true || transform.position.x < cameraSquareR.transform.position.x == false
            if (Vector2.Distance(transform.position, setCameraPosition) < 0.5)
            {
                setCameraPosition = Vector3.zero;
                moveStautu = 0;
            }
            else
            {
                if (setCameraPosition.x > 0)
                {
                    if (Rightfunc() == false)
                    {
                        moveStautu = 0;
                        return;
                    }
                }
                else
                {
                    if (Leftfunc() == false)
                    {
                        moveStautu = 0;
                        return;
                    }
                }
                Vector3 pos = Vector2.Lerp(transform.position, setCameraPosition, Time.deltaTime * setCameraSpeedToPos);
                pos.z = -10;
                transform.position = pos;
            }
        }
    }
    public bool Rightfunc()
    {
        if (transform.position.x <= cameraSquareR.transform.position.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public float Rightfunc(float num)
    {
        if (num < cameraSquareR.transform.position.x)
        {
            return 0;
        }
        else
        {
            return cameraSquareR.transform.position.x - num;
        }
    }
    public bool Leftfunc()
    {
        if (transform.position.x >= cameraSquareL.transform.position.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public float Leftfunc(float num)
    {
        if (num > cameraSquareL.transform.position.x)
        {
            return 0;
        }
        else
        {
            return cameraSquareL.transform.position.x - num;
        }
    }
    public void MoveStop()
    {
        if (moveStautu == 0)
        {
            left = false;
            right = false;
        }
    }
    public void SetCameraMove(Vector3 pos, int move = 1)
    {
        if (NetworkMaster.Instance.endState != 0)
        {
            return;
        }
        SliceTouch.Instance.CameraSpeed = 0;
        moveStautu = move;
        setCameraPosition = pos;
    }
    public void CameraMoveToPos(Vector3 pos)
    {
        if (NetworkMaster.Instance.endState != 0)
        {
            return;
        }
        pos.y = 0f;
        pos.z = Camera.main.transform.position.z;
        SetCameraMove(pos,2);
    }
}
