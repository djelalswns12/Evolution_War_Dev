using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldObjectPool : MonoBehaviour
{
    public static GoldObjectPool Instance;
    public GameObject obj;
    Queue<MoneyPos> GoldEffects = new Queue<MoneyPos>();
    public int cnt;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        InitQueue(cnt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitQueue(int cnt)
    {
        for (int i = 0; i < cnt; i++)
        {
            GoldEffects.Enqueue(CreateEffect());
        }
    }
    public MoneyPos Pop(Vector2 pos, int gold)
    {
        if (GoldEffects.Count <= 0)
        {
            InitQueue(50);
        }
        var one = GoldEffects.Dequeue();
        one.Init(pos, gold);
        one.gameObject.SetActive(true);
        one.transform.parent = null;
        return one;
    }
    public void Push(MoneyPos old)
    {
        old.transform.SetParent(transform);
        old.gameObject.SetActive(false);
        GoldEffects.Enqueue(old);
    }
    MoneyPos CreateEffect()
    {
        var one = Instantiate(obj, transform).GetComponent<MoneyPos>();
        one.gameObject.SetActive(false);
        return one;
    }
}
