using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetImage : MonoBehaviour
{
    Image myImage;
    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        var nowPlayerName = NetworkMaster.player.GetComponent<monsterScript>().myName;
        myImage.sprite = MainGameManager.mainGameManager.buildIconList[int.Parse(NetworkMaster.Instance.GetMonsterOption(nowPlayerName, "icon")) - 3000];
    }
}
