using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assemble : SkillScript
{
    float assembleTime;
    public Assemble(string index)
    {
        this.index = index;
    }
    public override void Active()
    {
       
    }

    public override void Passive()
    {
        float perTime = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perTime", SceneVarScript.Instance.skillOption));
        assembleTime += Time.deltaTime;
        if (assembleTime > perTime)
        {
            assembleTime = 0;
            string data = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "needMonster", SceneVarScript.Instance.skillOption));
            string[] moneys = SceneVarScript.Instance.GetOptionByIndex(index, "perMoney", SceneVarScript.Instance.skillOption).Split('/');
            int perMoney = int.Parse(moneys[MainGameManager.mainGameManager.GetPlayerBuliding()]);
            //Debug.Log("필요 데이터:" + data);
            var needs = data.Split(',');
            for (int i = 0; i < needs.Length / 2; i++)
            {
                foreach (var obj in MainGameManager.GetMonsterList()[needs[i * 2]])
                {
                    if (obj != null)
                    {
                        MainGameManager.mainGameManager.CreatGoldEffect(obj.transform.position, perMoney);
                    }
                }
            }
        }
    }

    public override void Unique()
    {
        
    }

    public override void UnPassive()
    {
        assembleTime = 0;
    }

}
