using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleLayer : MonoBehaviour
{
    //Ʈ����(������,�͵�) ǥ�õ� ��ƼŬ
    public SpriteRenderer parSp;
    public ParticleSystemRenderer[] rend;
    // Start is called before the first frame update
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("----------------------------------------" + gameObject.name);
        Debug.Log("----------------------------------------" + gameObject.name);
        Debug.Log("----------------------------------------" + gameObject.name);
        SetRender();
    }
    public void SetRender()
    {
        for (int i = 0; i < rend.Length; i++)
        {
            rend[i].sortingLayerName = parSp.sortingLayerName;
            rend[i].sortingOrder = parSp.sortingOrder + 1;
        }
    }
}
