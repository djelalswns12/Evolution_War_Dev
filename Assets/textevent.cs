using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class textevent : MonoBehaviour
{
    public int alphaState;
    public TextMeshProUGUI text;
    float t;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine("alphaChange1");
    }

    // Update is called once per frame
    void Update()
    {

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
