using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GroundEffectScript : MonoBehaviourPunCallbacks
{
    public LayerMask whatIsLayer2;
    public Vector2 attOffset, size;
    public int damage;
    public float speed;
    public bool dir;
    SpriteRenderer mySp;
    public float settingSpeed;
    public List<GameObject> hittedMonster;
    public PhotonView pv;
    public int dieMoneyGet;
    public GameObject creator;
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        mySp = GetComponent<SpriteRenderer>();
        mySp.flipX = !dir;
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            MonseterAttack();
        }
        transform.position += Vector3.right*(dir==true?1:-1)*speed*Time.deltaTime;
    }
    public void DestroyObj()
    {
        Destroy(gameObject);
    }
    public void ChangeSpeed()
    {
        speed = settingSpeed;
    }



    public void MonseterAttack()
    {
            Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + attOffset.x, transform.position.y + attOffset.y), size, 0, whatIsLayer2);
            if (hitArea.Length > 0)
            {
            for (int i = 0; i < hitArea.Length; i++)
                {
                    if (hitArea[i].tag != "boss" && !hittedMonster.Contains(hitArea[i].gameObject))
                    {
                        hittedMonster.Add(hitArea[i].gameObject);
                        monsterScript target = hitArea[i].gameObject.GetComponent<monsterScript>();
                        target.pv.RPC("CrowdControl", RpcTarget.All,creator.transform.position,3f,2f);
                        target.pv.RPC("GetDamage", RpcTarget.All, damage,dieMoneyGet, NetworkMaster.Instance.dir);
                    }
                }
            }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + attOffset.x, transform.position.y + attOffset.y), size);
    }
}
