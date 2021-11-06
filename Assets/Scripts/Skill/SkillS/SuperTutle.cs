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
        //����� ���� �̸� ����
        var settingName = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "needMonster", SceneVarScript.Instance.skillOption)).Split(',')[0];
        Debug.Log(settingName);
        //������� �ʿ� ���� ����
        var settingNeed = int.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "need0", SceneVarScript.Instance.skillOption));

        //��ȯ�� ���� �̸� ����
        var spawnMonster = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "spawnMonster", SceneVarScript.Instance.skillOption));

        //�÷��̾�κ��� ���� �ָ��ִ� �ź��̵� ����

        int spawnLayer = -1;
        var list = MainGameManager.GetMonsterList();
        list[settingName].Sort(cmp);
        Debug.Log("�÷��̾�� �ź��� �Ÿ��� �������� �������� ���� �Ϸ�");
        spawnLayer = list[settingName][0].GetComponent<monsterScript>().GetLayerNum();
        //����� �ź��̸� �����Ű�鼭 �������� �÷��̾�� ���� ���� ����� �ź����� ���̾ ���ؿ���
        for (int i = 0; i < settingNeed; i++)
        {
            //10���� ��� �����Ű��
            list[settingName][i].GetComponent<monsterScript>().hp = 0;
        }
        //����� ���̾ �ź��� ��ȯ��Ű��
        NetworkMaster.Instance.CreatMonster(spawnMonster, 1, NetworkMaster.Instance.CreatPosXOffset(NetworkMaster.player), spawnLayer, NetworkMaster.player);
        NetworkMaster.Instance.SendGameMsgFunc("���� �ź��̰� ���忡 �����߽��ϴ�!", 1);
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
