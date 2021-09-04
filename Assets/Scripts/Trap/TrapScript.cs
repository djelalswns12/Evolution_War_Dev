using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    public int level;
    public float addMoney;
    protected monsterScript monster;
    public string myName;

    public float setCool;
    public float nowCool;

    public bool isLoad;
    public GameObject bar;
    protected Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        isLoad = false;
        anim = GetComponent<Animator>();
        monster = GetComponent<monsterScript>();
    }

    // Update is called once per frame
    void Update()
    {
        myName = monster.myName;
        BarRendering();
        CoolManage();
        if (isLoad == true && monster.pv.IsMine==true)
        {
            UseSkill();
        }
    }
    //매 주기 마다 골드 획득 함수
    public void VersionRendering()
    {

    }
    public void CoolManage()
    {
        setCool = int.Parse(SceneVarScript.Instance.GetOptionByName(myName, "perTime", SceneVarScript.Instance.trapOption));
        nowCool += Time.deltaTime;
        isLoad = true;
    }
    protected virtual void UseSkill()
    {
        if (setCool <= nowCool)
        {
            //사용될 스킬 GetGold();
            nowCool = 0;
        }
    }
    public void BarRendering()
    {
        if (nowCool > setCool)
        {
            bar.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (nowCool <= 0)
        {
            bar.transform.localScale = new Vector3(0, 1, 1);
        }
        else
        {
            bar.transform.localScale = new Vector3(Mathf.Lerp(bar.transform.localScale.x, nowCool / setCool, 0.5f), 1, 1);
        }
    }
}
