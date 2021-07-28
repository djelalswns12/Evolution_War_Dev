using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossScripit : MonoBehaviourPunCallbacks,IPunObservable
{
    public GameObject touchEffectObj;
    public GameObject Effect1;
    public PhotonView pv;
    public int bossDropGold; 
    public int dieMoneyGet;
    Color orginColor = new Color(1, 1, 1);
    Color redColor = new Color(1, 0, 0);
    public List<GameObject> hittedList;
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
        bossDropGold =int.Parse(SceneVarScript.Instance.GetOptionByName(monster.myName, "touchDropGold", SceneVarScript.Instance.bossOption));
        MainGameManager.mainGameManager.SetNowBoss(gameObject);
        if (MainGameManager.mainGameManager.GetCanTouchAttack())
        {
            if (Input.GetMouseButtonDown(0))
            {
                PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

                eventDataCurrentPosition.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
                if (results.Count >0)
                {
                    if (results.Count == 1)
                    {
                        if (results[0].gameObject != MainGameManager.mainGameManager.moveUI)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                


                ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction))
                {
                    if (hit.transform.gameObject == gameObject)
                    {
                        Instantiate(touchEffectObj, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                        MainGameManager.mainGameManager.CreatGoldEffect(Camera.main.ScreenToWorldPoint(Input.mousePosition), bossDropGold + MainGameManager.mainGameManager.GetTouchDropGold());
                        monster.RpcCallGetDamage(MainGameManager.mainGameManager.GetTouchDamge(), dieMoneyGet, NetworkMaster.Instance.dir);
                        StartCoroutine(colorCo());
                        MainGameManager.mainGameManager.CoolTouchAttack();
                        break;
                    }
                }
            }
        }
        if (pv.IsMine)
        {
            MonsterAttack();
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
    public void SpawnSingleGroundEffect()
    {
        Vector3 groundEffPos = transform.position + Vector3.right * 1.5f*(monster.GetTargetPlayer()==true?1f:-1f);
        GroundEffectScript script;
        GameObject GroundEffect = GameObject.Instantiate(Effect1, groundEffPos, Quaternion.identity);
        script = GroundEffect.GetComponent<GroundEffectScript>();
        script.dir = monster.GetTargetPlayer();
        script.whatIsLayer2 = monster.whatIsLayer2;
        script.damage = monster.damage;
        script.dieMoneyGet = 1;
        script.creator = gameObject;
    }
    public void MonsterAttack()
    {
        Collider2D[] hitArea = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + monster.attOffset3.x, transform.position.y + monster.attOffset3.y), monster.size3, 0, monster.whatIsLayer2);
        if (hitArea.Length > 0)
        {
            for (int i = 0; i < hitArea.Length; i++)
            {
                if (hitArea[i].tag != "boss" && !hittedList.Contains(hitArea[i].gameObject))
                {
                    hittedList.Add(hitArea[i].gameObject);
                    monsterScript target = hitArea[i].gameObject.GetComponent<monsterScript>();
                    float crowdDir = (transform.position.x - target.transform.position.x);
                    target.pv.RPC("CrowdControl", RpcTarget.All, (crowdDir > 0 ? -1 : 1), 5f, 4f);
                    target.RpcCallGetDamage(monster.damage, dieMoneyGet, NetworkMaster.Instance.dir);
                }
            }
        }
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
