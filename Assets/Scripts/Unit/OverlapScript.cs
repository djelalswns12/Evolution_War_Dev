using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlapScript : MonoBehaviour
{
    public static OverlapScript Instance;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    public void ReleaseMonster(Collider2D[] OverlapUnits)
    {
        #region
        int pointIndex = -1;
        float selPoint;
        #endregion
        int monsterIndex = -1;
        int createNumber = NetworkMaster.Instance.creatnumber + 1;
        //생성이 가장 빨리되었던 몬스터를 찾기위한 인덱스 변수 이다. 
        //몬스터 생성번호의 (최대값+1)의 값으로 설정한다.
        monsterScript selectMonster;
        if (OverlapUnits.Length > 0)
        {
            #region
            if (NetworkMaster.player.GetComponent<PlayerScript>().dir == true)
            {
                selPoint = -10000;
            }
            else
            {
                selPoint = 10000;
            }
            #endregion
            for (int i = 0; i < OverlapUnits.Length; i++)
            {
                selectMonster = OverlapUnits[i].GetComponent<monsterScript>();

                if (createNumber > selectMonster.creatnumber && selectMonster.attackPlayer==false)
                {
                    //for문을 돌며 몬스터 생성이 가장 빨리된 몬스터의 인덱스를 구한다.
                    createNumber = selectMonster.creatnumber;
                    monsterIndex = i;
                }
                #region 
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
                if (OverlapUnits[monsterIndex].GetComponent<monsterScript>().transform.position.x + 0.3f < OverlapUnits[pointIndex].GetComponent<monsterScript>().transform.position.x)
                {
                    int temp = OverlapUnits[monsterIndex].GetComponent<monsterScript>().creatnumber;
                    OverlapUnits[monsterIndex].GetComponent<monsterScript>().creatnumber = OverlapUnits[pointIndex].GetComponent<monsterScript>().creatnumber;
                    OverlapUnits[pointIndex].GetComponent<monsterScript>().creatnumber = temp;
                }
            }
            else
            {
                if (OverlapUnits[monsterIndex].GetComponent<monsterScript>().transform.position.x - 0.3f > OverlapUnits[pointIndex].GetComponent<monsterScript>().transform.position.x)
                {
                    int temp = OverlapUnits[monsterIndex].GetComponent<monsterScript>().creatnumber;
                    OverlapUnits[monsterIndex].GetComponent<monsterScript>().creatnumber = OverlapUnits[pointIndex].GetComponent<monsterScript>().creatnumber;
                    OverlapUnits[pointIndex].GetComponent<monsterScript>().creatnumber = temp;
                }
            }
            #endregion
            //Debug.Log(monsterIndex);
            OverlapUnits[monsterIndex].gameObject.GetComponent<monsterScript>().ResetSpeed();
            //이 작업을 통해 결정된 몬스터는 멈춰있던 스피드 초기화 하여 전진할 수 있도록 한다.
        }
    }
}
