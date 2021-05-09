using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class monsterHpbar : MonoBehaviour
{
    public monsterScript Instance;
    SpriteRenderer mySp;
    SpriteRenderer targetsp,parentsp;
    // Start is called before the first frame update
    void Start()
    {
        Instance = GetComponentInParent<monsterScript>();
        targetsp = transform.parent.parent.GetComponent<SpriteRenderer>();
        parentsp = transform.parent.GetComponent<SpriteRenderer>();
        mySp = GetComponent<SpriteRenderer>();
        if (gameObject.tag != "Player")
        {
            mySp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            transform.parent.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        }

    // Update is called once per frame
    void Update()
    {
        
        mySp.sortingLayerName = targetsp.sortingLayerName;
        parentsp.sortingLayerName = targetsp.sortingLayerName;
        if (Instance.hp > Instance.mhp)
        {
            transform.localScale = new Vector3(1, 1, 1);
             Instance.hp =  Instance.mhp;
        }
        else if ( Instance.hp <= 0)
        {
            Instance.hp = 0;
            transform.localScale = new Vector3(0, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x,  Instance.hp /  Instance.mhp, 0.5f), 1, 1);
        }
    }
}
