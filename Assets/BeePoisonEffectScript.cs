using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BeePoisonEffectScript : ThrowScript
{
    public List<GameObject> hittedMonster;
    public override void Start()
    {
        base.Start();
        hittedMonster = new List<GameObject>();
    }
    public override void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack_effect") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.1f)
        {
            if (pv.IsMine && canAttack == true)
            {
                canAttack = false;
                MonsterAttack();
            }
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                Destroy(gameObject);
            }

        }
    }
    public override void MonsterAttack()
    {
        monsterScript target;
        Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + attOffset3.x, transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
        if (hitArea.Length > 0)
        {
            for (int i = 0; i < hitArea.Length; i++)
            {
                target = hitArea[i].gameObject.GetComponent<monsterScript>();
                if (target != null)
                {
                    if (target.myPlayer != par.myPlayer && hitArea[i].tag != "boss" && !hittedMonster.Contains(hitArea[i].gameObject))
                    {
                        hittedMonster.Add(hitArea[i].gameObject);
                        var poisonSpeed = (100 - float.Parse(SceneVarScript.Instance.GetOptionByIndex("4", "addSpeed", SceneVarScript.Instance.skillOption))) / 100; //이속 감소량
                        var poisonDamage = float.Parse(SceneVarScript.Instance.GetOptionByIndex("4", "addDamage", SceneVarScript.Instance.skillOption)) / 100; // 도트 데미지량
                        target.pv.RPC("GetPoision", RpcTarget.All, poisonDamage, poisonSpeed);
                    }
                }
            }
        }
    }
}
