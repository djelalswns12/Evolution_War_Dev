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
        SetImage();
    }
    public void SetImage()
    {
        int set;
        if (myText.alignment == TextAnchor.MiddleCenter)
        {
            set = 2;
        }else if (myText.alignment == TextAnchor.MiddleLeft)
        {
            set = -1;
        }else if (myText.alignment == TextAnchor.MiddleRight)
        {
            set = 1;
        }
        else
        {
            set = 2;
        }
        target.anchoredPosition = prePos + new Vector3(xOffset + (-myText.preferredWidth / set), yOffset, 0);
    }
}
