using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankManager : ScrollAddManager
{
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (SceneVarScript.Instance.usersReset == false)
        {
            Clear();
            data = SceneVarScript.Instance.usersOption;
            PushElements();
            SceneVarScript.Instance.usersReset = true;
        }
    }
    private void Start()
    {
        data = SceneVarScript.Instance.usersOption;
        PushElements();
    }
    protected override void SetData(GameObject selectObj,int index)
    {
        RankChart chart = selectObj.GetComponent<RankChart>();
        chart.RankAndName.text = "<color=yellow>" + (data.Length-index) + " µî</color> "+ data[index]["username"].ToString();
        chart.WinLoseRating.text = data[index]["win"].ToString()+"½Â "+data[index]["lose"].ToString()+"ÆÐ "+"<color=yellow>"+data[index]["rating"].ToString()+"P</color>";
        //chart.WinLoseRating
    }
}
