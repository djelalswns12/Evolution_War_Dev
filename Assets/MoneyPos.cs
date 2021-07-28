using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPos : MonoBehaviour
{
    float gra;
    public float setGravity,yPower,scalePower;
    public Text txt;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.up*gra*Time.deltaTime;
        gra -= setGravity*Time.deltaTime;
        if (gra < 0)
        {
            transform.localScale *=1-(scalePower * Time.deltaTime);
             if(transform.localScale.x < 0.15)
                {
                    GoldObjectPool.Instance.Push(this);
                } 
        }
    }
    public void Init(Vector2 pos,int gold)
    {
        MainGameManager.mainGameManager.CountMoney(gold);
        gra = yPower;
        transform.localScale = new Vector3(1, 1, 1);
        txt.text = MainGameManager.mainGameManager.StringDot(gold) + "+";
        transform.position = pos;
    }
}
