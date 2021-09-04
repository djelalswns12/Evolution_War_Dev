using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeChart : MonoBehaviour
{
    public int noticeIndex;
    public Text thema;
    public Button open;
    public GameObject openTarget;
    // Start is called before the first frame update
    void Start()
    {
        open.onClick.AddListener(ClickEvent);
    }

    // Update is called once per frame
    void Update()
    {
      thema.text="<color=yellow>No."+(noticeIndex+1)+"</color> " +SceneVarScript.Instance.noticeOption[noticeIndex]["thema"].ToString();
    }
    void ClickEvent()
    {
        lobbymanager.Instance.noticeIndex = noticeIndex;
        lobbymanager.Instance.UiOpening(lobbymanager.Instance.uiList[5]);
    }
}
