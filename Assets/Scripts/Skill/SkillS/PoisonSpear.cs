using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSpear : SkillScript
{
    public PoisonSpear(string index)
    {
        this.index = index;
    }
    public override void Active()
    {
        var list = MainGameManager.GetMonsterList();
        var perTime = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perTime", SceneVarScript.Instance.skillOption)); // 지속시간
        var addSpeed = (100 - float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "addSpeed", SceneVarScript.Instance.skillOption))) / 100; //이속 감소량
        var addDamage = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "addDamage", SceneVarScript.Instance.skillOption)) / 100; // 도트 데미지량
        if (list.ContainsKey("OldHuman"))
        {
            foreach (var item in list["OldHuman"])
            {
                item.GetComponent<monsterScript>().FuncOldHumanBuff(perTime, addDamage, addSpeed);
            }
        }
        NetworkMaster.Instance.SendGameMsgFunc("독창 전술이 발동되었습니다!");
    }

    public override void Passive()
    {

    }

    public override void Unique()
    {
      
    }

    public override void UnPassive()
    {

    }

}
