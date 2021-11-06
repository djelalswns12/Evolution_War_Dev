using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDestroy : SkillScript
{
    public BuildingDestroy(string index)
    {
        this.index = index;
    }
    public override void Active()
    {
        //Debug.Log("��Ƽ�� �ߵ�");
    }

    public override void Passive()
    {
        foreach (var monsters in MainGameManager.GetMonsterList())
        {
            foreach (var monster in monsters.Value)
            {
                if (monster != null)
                {
                    monsterScript mob = monster.GetComponent<monsterScript>();
                    if (!mob.isPlayerMultiAttack)
                    {
                        mob.StartCoroutine(mob.PlayerMultiAttack(9999));
                    }
                }
            }
        }
        //Debug.Log("�⺻ ����ȿ�� �ߵ�");
    }

    public override void Unique()
    {

        //Debug.Log("����ȿ�� �ߵ�");
    }
    public override void UnPassive()
    {
        foreach (var monsters in MainGameManager.GetMonsterList())
        {
            foreach (var monster in monsters.Value)
            {
                if (monster != null)
                {
                    monsterScript mob = monster.GetComponent<monsterScript>();
                    mob.isPlayerMultiAttack = false;
                }
            }
        }
    }
}
