using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
public class ThrowScript : MonoBehaviourPunCallbacks,IPunObservable
{
    public bool canAttack;
    public int damage;
    public float speed,turnSpeed;
    public Vector2 target;
    public PhotonView pv;
    bool lifeCycle;
    public SpriteRenderer sp;
    Animator anim;
    public bool dir;
    public Vector2 attOffset3, size3;//공격범위에 적있는지 확인
    public LayerMask whatIsLayer2;
    public int lostTarget;
    public int dieMoneyGet;
    public float bonusMoney;
    // Start is called before the first frame update
    void Start()
    {
        lostTarget = 0;
        canAttack = true;
        pv = GetComponent<PhotonView>();
        sp =GetComponentInChildren<SpriteRenderer>();
        sp.flipX = dir;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        sp.flipX = dir;
        //gameObject.GetComponentInChildren<SpriteRenderer>().enabled = lifeCycle;
        if (target != null)
            {
                sp.enabled = true;
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("hit"))
            {
                transform.Rotate(Vector3.forward*(dir==true? -1:1), turnSpeed * Time.deltaTime);
                lostTarget = 1;
                transform.position = Vector2.MoveTowards((Vector2)transform.position, target, speed * Time.deltaTime);
                if (pv.IsMine)
                {
                    if (canAttack == true)
                    {
                        MonsterAttack();
                    }
                    if (Vector2.Distance((Vector2)transform.position, target) <= 0.05f)
                    {
                        pv.RPC("Hitted", RpcTarget.All);
                        //이미 생성자 pc에선 삭제되었음
                    }
                }
            }
            else
            {
                //hit 이라면
                transform.Rotate(Vector3.zero);
                canAttack = false;
            }
        }
        else
        {
            if (lostTarget == 1)
            {
                Destroy(gameObject);
            }
            sp.enabled = false;
        }



    if(anim.GetCurrentAnimatorStateInfo(0).IsName("hit")&& anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f)
        {
            pv.RPC("ThrowDestroy", RpcTarget.All);
        }

    }
    public void MonsterAttack() {
        Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3, 0, whatIsLayer2);
        if (hitArea.Length > 0)
        {
            for (int i = 0; i < hitArea.Length; i++)
            {
                
                monsterScript target = hitArea[i].gameObject.GetComponent<monsterScript>();
                if ((target.dir != this.dir && (target.tag == "monster" || target.tag == "Player")) || target.tag == "boss" || target.tag == "Test")
                {
                    if (hitArea.Length > 2 && hitArea[i].tag == "Player")
                    {
                        continue;
                        //플레이어 공격중에 캐릭터 생성된다면 공격타겟을 바꿔줘야하기 때문
                    }
                    canAttack = false;
                    pv.RPC("Hitted", RpcTarget.All); // 애니메이션 변경
                    if ((int)bonusMoney > 0)
                    {
                        MainGameManager.mainGameManager.CreatGoldEffect(transform.position,(int)bonusMoney);
                    }
                    target.pv.RPC("GetDamage", RpcTarget.All, damage,dieMoneyGet, NetworkMaster.Instance.dir); //데미지 삽입
                  
                    return;
                }
            }
        }
    }
    [PunRPC]
    public void ThrowDestroy()
    {
        Destroy(gameObject);
    }
    
    [PunRPC]
    public void Hitted()
    {
        turnSpeed = 0;
        transform.rotation = Quaternion.Euler(0,0,0);
        anim.SetBool("hit", true);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(target);
            stream.SendNext(dir);
        }
        else
        {
            target = (Vector2)stream.ReceiveNext();
            dir = (bool)stream.ReceiveNext();
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + (attOffset3.x * (dir == true ? -1 : 1)), transform.position.y + attOffset3.y), size3);
    }
}

