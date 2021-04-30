using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class overlapScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void release(Collider2D[] gb)
    {
        int pointIndex = -1;
        int monsterIndex = -1;
        float selPoint;
        int check = NetworkMaster.Instance.creatnumber + 1;
        monsterScript selectMonster;
        // Debug.Log(gb.Length);
        if (gb.Length > 0)
        {
            if (NetworkMaster.player.GetComponent<PlayerScript>().dir == true)
            {
                selPoint = -10000;
            }
            else
            {
                selPoint = 10000;
            }

            for (int i = 0; i < gb.Length; i++)
            {
                selectMonster = gb[i].GetComponent<monsterScript>();

                if (check > selectMonster.creatnumber && selectMonster.attackPlayer==false)
                {
                    //가장먼저 태어난 몬스터를 고름 > monsterIndex
                    check = selectMonster.creatnumber;
                    monsterIndex = i;
                }
                if (NetworkMaster.player.GetComponent<PlayerScript>().dir == true)
                {
                    //먼저 앞서가 있는 몬스터를 고름 > pointIndex
                    if (selPoint < selectMonster.transform.position.x)
                    {
                        selPoint = selectMonster.transform.position.x;
                        pointIndex = i;
                    }
                }
                else
                {
                    if (selPoint > selectMonster.transform.position.x)
                    {
                        selPoint = selectMonster.transform.position.x;
                        pointIndex = i;
                    }
                }

            }
            if (NetworkMaster.player.GetComponent<PlayerScript>().dir == true)
            {
                //가장 먼저 태어난 몬스터의 x좌표+보정값 보다 앞서있는 몬스터의 x좌표 값이 더 크다면 생성순서를 뒤바꿔준다.
                if (gb[monsterIndex].GetComponent<monsterScript>().transform.position.x + 0.3f < gb[pointIndex].GetComponent<monsterScript>().transform.position.x)
                {
                    int temp = gb[monsterIndex].GetComponent<monsterScript>().creatnumber;
                    gb[monsterIndex].GetComponent<monsterScript>().creatnumber = gb[pointIndex].GetComponent<monsterScript>().creatnumber;
                    gb[pointIndex].GetComponent<monsterScript>().creatnumber = temp;
                }
            }
            else
            {
                if (gb[monsterIndex].GetComponent<monsterScript>().transform.position.x - 0.3f > gb[pointIndex].GetComponent<monsterScript>().transform.position.x)
                {
                    int temp = gb[monsterIndex].GetComponent<monsterScript>().creatnumber;
                    gb[monsterIndex].GetComponent<monsterScript>().creatnumber = gb[pointIndex].GetComponent<monsterScript>().creatnumber;
                    gb[pointIndex].GetComponent<monsterScript>().creatnumber = temp;
                }
            }
            //Debug.Log(gb[monsterIndex].gameObject.GetComponent<monsterScript>().creatnumber+"탈출성공");
            //이 작업을 거친 몬스터를 중첩해제 시킨뒤 앞으로 이동시킨다.
            gb[monsterIndex].gameObject.GetComponent<monsterScript>().resetSpeed();
        }
    }
}
