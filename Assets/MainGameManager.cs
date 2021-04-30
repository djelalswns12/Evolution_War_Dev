using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainGameManager : MonoBehaviour
{
    public static MainGameManager mainGameManager;
    [SerializeField]
    public int money;
    private float getMoneyTime;
    public GameObject hgold; // 머니 이펙트 리소스
    public int perMoney; //getPerTime초당 들어오는 돈의 양
    public float getPerTime;//초의 기준
    public Text goldText; 
    public float dropGoldEff; // 처치 골드 효율
    public GameObject focus; // 시야 포커스
    [SerializeField]
    private int touchDamge;
    // Start is called before the first frame update
    void Start()
    {
        focus = null;
        mainGameManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        getMoneyTime += Time.deltaTime;
        if (getMoneyTime >= getPerTime)
        {
           
            getMoneyTime-=getPerTime;
            CountMoney(perMoney);
        }
        goldText.text = StringDot(GetMoney().ToString())+" Gold";

        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    CreatGoldEffect(Camera.main.ScreenToWorldPoint(Input.mousePosition), Random.Range(10, 5000));
        //}
    }
    public void CreatGoldEffect(Vector2 pos,int gotGold)
    {
        GameObject GoldEffect=GameObject.Instantiate(hgold, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        GoldEffect.GetComponent<MoneyPos>().gotGold = gotGold;
        GoldEffect.GetComponent<MoneyPos>().txt.text = StringDot(gotGold)+"+";
    }
    public int GetMoney()
    {
        return money;
    }
    void SetMoney(int value)
    {
        money=value;
    }
    public void CountMoney(int value)
    {
        SetMoney(GetMoney() + value);
    }
    string StringDot<T>(T str)
    {
        string answer="";
        string copy=str.ToString();
        for(int i = copy.Length-1; i >= 0; i--)
        {
            answer = copy[i] + answer;
            if ( ((copy.Length - 1)-i)  % 3==2 && i>0)
            {
                answer = "," + answer;
            }
        }

        return answer;
    }
    public int GetTouchDamge()
    {
        return touchDamge;
    }
    public GameObject GetFocus()
    {
        return focus;
    }
    public void SetFocus(GameObject newOne)
    {
        this.focus = newOne;
    }
}
