using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipspByTargetsp : MonoBehaviour
{
    Vector2 pos;
    SpriteRenderer mySp;
    public SpriteRenderer targetsp;
    public float SetX;
    public bool flip;
    // Start is called before the first frame update
    void Start()
    {
        SetX = transform.localPosition.x;
        targetsp = transform.parent.GetComponent<SpriteRenderer>();
        mySp = GetComponent<SpriteRenderer>();
        mySp.sortingLayerName = targetsp.sortingLayerName;
        if (gameObject.tag != "Player")
        {
            mySp.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }

    // Update is called once per frame
    void Update()
    {
        mySp.sortingLayerName = targetsp.sortingLayerName;
        mySp.flipX =flip==true? targetsp.flipX : !targetsp.flipX;
        pos = transform.localPosition;
        pos.x = mySp.flipX == true ? -SetX : SetX;
        transform.localPosition = pos;
    }
}
