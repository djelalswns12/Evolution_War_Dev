using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCheckCameraMove : ClickCheck
{
    // Start is called before the first frame update
    public int num;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Check())
        {
            LobbySoundManager.Instance.BtnClickSoundPlay();
            SliceTouch.Instance.AddForceToPlayer(num);
        }
    }
}
