using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeManager : ScrollAddManager
{
    // Start is called before the first frame update
    public Text thema;
    public Text content;
    public Text date;
    void Update()
    {
        if (SceneVarScript.Instance.noticeReset == false)
        {
            Clear();
            data = SceneVarScript.Instance.noticeOption;
            PushElements();
            SceneVarScript.Instance.noticeReset = true;
        }
        thema.text = SceneVarScript.Instance.noticeOption[lobbymanager.Instance.noticeIndex]["thema"].ToString();
        content.text = SceneVarScript.Instance.noticeOption[lobbymanager.Instance.noticeIndex]["content"].ToString();
        date.text = "¿€º∫¿œ : "+SceneVarScript.Instance.noticeOption[lobbymanager.Instance.noticeIndex]["date"].ToString();
    }
    private void Start()
    {
        data = SceneVarScript.Instance.noticeOption;
        PushElements();
    }
    protected override void SetData(GameObject selectObj, int index)
    {
        NoticeChart chart = selectObj.GetComponent<NoticeChart>();
        chart.noticeIndex = index;
        //chart.WinLoseRating
    }
}
