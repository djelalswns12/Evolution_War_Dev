using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class creatImgScript : MonoBehaviour
{
    public GameObject redLine,pannel;
    public LayerMask targetLayer;
    Vector2 redLinepos;
    Image myImg, pannelImage;
    public int redLineCreateLayer;
    // Start is called before the first frame update
    void Start()
    {
        myImg = GetComponent<Image>();
        pannelImage = pannel.GetComponent<Image>();
        myImg.enabled = false;
        redLineCreateLayer = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (myImg.enabled == true)
        {
            transform.position = Input.mousePosition;
            Vector2 pos=Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D[] hitArea = Physics2D.OverlapBoxAll(pos,new Vector2(1,1),0,targetLayer);
            if (hitArea.Length > 0)
            {
                for (int i = hitArea.Length - 1; i >= 0; i--)
                {
                    pannelImage.color = new Color(1, 1, 1);
                    if (hitArea[i].gameObject.layer == LayerMask.NameToLayer("upground"))
                    {
                        NetworkMaster.Instance.SetLayer(1);
                        redLineCreateLayer = 2;
                        redLinepos = redLine.transform.position;
                        redLinepos.y = -0.5f;
                        redLine.transform.position = redLinepos;
                        break;
                    }
                    else if (hitArea[i].gameObject.layer == LayerMask.NameToLayer("downground"))
                    {
                        NetworkMaster.Instance.SetLayer(0);
                        redLineCreateLayer = 1;
                        redLinepos = redLine.transform.position;
                        redLinepos.y = -2f;
                        redLine.transform.position = redLinepos;
                        break;
                    }
                    else if (hitArea[i].gameObject.layer == LayerMask.NameToLayer("uiground"))
                    {
                        redLineCreateLayer = 0;
                        pannelImage.color = new Color(0.9f, 0.65f, 0.65f);
                        redLinepos = redLine.transform.position;
                        redLinepos.y = -1000f;
                        redLine.transform.position = redLinepos;
                        break;
                    }
                }
            }
            else
            {
                redLineCreateLayer = -1;
                pannelImage.color = new Color(1, 1, 1);
                redLinepos = redLine.transform.position;
                redLinepos.y = -1000f;
                redLine.transform.position = redLinepos;
            }
        }
        else
        {
            redLineCreateLayer = -1;
            pannelImage.color = new Color(1, 1, 1);
            redLinepos = redLine.transform.position;
            redLinepos.y = -1000f;
            redLine.transform.position = redLinepos;
        }


    }
}
