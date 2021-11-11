using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMaster : SkillScript
{
    float perTime;
    float addDamage;
    float perAttackSpeed;
    public TrapMaster(string index)
    {
        this.index = index;
        perTime =float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perTime", SceneVarScript.Instance.skillOption));
        perAttackSpeed= float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perAttackSpeed", SceneVarScript.Instance.skillOption));
        addDamage= float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "addDamage", SceneVarScript.Instance.skillOption));
    }
    public override void Active()
    {
        List<string> traps = new List<string>();
        TrapScript targetMonster;
        foreach (var data in SceneVarScript.Instance.trapOption)
        {
            traps.Add(data["name"].ToString());
        }
        foreach (var trap in traps)
        {
            Debug.Log("이름:" + trap);
            if (MainGameManager.GetMonsterList().ContainsKey(trap))
            {
                foreach (var target in MainGameManager.GetMonsterList()[trap])
                {
                    Debug.Log("존재 이름:" + trap);
                    if (target != null)
                    {
                        Debug.Log("강화될 이름:" + trap);
                        targetMonster = target.GetComponent<TrapScript>();
                        targetMonster.monster.hp = targetMonster.monster.mhp;
                        targetMonster.TrapEnhance(perTime, addDamage, perAttackSpeed);
                    }
                }
            }
        }

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
