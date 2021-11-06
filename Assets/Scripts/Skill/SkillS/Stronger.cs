using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stronger : SkillScript
{
    public Stronger(string index)
    {
        this.index = index;
    }
    public override void Active()
    {
        var perTime = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perTime", SceneVarScript.Instance.skillOption));
        var perAttackSpeed = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perAttackSpeed", SceneVarScript.Instance.skillOption)) / 100;
        foreach (var item in MainGameManager.GetMonsterList()["Lion"])
        {
            item.GetComponent<monsterScript>().FuncLionAttackSpeedBuff(perTime, perAttackSpeed);
        }
        NetworkMaster.Instance.SendGameMsgFunc("약육강식이 발동되었습니다!");
    }

    public override void Passive()
    {

    }

    public override void Unique()
    {
       SkillManager.Instance.lionBossBonusDamage = (float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "addDamage", SceneVarScript.Instance.skillOption)) / 100);
    }

    public override void UnPassive()
    {

    }
}
