using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerScript : MonoBehaviourPunCallbacks,IPunObservable
{
    // Start is called before the first frame update
    public int playerSp;

    public PhotonView pv;
    public bool myplayer = false;
    public SpriteRenderer sp;
    public bool dir;

    public Rigidbody2D rigid;

    public GameObject[] buliding;
    public float allMoney;
    public string userName;
    public string[] UseSKill;
    void Start()
    {
        UseSKill = new string[SceneVarScript.MAX_SKILL_COUNT];
        myplayer = pv.IsMine;

        //if (myplayer)
        //    sp.color = new Color(0, 1, 0);
        //else
        //    sp.color = new Color(1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < buliding.Length; i++)
        {
            if (playerSp == i)
            {
                buliding[i].SetActive(true);
                if (myplayer == false)
                {
                    buliding[i].GetComponent<SpriteOutline>().enabled=true;
                }
            }
            else
            {
                buliding[i].SetActive(false);
            }
        }
        sp.flipX = dir;
        if (myplayer == false)
        {
            NetworkMaster.otherPlayer = gameObject;
            NetworkMaster.Instance.otherPlayerHasBeen = true;
            MainGameManager.mainGameManager.enemyUseSkill = UseSKill;
            MainGameManager.mainGameManager.enemyAllMoney = allMoney;
            MainGameManager.mainGameManager.enemyUserName = userName;
            return;
        }
        //아래부터 IsMine 이라면
        userName = SceneVarScript.Instance.GetUserOption("username");
        allMoney = MainGameManager.mainGameManager.allMoney;
        playerSp = MainGameManager.mainGameManager.GetPlayerBuliding();
        for (int i = 0; i < SceneVarScript.MAX_SKILL_COUNT; i++)
        {
            UseSKill[i] = SceneVarScript.Instance.GetOptionByIndex(SceneVarScript.Instance.GetUserOption("skill" + (i + 1)), "name", SceneVarScript.Instance.skillOption);
        }
        MainGameManager.mainGameManager.useSkill = UseSKill;
    }
    public void SetPlayerSp()
    {
        //네트워크 마스터로 부터 플레이어 건물 상태 가져와서 해당스크립트 변수에 할당에서 공유

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(dir);
            stream.SendNext(playerSp);
            stream.SendNext(UseSKill);
            stream.SendNext(allMoney);
            stream.SendNext(userName);
        }
        else
        {
            dir = (bool)stream.ReceiveNext();
            playerSp = (int)stream.ReceiveNext();
            UseSKill = (string[])stream.ReceiveNext();
            allMoney = (float)stream.ReceiveNext();
            userName = (string)stream.ReceiveNext();

            //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            //Debug.Log(lag);
        }
    }
}
