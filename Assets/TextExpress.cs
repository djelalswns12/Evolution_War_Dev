using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TextExpress : MonoBehaviour
{
    public float offset;
    public GameObject textObj;
    LinkedList<GameObject> textList;
    // Start is called before the first frame update
    void Start()
    {
        textList= new LinkedList<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textList.Count > 0)
        {
            int pos = 0;
            foreach(GameObject item in textList)
            {
                if (item.GetComponent<TextMeshProUGUI>().alpha < 0)
                {
                    textList.RemoveLast();
                    break;
                }
                item.GetComponent<RectTransform>().localPosition = new Vector3(0, offset+pos, 0);
                pos += 65;
            }
        }
    }
    public void setNewText(string s)
    {
        GameObject newText=GameObject.Instantiate(textObj);
        newText.transform.SetParent(transform,false);
        newText.GetComponent<TextMeshProUGUI>().text = s;
        Debug.Log(s);
        textList.AddFirst(newText);
    }


}
