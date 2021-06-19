using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapBtn : MonoBehaviour
{
    public RectTransform obj,parObj,rayOut;
    
    bool isOn;
    Vector2 prePos,preParSize;
    Coroutine myRoutine;


    public float speed;
    public Text nameTxt,optionTxt;

    public string myname;

    public CreateBtn btn;
    // Start is called before the first frame update
    void Start()
    {
        btn.SetMyName(myname);
        preParSize = parObj.sizeDelta;
        prePos = obj.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        optionTxt.text = NetworkMaster.Instance.GetMonsterOption(myname, "desc");
        nameTxt.text = NetworkMaster.Instance.GetMonsterOption(myname, "nickname");
      
    }
    public void StartOpitonMove()
    {
        if (myRoutine != null)
        {
            StopCoroutine(myRoutine);
        }
        myRoutine=StartCoroutine(OptionObjMove());
        isOn = !isOn;
    }
    IEnumerator OptionObjMove()
    {
        if (!isOn)
        {
            while (Vector2.Distance(obj.anchoredPosition, new Vector2(obj.anchoredPosition.x, -141f)) > 1)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rayOut);
                obj.anchoredPosition = Vector2.Lerp(obj.anchoredPosition, new Vector2(obj.anchoredPosition.x, -141f), speed * Time.deltaTime);
                //parObj.anchoredPosition= Vector2.Lerp(parObj.anchoredPosition, new Vector2(parObj.anchoredPosition.x, preParPos.y - 50), speed * Time.deltaTime);
                parObj.sizeDelta = Vector2.Lerp(parObj.sizeDelta, new Vector2(parObj.sizeDelta.x,preParSize.y+100), speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (Vector2.Distance(obj.anchoredPosition, prePos) > 1)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rayOut);
                obj.anchoredPosition = Vector2.Lerp(obj.anchoredPosition, prePos, speed * Time.deltaTime);
                //parObj.anchoredPosition = Vector2.Lerp(parObj.anchoredPosition, preParPos, speed * Time.deltaTime);
                parObj.sizeDelta = Vector2.Lerp(parObj.sizeDelta, preParSize, speed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
        }

    }
}
