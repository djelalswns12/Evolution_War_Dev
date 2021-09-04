using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class monsterHpbar : MonoBehaviour
{
    public monsterScript Instance;
    public Text myText;
    SpriteRenderer mySp;
    SpriteRenderer targetsp,parentsp;
    // Start is called before the first frame update
    void Start()
    {
        myText = transform.parent.GetComponentInChildren<Text>();
        Instance = GetComponentInParent<monsterScript>();
        targetsp = transform.parent.parent.GetComponent<SpriteRenderer>();
        parentsp = transform.parent.GetComponent<SpriteRenderer>();
        mySp = GetComponent<SpriteRenderer>();
        if (Instance.gameObject.tag != "Player")
        {
            mySp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            transform.parent.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
        }

    // Update is called once per frame
    void Update()
    {
        if (myText != null)
        {
            myText.text = Instance.hp + "/" + Instance.mhp;
        }
        mySp.sortingLayerName = targetsp.sortingLayerName;
        parentsp.sortingLayerName = targetsp.sortingLayerName;
        if (Instance.hp > Instance.mhp)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if ( Instance.hp <= 0)
        {
            transform.localScale = new Vector3(0, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Lerp(transform.localScale.x,  Instance.hp /  Instance.mhp, 0.5f), 1, 1);
        }
    }
}
