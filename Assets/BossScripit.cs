using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class BossScripit : MonoBehaviourPunCallbacks,IPunObservable
{
    public GameObject touchEffectObj;
    public GameObject Effect1;
    public PhotonView pv;
    public int bossDropGold; 
    public int dieMoneyGet;
    Color orginColor = new Color(1, 1, 1);
    Color redColor = new Color(1, 0, 0);
    // Start is called before the first frame update
    Ray2D ray;

    monsterScript monster;
    void Start()
    {
        monster = GetComponent<monsterScript>();

    }
    IEnumerator colorCo()
    {
        monster.sp.color = redColor;
        yield return new WaitForSeconds(0.04f);
        monster.sp.color = orginColor;
        yield return new WaitForSeconds(0.04f);
        monster.sp.color = redColor;
        yield return new WaitForSeconds(0.04f);
        monster.sp.color = orginColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction))
            {
                if (hit.transform.gameObject == gameObject)
                {
                    Instantiate(touchEffectObj, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                    MainGameManager.mainGameManager.CreatGoldEffect(Camera.main.ScreenToWorldPoint(Input.mousePosition), bossDropGold);
                    monster.pv.RPC("GetDamage", RpcTarget.All, MainGameManager.mainGameManager.GetTouchDamge(),dieMoneyGet,NetworkMaster.Instance.dir);
                    StartCoroutine(colorCo());
                }
            }
        }
     }
    public void SpawnGroundEffect()
    {
        GroundEffectScript script;
        GameObject LeftGroundEffect = GameObject.Instantiate(Effect1, transform.position + Vector3.left*1.5f, Quaternion.identity);
        script = LeftGroundEffect.GetComponent<GroundEffectScript>();
        script.dir = false;
        script.whatIsLayer2 = monster.whatIsLayer2;
        script.damage = monster.damage;
        script.dieMoneyGet = 1;
        script.creator=gameObject;
        GameObject RightGroundEffect = GameObject.Instantiate(Effect1, transform.position + Vector3.right*1.5f, Quaternion.identity);
        script = RightGroundEffect.GetComponent<GroundEffectScript>();
        script.dir = true;
        script.whatIsLayer2 = monster.whatIsLayer2;
        script.damage = monster.damage;
        script.dieMoneyGet = 1;
        script.creator=gameObject;
    }

    [PunRPC]
    void BossHit(int damage)
    {
        if (PhotonNetwork.IsMasterClient)
            monster.hp -= damage;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(bossDropGold);
        }
        else
        {
            bossDropGold = (int)stream.ReceiveNext();
        }
    }
}
