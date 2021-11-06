using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldBanana :SkillScript
{
    public GoldBanana(string index)
    {
        this.index = index;
    }
    public override void Active()
    {

    }

    public override void Passive()
    {
        //������ ���ݼӵ�
        SkillManager.Instance.monkeyAttackSpeed = (float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perAttackSpeed", SceneVarScript.Instance.skillOption)) / 100);
        //�ٳ��� ���� ������ �߰� ���
        var moneys = SceneVarScript.Instance.GetOptionByIndex(index, "perMoney", SceneVarScript.Instance.skillOption).Split('/');
        SkillManager.Instance.bananaBonusGold = int.Parse(moneys[MainGameManager.mainGameManager.GetPlayerBuliding()]);
    }

    public override void Unique()
    {
       
    }

    public override void UnPassive()
    {
        SkillManager.Instance.monkeyAttackSpeed = 0;
        SkillManager.Instance.bananaBonusGold = 0;
    }

}
