using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBee : SkillScript
{
    public monsterScript player;
    public DeathBee(string index)
    {
        this.index = index;
        player = NetworkMaster.player.GetComponent<monsterScript>();
    }
    public override void Active()
    {
  
    }

    public override void Passive()
    {

    }

    public override void Unique()
    {
        if (SkillManager.Instance.skillCool[int.Parse(index)] <= 0)
        {
            if (player.isHitted == true)
            {
                player.isHitted = false;
                NetworkMaster.Instance.CreatMonster("Bee", 0, 0, 0, NetworkMaster.player).GetComponent<monsterScript>().isGhost = true;
                NetworkMaster.Instance.CreatMonster("Bee", 0, 0, 1, NetworkMaster.player).GetComponent<monsterScript>().isGhost = true;
                SkillManager.Instance.skillCool[int.Parse(index)] = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "cool", SceneVarScript.Instance.skillOption));
            }
        }
    }
    public override void UnPassive()
    {

    }
}
