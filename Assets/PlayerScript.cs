using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerScript : MonoBehaviourPunCallbacks,IPunObservable
{
    // Start is called before the first frame update
    public int moveStautu;

    public float speed;
    public PhotonView pv;
    public bool myplayer = false;
    public SpriteRenderer sp;
    public bool dir;

    public Rigidbody2D rigid;

    public bool right, left;
    void Start()
    {
 
        right = false;
        left = false;
        myplayer = pv.IsMine;

        //if (myplayer)
        //    sp.color = new Color(0, 1, 0);
        //else
        //    sp.color = new Color(1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        sp.flipX = dir;
        if (myplayer == false)
        {
            NetworkMaster.otherPlayer = gameObject;
            NetworkMaster.Instance.otherPlayerHasBeen = true;
            return;
        }
        //아래부터 IsMine 이라면

        if (moveStautu == 0)
        {
            if (right)
                Rightfunc();

            if (left)
                Leftfunc();
        }

    }
    public void Rightfunc()
    {
        rigid.velocity = Vector2.right*speed;

    }
    public void Leftfunc()
    {
        rigid.velocity = Vector2.left*speed;
    }
    public void MoveStop()
    {
        if (moveStautu == 0)
        {
            left = false;
            right = false;
            rigid.velocity = Vector2.zero;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(dir);
        }
        else
        {
            dir = (bool)stream.ReceiveNext();
            //float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            //Debug.Log(lag);
        }
    }
}
