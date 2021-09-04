using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlindManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static BlindManager Instance;
    public Image blind;
    public float power;
    private Coroutine on;
    private Coroutine off;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnBlind()
    {
        if (on == null)
        {
            on = StartCoroutine(OnblindFunc());
        }
        else
        {
            if (off != null)
            {
                StopCoroutine(off);
            }
            StopCoroutine(on);
            on = StartCoroutine(OnblindFunc());
        }
    }
    public void CloseBlind()
    {
        if (off == null)
        {
            off = StartCoroutine(CloseblindFunc());
        }
        else
        {
            if (on != null)
            {
                StopCoroutine(on);
            }
            StopCoroutine(off);
            off = StartCoroutine(CloseblindFunc());
        }
    }
    IEnumerator OnblindFunc()
    {
        Color one = blind.color;
        one.a = 0;
        while (one.a<1) {
            blind.color = one;
            one.a += power * Time.deltaTime;
            yield return null;
        }
        on = null;
    }
    IEnumerator CloseblindFunc()
    {
        Color one = blind.color;
        one.a = 1;
        while (one.a > 0)
        {
            blind.color = one;
            one.a -= power * Time.deltaTime;
            yield return null;
        }
        off = null;
    }
}
