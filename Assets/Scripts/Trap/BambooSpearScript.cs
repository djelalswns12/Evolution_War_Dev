using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BambooSpearScript : TrapScript
{
    protected override void UseSkill()
    {
        if (setCool <= nowCool)
        {
            Attack();
            monster.RpcCallTrapCoolReset();
        }
    }
    public void Attack()
    {
        monster.SetNuckbackFlag(true);
        anim.SetBool("attack", true);
    }
    public void AttackEnd()
    {
        anim.SetBool("attack", false);
    }
}
