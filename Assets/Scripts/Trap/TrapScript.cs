using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour
{
    public int level;
    public float addMoney;
    protected monsterScript monster;
    public string myName;

    public float setCool;
    public float nowCool;

    public bool isLoad;
    public GameObject bar;
    protected Animator anim;
    WaitForSeconds oneSec = new WaitForSeconds(1f);
    // Start is called before the first frame update
    void Start()
    {
        isLoad = false;
        anim = GetComponent<Animator>();
        monster = GetComponent<monsterScript>();
    }

    // Update is called once per frame
    void Update()
    {
        myName = monster.myName;
        BarRendering();
        CoolManage();
        OntheBoss();
        if (isLoad == true && monster.pv.IsMine==true)
        {
            UseSkill();
        }
    }
    //�� �ֱ� ���� ��� ȹ�� �Լ�
    public void VersionRendering()
    {

    }
    IEnumerator OntheBoss()
    {
        while (true)
        {
            Collider2D[] hit2 = Physics2D.OverlapBoxAll(new Vector2(transform.position.x + (monster.attOffset3.x * (monster.dir == true ? -1 : 1)), transform.position.y + monster.attOffset3.y), monster.size3, 0, monster.whatIsLayer2);
            for (int i = 0; i < hit2.Length; i++)
            {
                if (hit2[i].tag == "boss")
                {
                    monster.RpcCallGetDamage((int)Mathf.Ceil(monster.mhp * 0.2f), 1, NetworkMaster.Instance.dir);
                }
            }
            yield return oneSec;
        }
    }
    private void CoolManage()
    {
        setCool = float.Parse(SceneVarScript.Instance.GetOptionByName(myName, "perTime", SceneVarScript.Instance.trapOption));
        //Ʈ�� DB�� ���� �ش� Ʈ���� �̸��� Key������ �ؼ� ȿ�� �ߵ��� �Ҹ�Ǵ� ��Ÿ�Ӱ��� �����´�.
        nowCool += Time.deltaTime;
        isLoad = true;
    }
    private void BarRendering()
    {
        if (nowCool > setCool)
        {
            bar.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (nowCool <= 0)
        {
            bar.transform.localScale = new Vector3(0, 1, 1);
        }
        else
        {
            bar.transform.localScale = new Vector3(Mathf.Lerp(bar.transform.localScale.x, nowCool / setCool, 0.5f), 1, 1);
        }
    }
    protected virtual void UseSkill()
    {
        if (setCool <= nowCool)
        {
            //���� ȿ�� �ڽ� Ŭ�������� ����
        }
    }
}
