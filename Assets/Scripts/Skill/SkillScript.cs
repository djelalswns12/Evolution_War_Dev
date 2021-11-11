using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillScript
{
    public string index;
    public int setSlot;
    public abstract void Active();
    public abstract void Passive();
    public abstract void UnPassive();
    public abstract void Unique();
    public int NeedsCheck()
    {
        if (!SkillCheckNeedMoney(index))
        {
            return 1;
        }
        else if (!SkillCheckNeedMonster(index))
        {
            //Debug.Log("���� ����");
            return 2;
        }
        else if (!SkillCheckAnyMonster(index))
        {
            //Debug.Log("���� ����3");
            return 3;
        }
        return 0;
    }
    public bool SkillCheckNeedMoney(string index)
    {
        string data = SceneVarScript.Instance.GetOptionByIndex(index, "needMoney", SceneVarScript.Instance.skillOption);
        if (data != "null")
        {
            if (MainGameManager.mainGameManager.GetMoney() >= int.Parse(data))
            {
                //Debug.Log("Enough Money!");
                return true;
            }
            else
            {
                //Debug.Log("Insufficient Money!");
                return false;
            }
        }
        //Debug.Log($"(needMoney: {index} )index is Not Load So resturn true");
        return true;
    }
    public bool SkillCheckNeedMonster(string index)
    {
        var list = MainGameManager.GetMonsterList();
        if (SceneVarScript.Instance.GetOptionByIndex(index, "needMonster", SceneVarScript.Instance.skillOption) != "null")
        {
            string data = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "needMonster", SceneVarScript.Instance.skillOption));
            //Debug.Log("�ʿ� ������:" + data);
            var needs = data.Split(',');
            for (int i = 0; i < needs.Length / 2; i++)
            {
                if (!list.ContainsKey(needs[i * 2]))
                {
                    //Debug.Log(needs[i * 2]+"�� ��ȯ��Ͽ� �����ϴ�.");
                    return false;
                }
                if (list[needs[i * 2]].Count >= int.Parse(needs[(i * 2) + 1]))
                {
                    //Debug.Log(list[needs[i * 2]].Count + "�� ���� �����մϴ�.");
                }
                else
                {
                    //Debug.Log(list[needs[i * 2]].Count + "�� ������ �����մϴ�.");
                    return false;
                }
            }
            return true;
        }
        //Debug.Log($"(needMonster {index} )index is Not Load So return true");
        return true;
    }
    public bool SkillCheckAnyMonster(string index)
    {
        var list = MainGameManager.GetMonsterList();
        if (SceneVarScript.Instance.GetOptionByIndex(index, "anyMonster", SceneVarScript.Instance.skillOption) != "null")
        {
            string data = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "anyMonster", SceneVarScript.Instance.skillOption));
            //Debug.Log("�ʿ� ������:" + data);
            var needs = data.Split('/');
            bool[] checkList = new bool[needs.Length];
            for (int i = 0; i < needs.Length; i++)
            {
                checkList[i] = false;
                int count = 0;
                var monsters = needs[i].Split(',');
                for (int j = 0; j < monsters.Length - 1; j++)
                {
                    if (list.ContainsKey(monsters[j]))
                    {
                        count += list[monsters[j]].Count;
                    }
                    if (count >= int.Parse(monsters[monsters.Length - 1]))
                    {
                        checkList[i] = true;
                        break;
                    }
                }
            }
            foreach (bool check in checkList)
            {
                if (check == false)
                {
                    return false;
                }
            }
            return true;
        }
        //Debug.Log($"(needMonster {index} )index is Not Load So return true");
        return true;
    }
}
