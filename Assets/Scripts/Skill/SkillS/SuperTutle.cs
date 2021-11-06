using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperTutle : SkillScript
{
    public int cmp(GameObject obj1,GameObject obj2)
    {
        return Mathf.Abs(obj1.transform.position.x - NetworkMaster.player.transform.position.x).CompareTo(Mathf.Abs(obj2.transform.position.x - NetworkMaster.player.transform.position.x));
    }
    public SuperTutle(string index)
    {
        this.index = index;
    }
    public override void Active()
    {
        //차출될 몬스터 이름 설정
        var settingName = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "needMonster", SceneVarScript.Instance.skillOption)).Split(',')[0];
        Debug.Log(settingName);
        //차출몬스터 필요 갯수 설정
        var settingNeed = int.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "need0", SceneVarScript.Instance.skillOption));

        //소환될 몬스터 이름 설정
        var spawnMonster = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "spawnMonster", SceneVarScript.Instance.skillOption));

        //플레이어로부터 가장 멀리있는 거북이들 차출

        int spawnLayer = -1;
        var list = MainGameManager.GetMonsterList();
        list[settingName].Sort(cmp);
        Debug.Log("플레이어와 거북이 거리를 기준으로 내림차순 정렬 완료");
        spawnLayer = list[settingName][0].GetComponent<monsterScript>().GetLayerNum();
        //차출된 거북이를 사망시키면서 마지막에 플레이어로 부터 가장 가까운 거북이의 레이어를 구해오기
        for (int i = 0; i < settingNeed; i++)
        {
            //10마리 모두 사망시키기
            list[settingName][i].GetComponent<monsterScript>().hp = 0;
        }
        //저장된 레이어에 거북이 소환시키기
        NetworkMaster.Instance.CreatMonster(spawnMonster, 1, NetworkMaster.Instance.CreatPosXOffset(NetworkMaster.player), spawnLayer, NetworkMaster.player);
        NetworkMaster.Instance.SendGameMsgFunc("슈퍼 거북이가 전장에 출현했습니다!", 1);
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
