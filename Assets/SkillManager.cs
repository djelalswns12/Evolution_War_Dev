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
        SKillUniqueEffect();
        SetSkillBtn();
        SkillCoolManager();
        for (int i = 0; i < SceneVarScript.Instance.skillOption.Length; i++)
        {
            UseSkill(i.ToString());
        }
    }
    public void UseSkill(string index)
    {
        switch (index)
        {
            case "0":
                skillActiveList[int.Parse(index)] = skill0_Assemble(index);
                break;
            case "1":
                skillActiveList[int.Parse(index)] = skill1_SuperTutle(index);
                break;
            case "2":
                skillActiveList[int.Parse(index)] = skill2_GoldBanana(index);
                break;
            case "3":
                skillActiveList[int.Parse(index)] = skill3_Stronger(index);
                break;
            case "4":
                skillActiveList[int.Parse(index)] = skill4_PoisonSpear(index);
                break;
            default:
                Debug.Log("존재하지 않는 스킬을 요청하였습니다.");
                break;
        }

    }
    public bool skill0_Assemble(string index)
    {
        int slotNum = SkillCheckIsTake(index);
        if (slotNum == -1)
        {
            return false;
        }
        if (SkillCheckNeedMoney(index) == false || SkillCheckNeedMonster(index) == false)
        {
            skill0_Assemble_passive(false, index);
            skillOnParticle[slotNum].gameObject.SetActive(false);
            return false;
        }
        //사용 여부 ON/OFF
        skillOnParticle[slotNum].gameObject.SetActive(true);

        //기본  지속 효과
        skill0_Assemble_passive(true, index);

        return true;
    }
    public bool skill1_SuperTutle(string index)
    {
        int slotNum = SkillCheckIsTake(index);
        if (slotNum == -1)
        {
            return false;
        }
        if (SkillCheckNeedMoney(index) == false || SkillCheckNeedMonster(index) == false)
        {
            skillOnParticle[slotNum].gameObject.SetActive(false);
            return false;
        }
        skillOnParticle[slotNum].gameObject.SetActive(true);
        return true;
    }
    public bool skill2_GoldBanana(string index)
    {
        int slotNum = SkillCheckIsTake(index);
        if (slotNum == -1)
        {
            return false;
        }
        if (SkillCheckNeedMoney(index) == false || SkillCheckNeedMonster(index) == false)
        {
            skill2_GoldBanana_passive(false, index);
            skillOnParticle[slotNum].gameObject.SetActive(false);
            return false;
        }
        //사용 여부 ON/OFF
        skillOnParticle[slotNum].gameObject.SetActive(true);

        //기본  지속 효과
        skill2_GoldBanana_passive(true, index);

        return true;

    }
    public bool skill3_Stronger(string index)
    {
        int slotNum = SkillCheckIsTake(index);
        if (slotNum == -1)
        {
            return false;
        }
        if (SkillCheckNeedMoney(index) == false || SkillCheckNeedMonster(index) == false)
        {
            skillOnParticle[slotNum].gameObject.SetActive(false);
            return false;
        }
        //사용 여부 ON/OFF
        skillOnParticle[slotNum].gameObject.SetActive(true);

        //기본  지속 효과

        return true;
    }
    public bool skill4_PoisonSpear(string index)
    {
        int slotNum = SkillCheckIsTake(index);
        if (slotNum == -1)
        {
            return false;
        }
        if (SkillCheckNeedMoney(index) == false || SkillCheckNeedMonster(index) == false)
        {
            skillOnParticle[slotNum].gameObject.SetActive(false);
            return false;
        }
        //사용 여부 ON/OFF
        skillOnParticle[slotNum].gameObject.SetActive(true);

        //기본  지속 효과
        return true;
    }
    #region 엑티브 효과
    //////////////////////////    엑티브 효과      ////////////////////////////
    public void skill1_SuperTutle_Active(string index)
    {
        skillCool[int.Parse(index)]= float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "cool", SceneVarScript.Instance.skillOption));
        //차출될 몬스터 이름 설정
        var settingName = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "needMonster", SceneVarScript.Instance.skillOption)).Split(',')[0];
        Debug.Log(settingName);
        //차출몬스터 필요 갯수 설정
        var settingNeed = int.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "need0", SceneVarScript.Instance.skillOption));

        //소환될 몬스터 이름 설정
        var spawnMonster = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(index, "spawnMonster", SceneVarScript.Instance.skillOption));

        //플레이어로부터 가장 멀리있는 거북이 10마리 차출

        int spawnLayer = -1;
        for (int i = 0; i < list[settingName].Count; i++)
        {
            float setDistance = Mathf.Abs(list[settingName][i].transform.position.x - NetworkMaster.player.transform.position.x);
            for (int j = i; j < list[settingName].Count; j++)
            {
                float distance = Mathf.Abs(list[settingName][j].transform.position.x - NetworkMaster.player.transform.position.x);
                if (setDistance < distance)
                {
                    GameObject tmp = list[settingName][j];
                    list[settingName][j] = list[settingName][i];
                    list[settingName][i] = tmp;
                    setDistance = distance;
                }
            }
        }
        Debug.Log("플레이어와 거북이 거리를 기준으로 내림차순 정렬 완료");
        //차출된 거북이를 사망시키면서 마지막에 플레이어로 부터 가장 가까운 거북이의 레이어를 구해오기
        for (int i = 0; i < settingNeed; i++)
        {
            //10마리 모두 사망시키기
            list[settingName][i].GetComponent<monsterScript>().hp = 0;
        }
        list[settingName][list[settingName].Count-1].GetComponent<monsterScript>().GetLayerNum();
        //저장된 레이어에 거북이 소환시키기
        NetworkMaster.Instance.CreatMonster(spawnMonster, 1, NetworkMaster.Instance.CreatPosXOffset(), spawnLayer);
        NetworkMaster.Instance.SendGameMsgFunc("슈퍼 거북이가 전장에 출현했습니다!",1);
    }


    public void skill3_Stronger_Active(string index)
    {
        skillCool[int.Parse(index)] = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "cool", SceneVarScript.Instance.skillOption));
        var perTime = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perTime", SceneVarScript.Instance.skillOption));
        var perAttackSpeed =float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perAttackSpeed", SceneVarScript.Instance.skillOption))/100;
        foreach (var item in list["Lion"])
        {
            item.GetComponent<monsterScript>().FuncLionAttackSpeedBuff(perTime,perAttackSpeed);
        }
        //foreach(var item in list)
        //{
        //    foreach(var ele in list[item.Key])
        //    {
        //        ele.GetComponent<monsterScript>().FuncLionAttackSpeedBuff(perTime, perAttackSpeed);
        //    }
        //}
        NetworkMaster.Instance.SendGameMsgFunc("약육강식이 발동되었습니다!");
    }
    public void skill4_PoisonSpear_Active(string index)
    {
        skillCool[int.Parse(index)] = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "cool", SceneVarScript.Instance.skillOption));
        var perTime = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "perTime", SceneVarScript.Instance.skillOption)); // 지속시간
        var addSpeed = (100-float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "addSpeed", SceneVarScript.Instance.skillOption))) / 100; //이속 감소량
        var addDamage = float.Parse(SceneVarScript.Instance.GetOptionByIndex(index, "addDamage", SceneVarScript.Instance.skillOption)) / 100; // 도트 데미지량
        foreach (var item in list["OldHuman"])
        {
            item.GetComponent<monsterScript>().FuncOldHumanBuff(perTime, addDamage,addSpeed);
        }
        NetworkMaster.Instance.SendGameMsgFunc("독창 전술이 발동되었습니다!");
    }
    #endregion


    #region 패시브 스킬
    //////////////////////////패시브 스킬 설명 ( 기본 지속 효과 )////////////////////////////
    public void skill0_Assemble_passive(bool isActive, string skill_Index)
    {
        if (isActive)
        {
            float perTime = float.Parse(SceneVarScript.Instance.GetOptionByIndex(skill_Index, "perTime", SceneVarScript.Instance.skillOption));
            assembleTime += Time.deltaTime;
            if (assembleTime > perTime)
            {
                assembleTime = 0;
                string data = SceneVarScript.Instance.GetDBSource(SceneVarScript.Instance.GetOptionByIndex(skill_Index, "needMonster", SceneVarScript.Instance.skillOption));
                string[] moneys= SceneVarScript.Instance.GetOptionByIndex(skill_Index, "perMoney", SceneVarScript.Instance.skillOption).Split('/');
                int perMoney = int.Parse(moneys[MainGameManager.mainGameManager.GetPlayerBuliding()]);
                //Debug.Log("필요 데이터:" + data);
                var needs = data.Split(',');
                for (int i = 0; i < needs.Length / 2; i++)
                {
                    foreach (var obj in list[needs[i * 2]])
                    {
                        MainGameManager.mainGameManager.CreatGoldEffect(obj.transform.position, perMoney);
                    }
                }
            }
        }
        else
        {
            assembleTime = 0;
        }
    }
    public void skill2_GoldBanana_passive(bool isActive, string skill_Index)
    {
        if (isActive)
        {
            //원숭이 공격속도
            monkeyAttackSpeed = (float.Parse(SceneVarScript.Instance.GetOptionByIndex(skill_Index, "perAttackSpeed", SceneVarScript.Instance.skillOption)) / 100);
            //바나나 공격 성공시 추가 골드
            var moneys= SceneVarScript.Instance.GetOptionByIndex(skill_Index, "perMoney", SceneVarScript.Instance.skillOption).Split('/');
            bananaBonusGold = int.Parse(moneys[MainGameManager.mainGameManager.GetPlayerBuliding()]);
        }
        else
        {
            monkeyAttackSpeed = 0;
            bananaBonusGold = 0;
        }
    }

    //////////////////////////패시브 스킬 설명 ( 기본 지속 효과 )////////////////////////////
    #endregion

    #region 고유효과
    ////////////////////////// 스킬 설명 (고유 효과 )////////////////////////////
    public void skill3_Stronger_Unique(bool isActive, string skill_Index)
    {
        if (isActive)
        {
            //사자 보스 뽀공
            lionBossBonusDamage = (float.Parse(SceneVarScript.Instance.GetOptionByIndex(skill_Index, "addDamage", SceneVarScript.Instance.skillOption)) / 100);
        }
        else
        {
            lionBossBonusDamage = 0;

        }
    }

    #endregion

    public void SKillUniqueEffect()
    {
        bool[] OnSKillList = new bool[SceneVarScript.Instance.skillOption.Length];
        foreach (var item in skillBtn)
        {
            var findIndex = SceneVarScript.Instance.GetOptionByName(item.GetComponent<SkillBtn>().myName, "index", SceneVarScript.Instance.skillOption);
            if (findIndex != "null")
            {
                OnSKillList[int.Parse(SceneVarScript.Instance.GetOptionByName(item.GetComponent<SkillBtn>().myName, "index", SceneVarScript.Instance.skillOption))] = true;
            }
        }
        for (int i = 0; i < OnSKillList.Length; i++)
        {
            switch (i)
            {
                case 3:
                    skill3_Stronger_Unique(OnSKillList[i], i.ToString());
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
        if (skillActiveList[useSkillIndex] == false)
        {
            NetworkMaster.Instance.SendGameMsgFunc("스킬 사용 조건이 부족합니다.");
            return;
        }
        switch (useSkillIndex)
        {
            case 1:
                skill1_SuperTutle_Active(useSkillIndex.ToString());
                break;
            case 3:
                skill3_Stronger_Active(useSkillIndex.ToString());
                break;
            case 4:
                skill4_PoisonSpear_Active(useSkillIndex.ToString());
                break;
        }
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
    public void SetSkillBtn()
    {
        //인게임 스킬 아이콘의 값을 설정한다.

        for (int i = 0; i < skillBtn.Length; i++)
        {
            if (SceneVarScript.Instance.GetUserOption("skill" + (i + 1)) != null)
            {
                skillBtn[i].GetComponent<SkillBtn>().myName = SceneVarScript.Instance.GetOptionByIndex(SceneVarScript.Instance.GetUserOption("skill" + (i + 1)), "name", SceneVarScript.Instance.skillOption);

            }
        }
    }
}


//스킬 매니져에서 현재 가지고 있는 스킬의 데이터를 확인한다.

//게임 매니져에서는 스킬매니져의 값을 가져온다

//스킬 매니져에서 게임메니져의 값을 읽어서 활성화 여부를 결정한다.
