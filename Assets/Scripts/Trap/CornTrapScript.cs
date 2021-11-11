using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornTrapScript : TrapScript
{
    protected override void UseSkill()
    {
        if (setCool <= nowCool)
        {
            GetGold();
            monster.RpcCallTrapCoolReset();
        }
    }

    public void GetGold()
    {
        var point = int.Parse(SceneVarScript.Instance.GetOptionByName(myName, "perMoney", SceneVarScript.Instance.trapOption));
        if (NetworkMaster.player != monster.myPlayer.gameObject && NetworkMaster.Instance.GetMode()=="AI")
        {
            AIManager.Instance.CalMoney(point);
        }
        else if(NetworkMaster.player == monster.myPlayer.gameObject)
        {
            MainGameManager.mainGameManager.CreatGoldEffect(transform.position, (int)point);
        }
    }
}
