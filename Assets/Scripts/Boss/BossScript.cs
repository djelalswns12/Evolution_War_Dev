using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;

public class BossScript : MonoBehaviourPunCallbacks, IPunObservable
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
        if (SceneVarScript.Instance.GetOptionByName(monster.myName, "touchDropGold", SceneVarScript.Instance.bossOption) != "null")
        {
            bossDropGold = int.Parse(SceneVarScript.Instance.GetOptionByName(monster.myName, "touchDropGold", SceneVarScript.Instance.bossOption));
        }
        else
        {
            bossDropGold = 0;
        }
        MainGameManager.mainGameManager.SetNowBoss(gameObject);

        if (MainGameManager.mainGameManager.GetCanTouchAttack())
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (TouchUI() == true)
                {
                    TouchObj();
                }
            }
        }

        if (pv.IsMine)
        {
            MonsterAttack();
        }
    }
    private bool TouchObj()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ray = new Ray2D(mousePos, Vector2.zero);
        foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction))
        {
            if (hit.transform.gameObject == gameObject)
            {
                Instantiate(touchEffectObj, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                MainGameManager.mainGameManager.CreatGoldEffect(mousePos, bossDropGold + MainGameManager.mainGameManager.GetTouchDropGold());
                monster.RpcCallGetDamage(MainGameManager.mainGameManager.GetTouchDamge(), dieMoneyGet, NetworkMaster.Instance.dir, 0, mousePos.x, mousePos.y);
                StartCoroutine(colorCo());
                MainGameManager.mainGameManager.CoolTouchAttack();
                return true;
            }
        }
        return false;
    }
    public bool TouchUI()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        if (results.Count > 0)
        {
            if (results.Count == 1)
            {
                if (results[0].gameObject != MainGameManager.mainGameManager.moveUI)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }
    public void SpawnGroundEffect()
    {
        SpawnAttack(true);
        SpawnAttack(false);
    }
    public void SpawnSingleGroundEffect()
    {
        SpawnAttack(monster.GetTargetPlayer());
    }
    public void SpawnAttack(bool target)
    {
        Vector3 groundEffPos = transform.position + Vector3.right * 1.5f * (target == true ? -1f : 1f);
        GroundEffectScript script;
        GameObject GroundEffect = GameObject.Instantiate(Effect1, groundEffPos, Quaternion.identity);
        script = GroundEffect.GetComponent<GroundEffectScript>();
        script.dir = !target;
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
