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
        var perTime = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perTime", SceneVarScript.Instance.skillOption)); // ���ӽð�
        var addSpeed = (100 - float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "addSpeed", SceneVarScript.Instance.skillOption))) / 100; //�̼� ���ҷ�
        var addDamage = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "addDamage", SceneVarScript.Instance.skillOption)) / 100; // ��Ʈ ��������
        if (list.ContainsKey("OldHuman"))
        {
            foreach (var item in list["OldHuman"])
            {
                item.GetComponent<monsterScript>().FuncOldHumanBuff(perTime, addDamage, addSpeed);
            }
        }
        NetworkMaster.Instance.SendGameMsgFunc("��â ������ �ߵ��Ǿ����ϴ�!");
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
