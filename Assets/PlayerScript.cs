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
    void Start()
    {
        myplayer = pv.IsMine;

        //if (myplayer)
        //    sp.color = new Color(0, 1, 0);
        //else
        //    sp.color = new Color(1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < buliding.Length; i++)
        {
            if (playerSp == i)
            {
                buliding[i].SetActive(true);
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
            return;
        }
        //아래부터 IsMine 이라면
        playerSp = MainGameManager.mainGameManager.GetPlayerBuliding();


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
        }
        else
        {
            dir = (bool)stream.ReceiveNext();
            playerSp = (int)stream.ReceiveNext();
            //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            //Debug.Log(lag);
        }
    }
}
