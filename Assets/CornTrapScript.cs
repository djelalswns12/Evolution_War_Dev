using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornTrapScript : MonoBehaviour
{
    public int level;
    public float addMoney;
    monsterScript monster;
    public string myName;

    public float setCool;
    public float nowCool;

    public GameObject bar;
    // Start is called before the first frame update
    void Start()
    {
        monster = GetComponent<monsterScript>();
    }

    // Update is called once per frame
    void Update()
    {
        myName = monster.myName;
        BarRendering();
        UseSkill();
        VersionRendering();
        setCool = int.Parse(SceneVarScript.Instance.GetOptionByName(myName, "perTime",SceneVarScript.Instance.trapOption));
        nowCool += Time.deltaTime;
    }
    //¸Å ÁÖ±â ¸¶´Ù °ñµå È¹µæ ÇÔ¼ö
    public void VersionRendering()
    {

    }
    public void GetGold()
    {
        var point = int.Parse(SceneVarScript.Instance.GetOptionByName(myName, "perMoney", SceneVarScript.Instance.trapOption));
        MainGameManager.mainGameManager.CreatGoldEffect(transform.position, (int)point);

    }
    public void UseSkill()
    {
        if (setCool <= nowCool)
        {
            GetGold();
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
