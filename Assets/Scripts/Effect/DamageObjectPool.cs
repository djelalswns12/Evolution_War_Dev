using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObjectPool : MonoBehaviour
{
    public static DamageObjectPool Instacne;
    // Start is called before the first frame update
    public GameObject obj;
    Queue<DamageEffect> damageEffects=new Queue<DamageEffect>();
    public int cnt;
    private void Awake()
    {
        Instacne = this;
       
    }
    void Start()
    {
        InitQueue(cnt);
    }
    private void InitQueue(int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            damageEffects.Enqueue(CreateEffect());
        }
    }
    public DamageEffect Pop(Vector2 pos,int damage,Color color)
    {
        if (damageEffects.Count <= 0)
        {
            InitQueue(50);
            //미리 생성해놓은 오브젝트 갯수가 부족하다면
            //추가로 생성해준다. 
        }
        var one = damageEffects.Dequeue();
        one.Init(pos,damage,color);
        //데미지 이펙트의 세부 데이터 설정

        one.gameObject.SetActive(true);
        one.transform.parent = null;
        //풀링을 담당하는 오브젝트와의 상속에서 벗어나
        //root 로 이동한다.
        return one;
    }
    public DamageEffect TickPop(Vector2 pos, int damage, Color color)
    {
        if (damageEffects.Count <= 0)
        {
            InitQueue(50);
        }
        var one = damageEffects.Dequeue();
        one.TickInit(pos, damage, color);
        one.gameObject.SetActive(true);
        one.transform.parent = null;
        return one;
    }
    public void Push(DamageEffect old)
    {
        old.transform.SetParent(transform);
        //폴링을 담당하는 오브젝트의 자식으로 설정한다.
        old.gameObject.SetActive(false);
        damageEffects.Enqueue(old);
    }
    DamageEffect CreateEffect()
    {
        var one = Instantiate(obj,transform).GetComponent<DamageEffect>();
        one.gameObject.SetActive(false);
        return one;
    }
}
