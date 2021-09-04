using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialObject : MonoBehaviour
{
    public Image dir,back;
    public Button xBtn;
    Color startColor;
    public float power;
    public float minAlpha,maxAlpha;
    // Start is called before the first frame update
    void Start()
    {
        dir.gameObject.SetActive(false);
        back.gameObject.SetActive(false);
        xBtn.onClick.AddListener(CloseTutorial);
        if (NetworkMaster.Instance.dir == true)
        {
            dir.gameObject.SetActive(true);
            StartCoroutine(DirColorChange(dir, dir.color));
        }
        else
        {
            back.gameObject.SetActive(true);
            StartCoroutine(DirColorChange(back, back.color));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator DirColorChange(Image obj,Color startColor)
    {
        var newColor = startColor;
        while (true)
        {
            while (obj.color.a >= minAlpha/100)
            {
                newColor.a -= power * Time.deltaTime;
                obj.color = newColor;
                yield return null;
            }
            while (obj.color.a <= maxAlpha/100)
            {
                newColor.a += power * Time.deltaTime;
                obj.color = newColor;
                yield return null;
            }
        }
    }
    public void CloseTutorial()
    {
        gameObject.SetActive(false);
    }
}
