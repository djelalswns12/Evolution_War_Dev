using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextDisapear : MonoBehaviour
{
    public bool disapear;
    public Text myText;
    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<Text>();
        myText.raycastTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (disapear == false)
        {
            disapear = true;
            StartCoroutine(DecreaseAlpha());
        }
    }

    IEnumerator DecreaseAlpha()
    {
        yield return new WaitForSeconds(1.2f);
        while (myText.color.a > 0)
        {
            var co = myText.color;
            co.a -= 0.05f;
            myText.color= co;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
