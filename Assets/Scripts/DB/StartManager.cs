using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    public static StartManager Instance;
    public GameObject UINaming;
    public GameObject UIOverLap;
    public Text InputName;
    public Button createBtn;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            UIOverLap.SetActive(true);
        }
    }
    public void PopUpNaming()
    {
        LobbySoundManager.Instance.UiOpenSoundPlay();
        UINaming.SetActive(true);
    }
    public void CallCreateUser()
    {
        createBtn.interactable = false;
        SceneVarScript.Instance.StartCoroutine(SceneVarScript.Instance.CreateUser(InputName.text));
    }
    public void PopUpCloseNaming()
    {
        LobbySoundManager.Instance.BtnClickSoundPlay();
        AuthScript.Instance.tabToPlay.SetActive(true);
        AuthScript.Instance.tabToPlay.GetComponent<Text>().text = "Tab to Play";
        UINaming.SetActive(false);
    }
    public void OverLapNickName()
    {
        Debug.Log("Áßº¹´Ð³Û");
        createBtn.interactable = true;
        UIOverLap.SetActive(true);
        Debug.Log("Áßº¹´Ð³Û2");

    }
}
