﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public static CameraScript Instance;
    public bool right, left;
    public int moveStautu;

    public float speed;

    public float backspeed,setcameraspeed;
    public GameObject back,groundsize;
    public GameObject[] backList;
    public GameObject cameraSquareL, cameraSquareR;

    public Vector3 setCameraPosition;
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
        for (int i=0;i<backList.Length;i++)
        {
            if (NetworkMaster.Instance.gameStage == i)
            {
                back = backList[i];
                backList[i].SetActive(true);
            }
            else
            {
                backList[i].SetActive(false);
            }
        }
        //back boundary : +- (groundsize.GetComponent<SpriteRenderer>().bounds.size.x / 2- back.GetComponent<SpriteRenderer>().bounds.size.x / 2)
        float backbounday = groundsize.GetComponent<SpriteRenderer>().bounds.size.x / 2 - back.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        Vector2 backpos=new Vector2(backbounday*transform.position.x/cameraSquareR.transform.position.x,0);
        backpos.y = back.transform.position.y;
        if (Rightfunc() == true && Leftfunc() == true)
        back.transform.position = backpos;

        if (moveStautu == 0)
        {
            if (Rightfunc() == false)
            {
                for(;Rightfunc()==false ;)
                {
                    transform.position += Vector3.left * 0.01f;
                }
            }
            if (Leftfunc() == false)
            {
                for (; Leftfunc() == false;)
                { 
                    transform.position += Vector3.right * 0.01f;
                }
            }
        }
        else
        {
                Vector3 pos = Vector2.Lerp(transform.position, setCameraPosition, Time.deltaTime * setcameraspeed);
                pos.z = -10;
                transform.position = pos;
                if (Vector2.Distance(transform.position, setCameraPosition) < 0.5 || transform.position.x > cameraSquareL.transform.position.x == false || transform.position.x < cameraSquareR.transform.position.x==false)
                {
                   
                    setCameraPosition = Vector3.zero;
                    moveStautu = 0;
                }
        }
    }
    public bool Rightfunc()
    {
        if (transform.position.x < cameraSquareR.transform.position.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool Leftfunc()
    {
        if (transform.position.x > cameraSquareL.transform.position.x)
        {
            return true;
        }
        else
        {
            return false;
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
    public void SetCameraMove(Vector3 pos,int move=1)
    {
        moveStautu = move;
        setCameraPosition = pos;
    }
}
