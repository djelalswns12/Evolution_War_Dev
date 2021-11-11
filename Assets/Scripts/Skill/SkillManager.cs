using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;
    //public delegate void SetSkill(string index);
    public Dictionary<string, List<GameObject>> list;
    public Dictionary<string, SkillScript> skillList;
    public GameObject[] skillBtn;
    public GameObject[] skillCoolImage;
    public ParticleSystem[] skillOnParticle;
    public bool[] skillActiveList;
    public float[] skillCool;
    public float assembleTime;
    public float bananaBonusGold, monkeyAttackSpeed;
    public float lionAttackSpeed, lionBonusDamage, lionBossBonusDamage;

    float startSkillBtnPosX;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        skillList = new Dictionary<string, SkillScript>();
        SetSkillList();
        list = MainGameManager.GetMonsterList();
        startSkillBtnPosX = skillBtn[0].transform.position.x;
        skillActiveList = new bool[SceneVarScript.Instance.skillOption.Length];
        skillCool = new float[SceneVarScript.Instance.skillOption.Length];
        // 진영에 따라 UI위치 지정
        if (!NetworkMaster.Instance.dir)
        {
            startSkillBtnPosX *= -1;
            foreach (var item in skillBtn)
            {
                var pos = item.transform.position;
                pos.x = startSkillBtnPosX;
                item.transform.position = pos;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetSkill();

        SkillCoolManager(); //
        
    }
    public void SetSkillList()
    {
        foreach (var skill in SceneVarScript.Instance.skillOption)
        {
            switch (int.Parse(skill["index"].ToString()))
            {
                case 0:
                    skillList.Add(skill["index"].ToString(), new Assemble(skill["index"].ToString()));
                    break;
                case 1:
                    skillList.Add(skill["index"].ToString(), new SuperTutle(skill["index"].ToString()));
                    break;
                case 2:
                    skillList.Add(skill["index"].ToString(), new GoldBanana(skill["index"].ToString()));
                    break;
                case 3:
                    skillList.Add(skill["index"].ToString(), new Stronger(skill["index"].ToString()));
                    break;
                case 4:
                    skillList.Add(skill["index"].ToString(), new PoisonSpear(skill["index"].ToString()));
                    break;
                case 5:
                    skillList.Add(skill["index"].ToString(), new BuildingDestroy(skill["index"].ToString()));
                    break;
                case 6:
                    skillList.Add(skill["index"].ToString(), new DeathBee(skill["index"].ToString()));
                    break;
                case 7:
                    skillList.Add(skill["index"].ToString(), new TrapMaster(skill["index"].ToString()));
                    break;
                default:
                    break;
            }
        }
    }
    public void SkillActive()
    {
        var skillName = MainGameManager.mainGameManager.GetNowSkill().GetComponent<SkillBtn>();
        var useSkillIndex = int.Parse(SceneVarScript.Instance.GetOptionByName(skillName.myName, "index", SceneVarScript.Instance.skillOption));

        if (skillCool[useSkillIndex] > 0)
        {
            NetworkMaster.Instance.SendGameMsgFunc("재사용 대기시간이 남았습니다.");
            return;
        }
        if (skillList[useSkillIndex.ToString()].NeedsCheck() != 0)
        {
            NetworkMaster.Instance.SendGameMsgFunc("조건이 만족되지 않았습니다.");
            return;
        }
        skillList[useSkillIndex.ToString()].Active();
        skillCool[useSkillIndex] = float.Parse(SceneVarScript.Instance.GetOptionByIndex(useSkillIndex.ToString(), "cool", SceneVarScript.Instance.skillOption));
    }
    public void SkillCoolManager()
    {
        for(int i=0;i< skillCool.Length; i++)
        {
            var slotNum = SkillCheckIsTake(i.ToString());
            if (skillCool[i] > 0) {
                if (slotNum != -1)
                {
                    skillCoolImage[slotNum].GetComponentInChildren<Image>().fillAmount = skillCool[i] / float.Parse(SceneVarScript.Instance.GetOptionByName(skillBtn[slotNum].GetComponent<SkillBtn>().myName, "cool", SceneVarScript.Instance.skillOption));
                    skillCoolImage[slotNum].GetComponentInChildren<Text>().text = skillCool[i].ToString("N1");
                    skillCoolImage[slotNum].SetActive(true);
                }
                skillCool[i] -= Time.deltaTime;
            }
            else
            {
                if (slotNum != -1)
                {
                    skillCoolImage[slotNum].SetActive(false);
                }
            }
        }
    }
    public int SkillCheckIsTake(string index)
    {
        for (int slotNum = 0; slotNum < skillBtn.Length; slotNum++)
        {
            if (index == SceneVarScript.Instance.GetOptionByName(skillBtn[slotNum].GetComponent<SkillBtn>().myName, "index", SceneVarScript.Instance.skillOption))
            {
                return slotNum;
            }
        }
        return -1;
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
        if (SceneVarScript.Instance.GetOptionByIndex(index, "needMonster", SceneVarScript.Instance.skillOption) != "null")
        {
            string data = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "needMonster", SceneVarScript.Instance.skillOption));
            //Debug.Log("필요 데이터:" + data);
            var needs = data.Split(',');
            for (int i = 0; i < needs.Length / 2; i++)
            {
                if (!list.ContainsKey(needs[i * 2]))
                {
                    // Debug.Log(needs[i * 2]+"는 소환목록에 없습니다.");
                    return false;
                }
                if (list[needs[i * 2]].Count >= int.Parse(needs[(i * 2) + 1]))
                {
                    // Debug.Log(needs[i * 2] + "는 갯수 만족합니다.");
                }
                else
                {
                    // Debug.Log(needs[i * 2] + "의 갯수가 부족합니다.");
                    return false;
                }
            }
            return true;
        }
        Debug.Log($"(needMonster {index} )index is Not Load So return true");
        return true;
    }
    public bool SkillCheckAnyMonster(string index)
    {
        if (SceneVarScript.Instance.GetOptionByIndex(index, "anyMonster", SceneVarScript.Instance.skillOption) != "null")
        {
            string data = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "anyMonster", SceneVarScript.Instance.skillOption));
            //Debug.Log("필요 데이터:" + data);
            var needs = data.Split('/');
            List<bool> checkList = new List<bool>(needs.Length);
            for (int i = 0; i < needs.Length; i++)
            {
                checkList[i] = false;
                int count=0;
                var monsters = needs[i].Split(',');
                for (int j=0;i<monsters.Length-1;j++)
                {
                    count += list[SceneVarScript.Instance.GetDBSource(monsters[j])].Count;
                    if (count >= int.Parse(monsters[monsters.Length - 1]))
                    {
                        checkList[i] = true;
                        break;
                    }
                }
            }
            foreach(bool check in checkList)
            {
                if (check == false)
                {
                    return false;
                }
            }
            return true;
        }
        Debug.Log($"(needMonster {index} )index is Not Load So return true");
        return true;
    }
    public void SetSkill()
    {
        for (int i = 0; i < skillBtn.Length; i++)
        {
            var skillIndex = SceneVarScript.Instance.GetUserOption("skill" + (i + 1));
            if (SceneVarScript.Instance.GetOptionByIndex(skillIndex ,"index", SceneVarScript.Instance.skillOption) != "null")
            {
                skillBtn[i].GetComponent<SkillBtn>().myName = SceneVarScript.Instance.GetOptionByIndex(skillIndex, "name", SceneVarScript.Instance.skillOption);
                skillList[SceneVarScript.Instance.GetOptionByIndex(skillIndex, "index", SceneVarScript.Instance.skillOption)].Unique();
                skillList[SceneVarScript.Instance.GetOptionByIndex(skillIndex, "index", SceneVarScript.Instance.skillOption)].setSlot = i+1;
                if (skillList[SceneVarScript.Instance.GetOptionByIndex(skillIndex, "index", SceneVarScript.Instance.skillOption)].NeedsCheck() == 0)
                {
                    skillOnParticle[SkillCheckIsTake(skillIndex)].gameObject.SetActive(true);
                    skillList[SceneVarScript.Instance.GetOptionByIndex(skillIndex, "index", SceneVarScript.Instance.skillOption)].Passive();
                }
                else
                {
                    skillOnParticle[SkillCheckIsTake(skillIndex)].gameObject.SetActive(false);
                    skillList[SceneVarScript.Instance.GetOptionByIndex(skillIndex, "index", SceneVarScript.Instance.skillOption)].UnPassive();
                }
            }
        }
    }
}


//스킬 매니져에서 현재 가지고 있는 스킬의 데이터를 확인한다.

//게임 매니져에서는 스킬매니져의 값을 가져온다

//스킬 매니져에서 게임메니져의 값을 읽어서 활성화 여부를 결정한다.
