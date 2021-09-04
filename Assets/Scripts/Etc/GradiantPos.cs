using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradiantPos : MonoBehaviour
{
    public static GradiantPos Instance;
    public Transform par,root;
    public GameObject mask;
    public SpriteRenderer maskSp,mySp;
    public float offSet;
    public float offSet2;//최소 거리 보장값
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        offSet = 0.1f;
        mySp = GetComponent<SpriteRenderer>();
        maskSp = mask.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MainGameManager.mainGameManager.GetFocus() != null)
        {
            root = MainGameManager.mainGameManager.GetFocus().transform;
            
            
            //root 몬스터가 상대방 플레이어를 못찾았을 경우
            if(root.GetComponent<monsterScript>().size3.x * 1.3f <4.5 )
            {
                offSet2 =4.5f;
            }
            else
            {
                if (root.tag != "Player")
                {
                    offSet2 = (root.GetComponent<monsterScript>().size3.x * 1.3f);
                }
                else
                {
                    offSet2 = 8f;
                }
            }
            //root 몬스터가 상대방 플레이어를 찾았을 경우
            mask.transform.localScale = new Vector2(((NetworkMaster.Instance.dir  ? 1 : -1) *offSet2 +(root.position.x - par.position.x)) / maskSp.size.x, mask.transform.localScale.y);
        }
            mySp.flipX = mask.transform.localScale.x > 0 ? false:true;
    transform.localPosition = new Vector2(
        (
        ((mask.transform.localScale.x > 0 ? 1 : -1) * offSet) +
        (mask.transform.localPosition.x)+
        ((mask.transform.localScale.x>0?1:-1)* maskSp.bounds.size.x)-
        ((mask.transform.localScale.x > 0 ? 1 : -1) * mySp.bounds.size.x/2)
        )
        /par.localScale.x, 0);
    }
}
