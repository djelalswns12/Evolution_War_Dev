using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPos : MonoBehaviour
{
    float gra;
    public float setGravity,jumpPower,scalePower;
    public int gotGold;
    public Text txt;
    public float tarX, tarY,speed,addSpeed;
    float width,height;
    Vector2 startPos,targetPos,pos,movePos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        targetPos = Camera.main.ScreenToWorldPoint(new Vector2(tarX, tarY));
        gra = jumpPower;
        MainGameManager.mainGameManager.CountMoney(gotGold);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0,gra,0)*Time.deltaTime;
        gra -= setGravity*Time.deltaTime;
        if (gra < 0)
        {
            transform.localScale *=1-(scalePower * Time.deltaTime);
             if(transform.localScale.x < 0.1)
                {
                    Destroy(gameObject);
                } 
        }
        /*
        width = targetPos.x - startPos.x;
        height = targetPos.y - startPos.y;
        //x좌표 값 수정
            movePos = Vector2.zero;
            movePos.x = Vector2.MoveTowards(transform.position, targetPos,speed * Time.deltaTime).x;
            movePos.y = transform.position.y;
            transform.position = movePos;
        
        //가속도
            speed += addSpeed;

        //거리가 가까워졌을때 발생하는 이벤트
        if (Vector2.Distance(targetPos,transform.position) < 0.1)
        {
            MainGameManager.mainGameManager.CountMoney(gotGold);
            Destroy(gameObject);
        }
        //타겟위치 새롭게 갱신
        targetPos = Camera.main.ScreenToWorldPoint(new Vector2(tarX, tarY));
        pos=new Vector2();
        
        //거리에 따른 타원의 방정식 만들기
        if (Mathf.Pow(height, 2) - Mathf.Pow((height / width) * (transform.position.x - targetPos.x),2) >= 0){
            pos.y = Mathf.Pow(Mathf.Pow(height, 2) - Mathf.Pow((height / width) * (transform.position.x-targetPos.x), 2), 0.5f)-Mathf.Abs(height)+targetPos.y;
        }
        else
        {
            pos.y = startPos.y;
        }
        pos.x = transform.position.x;
        transform.position = pos;
        */
    }
}
