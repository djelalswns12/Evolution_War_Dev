using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class textevent : MonoBehaviour
{
    public int alphaState;
    public Text text;
    float t;
    Coroutine ColorChange;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ColorChange == null)
        {
            StartCoroutine(alphaChange1());
        }
    }
    IEnumerator alphaChange1()
    {
        for (; ; )
        {
            for (float f = 1f; f >= 0.2; f -= 1f * Time.deltaTime)
            {
                Color c = text.color;
                c.a = f;
                text.color = c;
                yield return null;
            }
            for (float f = 0.2f; f <= 1; f += 1f * Time.deltaTime)
            {
                Color c = text.color;
                c.a = f;
                text.color = c;
                yield return null;
            }
        }
    }
}
