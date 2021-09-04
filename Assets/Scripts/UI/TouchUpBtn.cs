using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchUpBtn : MonoBehaviour
{
    public Text needMoney,level;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        level.text ="Lv."+(MainGameManager.mainGameManager.touchLevel + 1);
        needMoney.text = MainGameManager.mainGameManager.StringDot(MainGameManager.mainGameManager.GetNextTouchCost())+" G";
    }
}
