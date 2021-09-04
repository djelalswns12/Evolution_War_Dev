using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextExpress : MonoBehaviour
{
    public float offset;
    public GameObject textObj;
    public GameObject textMainObj;
    LinkedList<GameObject> textList;

    public Image mainTextImg;
    public Text mainText;
    Coroutine mainTextDisapear;
    bool isDisapearing;

    public Color preMainTextColor, preMainImgColor;
    // Start is called before the first frame update
    void Start()
    {
        textList= new LinkedList<GameObject>();
        preMainTextColor = mainText.color;
        preMainImgColor= mainTextImg.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (textList.Count > 0)
        {
            int pos = 0;
            foreach(GameObject item in textList)
            {
                if (item.GetComponent<Text>().color.a < 0)
                {
                    Destroy(textList.Last.Value);
                    textList.RemoveLast();
                    break;
                }
                item.GetComponent<RectTransform>().localPosition = new Vector3(0, offset+pos, 0);
                pos += 65;
            }
        }
        if (textMainObj.activeSelf == true)
        {
            if (isDisapearing == false)
            {
                isDisapearing = true;
                mainTextDisapear = StartCoroutine(DisapearMainText());
            }
        }
    }
    IEnumerator DisapearMainText()
    {
        var newColor = mainText.color;
        var newColor2 = mainTextImg.color;
        yield return new WaitForSeconds(3.5f);
        while (true) {
            newColor.a -= 0.1f;
            mainText.color = newColor;

            newColor2.a -= 0.1f;
            mainTextImg.color = newColor2;

            if (newColor.a <= 0 || newColor2.a <= 0)
            {
                textMainObj.SetActive(false);
                yield return null;
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void setNewText(string s)
    {
        GameObject newText=GameObject.Instantiate(textObj);
        newText.transform.SetParent(transform,false);
        newText.GetComponent<Text>().text = s;
        Debug.Log(s);
        if (textList.Count > 3)
        {
            Destroy(textList.Last.Value);
            textList.RemoveLast();
        }
        textList.AddFirst(newText);
    }
    public void setMainText(string s)
    {
        if (mainTextDisapear != null)
        {
            StopCoroutine(mainTextDisapear);
        }
        textMainObj.SetActive(true);
        mainText.color = preMainTextColor;

        mainTextImg.color = preMainImgColor;

        mainText.text = s;
        isDisapearing = false;
    }
}
