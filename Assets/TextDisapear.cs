using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextDisapear : MonoBehaviour
{
    public bool disapear;
    public TextMeshProUGUI myText;
    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();
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
        while (myText.alpha > 0)
        {
            myText.alpha -= 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
