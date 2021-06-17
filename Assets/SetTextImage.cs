using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetTextImage : MonoBehaviour
{
    public Text myText;
    public RectTransform target;
    public float xOffset,yOffset;
    public Vector3 prePos;
    // Start is called before the first frame update
    void Start()
    {
        prePos = myText.GetComponent<RectTransform>().anchoredPosition;
        myText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        target.anchoredPosition = prePos+ new Vector3(xOffset+(-myText.preferredWidth / 2), yOffset, 0);
    }
}
