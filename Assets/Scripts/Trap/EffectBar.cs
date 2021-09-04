using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectBar : MonoBehaviour
{
    SpriteRenderer mySp;
    public SpriteRenderer targetSp;
    SpriteRenderer[] childerenSp;
    // Start is called before the first frame update
    void Start()
    {
        
        //targetSp = transform.parent.GetComponent<SpriteRenderer>();
        childerenSp = transform.GetComponentsInChildren<SpriteRenderer>(true);
        mySp = GetComponent<SpriteRenderer>();
        if (gameObject.tag != "Player")
        {
            mySp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            foreach (var item in childerenSp)
            {
              item.maskInteraction= SpriteMaskInteraction.VisibleInsideMask;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        mySp.sortingLayerName = targetSp.sortingLayerName;
        foreach(var item in childerenSp)
        {
            item.sortingLayerName = targetSp.sortingLayerName;
        }
    }
}
