using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrapTouch : MonoBehaviour
{
    public monsterScript monsterScript;
    public GameObject touchEffectObj;
    public SpriteRenderer sp;
    Color orginColor = new Color(1, 1, 1);
    Color redColor = new Color(1, 0, 0);
    SetPopUP popUpMaster;
    // Start is called before the first frame update
    void Start()
    {
        popUpMaster = MainGameManager.mainGameManager.popUpMaster;
    }

    // Update is called once per frame
    void Update()
    {
        AttackTrap();
    }
    IEnumerator colorCo()
    {
        sp.color = redColor;
        yield return new WaitForSeconds(0.04f);
        sp.color = orginColor;
        yield return new WaitForSeconds(0.04f);
        sp.color = redColor;
        yield return new WaitForSeconds(0.04f);
        sp.color = orginColor;
    }
    public void AttackTrap()
    {
        if (!monsterScript.pv.IsMine)
        {
            //if (Input.touchCount > 0)
            //{
            //    if (Input.GetTouch(0).phase == TouchPhase.Began)
            //    {
            //        Ray2D ray = new Ray2D(Input.GetTouch(0).position, Vector2.zero);
            //        foreach (RaycastHit2D item in Physics2D.RaycastAll(ray.origin, ray.direction))
            //        {
            //            if (item.transform.gameObject == gameObject)
            //            {
            //                StartCoroutine(colorCo());
            //                Instantiate(touchEffectObj, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            //                monsterScript.RpcCallGetDamage(MainGameManager.mainGameManager.GetTouchDamge(), 0, NetworkMaster.Instance.dir);
            //            }
            //        }
            //    }
            //}
            if (true/*Application.isEditor*/)
            {
                if (MainGameManager.mainGameManager.GetCanTouchAttack())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        foreach (RaycastHit2D item in Physics2D.RaycastAll(ray.origin, ray.direction))
                        {
                            if (item.transform.gameObject == gameObject)
                            {
                                if (NetworkMaster.Instance.dir == true)
                                {
                                    if (gameObject.transform.position.x > GradiantPos.Instance.transform.position.x)
                                    {
                                        Debug.Log("시야 밖이라 공격할 수 없습니다.");
                                        return;
                                    }
                                }
                                else
                                {
                                    if (gameObject.transform.position.x < GradiantPos.Instance.transform.position.x)
                                    {
                                        Debug.Log("시야 밖이라 공격할 수 없습니다.");
                                        return;
                                    }
                                }
                                StartCoroutine(colorCo());
                                Instantiate(touchEffectObj, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                                monsterScript.RpcCallGetDamage(MainGameManager.mainGameManager.GetTouchDamge(), 0, NetworkMaster.Instance.dir);
                                MainGameManager.mainGameManager.CoolTouchAttack();
                                break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                foreach (RaycastHit2D hit in Physics2D.RaycastAll(ray.origin, ray.direction))
                {
                    if (hit.transform.gameObject == gameObject)
                    {
                        if (MainGameManager.mainGameManager.GetNowMonster()!=gameObject)
                        {
                            MainGameManager.mainGameManager.SetNowMonster(gameObject);
                            popUpMaster.RefreshPopUpContents(4);
                            break;
                        }
                        else
                        {
                            MainGameManager.mainGameManager.SetNowMonster(null);
                            popUpMaster.UpPopUp(4);
                        }
                    }
                }
            }
        }
    }
}
