using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updwonEvent : MonoBehaviour
{
    public float speed;
    public float deg;
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        StartCoroutine("upDown");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator upDown()
    {
        for (; ; )
        {
            for (float f = 1f; f >= 0.2; f -= speed * Time.deltaTime)
            {
                Vector3 tmp = rect.transform.position;
                tmp.y -= deg;
                rect.transform.position = tmp;
                yield return null;
            }
            for (float f = 0.2f; f <= 1; f += speed * Time.deltaTime)
            {
                Vector3 tmp = rect.transform.position;
                tmp.y += deg; 
                rect.transform.position = tmp;
                yield return null;
            }
        }
    }
}
