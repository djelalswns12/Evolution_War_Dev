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

    private AudioSource myAudio;

    public float speed;
    public Text nameTxt,optionTxt;

    public Image myImage;
    public string myname;

    public CreateBtn btn;
    // Start is called before the first frame update
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        btn.SetMyName(myname);
        preParSize = parObj.sizeDelta;
        prePos = obj.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        optionTxt.text = NetworkMaster.Instance.GetMonsterOption(myname, "desc");
        nameTxt.text = NetworkMaster.Instance.GetMonsterOption(myname, "nickname");
        myImage.sprite= MainGameManager.mainGameManager.trapIconList[int.Parse(NetworkMaster.Instance.GetMonsterOption(myname, "icon"))-1000];

    }
    public void StartOpitonMove()
    {
        if (myRoutine != null)
        {
            //만약 현재 실행중인 코루틴이 있다면 제거
            StopCoroutine(myRoutine);
        }
        myRoutine=StartCoroutine(OptionObjMove());
        isOn = !isOn;
    }
    IEnumerator OptionObjMove()
    {
        myAudio.PlayOneShot(myAudio.clip);
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
