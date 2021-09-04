using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : MonoBehaviour
{
    public SkillBtn[] redSkills;
    public SkillBtn[] blueSkills;
    public Text redMoney;
    public Text blueMoney;
    public Text rating;
    public Text addRating;

    public Text redUserNameText;
    public Text blueUserNameText;
    public Text rewardMoneyText;

    public Text redTitle;
    public Text blueTitle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < MainGameManager.mainGameManager.enemyUseSkill.Length; i++)
        {
            if (NetworkMaster.Instance.dir == true)
            {
                blueSkills[i].myName = MainGameManager.mainGameManager.enemyUseSkill[i];
            }
            else
            {
                redSkills[i].myName = MainGameManager.mainGameManager.enemyUseSkill[i];
            }
        }
        for (int i = 0; i < MainGameManager.mainGameManager.useSkill.Length; i++)
        {
            if (NetworkMaster.Instance.dir == true)
            {
                redSkills[i].myName = MainGameManager.mainGameManager.useSkill[i];
            }
            else
            {
                blueSkills[i].myName = MainGameManager.mainGameManager.useSkill[i];
            }
        }
        if (NetworkMaster.Instance.dir == true)
        {
            if(NetworkMaster.Instance.endState == 1)
            {
                redTitle.text = "REDÆÀ ½Â¸®";
                blueTitle.text = "BLUEÆÀ ÆÐ¹è";
            }
            else
            {
                redTitle.text = "REDÆÀ ÆÐ¹è";
                blueTitle.text = "BLUEÆÀ ½Â¸®";
            }
            redUserNameText.text = "ID : " + SceneVarScript.Instance.GetUserOption("username");
            blueUserNameText.text = "ID : " + MainGameManager.mainGameManager.enemyUserName;
            redMoney.text =MainGameManager.mainGameManager.StringDot((int)MainGameManager.mainGameManager.allMoney)+"G";
            blueMoney.text = MainGameManager.mainGameManager.StringDot((int)MainGameManager.mainGameManager.enemyAllMoney) + "G";
        }
        else
        {
            if (NetworkMaster.Instance.endState == 1)
            {
                redTitle.text = "REDÆÀ ÆÐ¹è";
                blueTitle.text = "BLUEÆÀ ½Â¸®";
            }
            else
            {
                redTitle.text = "REDÆÀ ½Â¸®";
                blueTitle.text = "BLUEÆÀ ÆÐ¹è";
            }
            blueUserNameText.text ="ID : "+ SceneVarScript.Instance.GetUserOption("username");
            redUserNameText.text = "ID : " + MainGameManager.mainGameManager.enemyUserName;
            blueMoney.text = MainGameManager.mainGameManager.StringDot((int)MainGameManager.mainGameManager.allMoney) + "G";
            redMoney.text = MainGameManager.mainGameManager.StringDot((int)MainGameManager.mainGameManager.enemyAllMoney) + "G";
        }


        rating.text = "RATING:" + SceneVarScript.Instance.GetUserOption("rating");
        if (NetworkMaster.Instance.endState == 1)
        {
            rewardMoneyText.text = "+" + MainGameManager.mainGameManager.StringDot(MainGameManager.mainGameManager.getRewardMoney);
            addRating.text = "<color=red>(+" + SceneVarScript.Instance.GetWinPoint() + ")</color>";
        }
        else if (NetworkMaster.Instance.endState == 2)
        {
            rewardMoneyText.text ="+"+MainGameManager.mainGameManager.StringDot(MainGameManager.mainGameManager.getRewardMoney);
            addRating.text = "<color=blue>(-" + SceneVarScript.START_RATING_DISCOUNT + ")</color>";
        }
    }
}