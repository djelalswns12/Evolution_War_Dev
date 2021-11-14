using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffControl : MonoBehaviour
{
    // Start is called before the first frame update
    List<GameObject> buffList;
    monsterScript monster;
    public GameObject buff;
    public SpriteRenderer[] buffSprites;
    Dictionary<string, string> hasBuffList=new Dictionary<string, string>();
    SpriteRenderer sp;
    int index = 0;
    void Start()
    {
        monster = GetComponent<monsterScript>();
        buffSprites = buff.GetComponentsInChildren<SpriteRenderer>(true);
        sp = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        index = 0;
        if (hasBuffList.Count > 0)
        {
            foreach (var item in hasBuffList)
            {
                buffSprites[index].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                buffSprites[index].sortingLayerName = sp.sortingLayerName;
                buffSprites[index].sprite = SceneVarScript.Instance.skillIcon[int.Parse(item.Value)];
                buffSprites[index].gameObject.SetActive(true);
                index++;
            }
        }
        for (int i = index; i < buffSprites.Length; i++)
        {
            buffSprites[i].gameObject.SetActive(false);
        }
        ////////////////////////////////////
        if (monster.hasLionBuffFlag)
        {
            buffInsert("LionBuff","3");
        }
        else
        {
            buffDelete("LionBuff");
        }
        ////////////////////////////////////
   
        /////////////////////////////////////
        if (monster.hasOldHumanBuffFlag)
        {
            buffInsert("OldHumanBuff", "4");
        }
        else
        {
            buffDelete("OldHumanBuff");
        }
        ////////////////////////////////////////

        /////////////////////////////////////////
        if (monster.hasPoisionFlag)
        {
            buffInsert("PoisionBuff","8");
        }
        else
        {
            buffDelete("PoisionBuff");
        }
        /////////////////////////////////////////
        
        /////////////////////////////////////////
        if (monster.hasThornsBuffFlag)
        {
            buffInsert("ThornsBuff","9"); //
        }
        else
        {
            buffDelete("ThornsBuff");
        }
        /////////////////////////////////////////
        
        /////////////////////////////////////////
        if (monster.hasGoldBananaFlag)
        {
            buffInsert("GoldBananaBuff", "2"); //바나나
        }
        else
        {
            buffDelete("GoldBananaBuff");
        }
        /////////////////////////////////////////
        if (monster.trapEnhanceFlag)
        {
            buffInsert("trapEnhanceFlag", "7"); //트랩마스터 아이콘 7
        }
        else
        {
            buffDelete("trapEnhanceFlag");
        }
        /////////////////////////////////////////
    }
    void buffInsert(string myName,string icon)
    {
        if (hasBuffList.ContainsKey(myName) == false)
        {
            hasBuffList.Add(myName, icon);
        }
    }
    void buffDelete(string myName)
    {
        if (hasBuffList.ContainsKey(myName) == true)
        {
            hasBuffList.Remove(myName);
        }
    }
}
