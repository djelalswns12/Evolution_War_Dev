using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerSpawner : MonoBehaviour
{
    public int myLayer;
    public GameObject[] creatures;
    public Image[] bar;
    private List<string[]> myList;
    private RightNav RightNav;

    private List<float> nowCool = new List<float>();
    private List<float> setCool = new List<float>();
    // Start is called before the first frame update
    Vector3 startPos;
    void Start()
    {
        startPos = transform.localScale;
        RightNav = MainGameManager.mainGameManager.RightNav.GetComponent<RightNav>();
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkMaster.Instance.dir == true)
        {
            transform.localScale = startPos;
        }
        else
        {
            var newPos = startPos;
            newPos.x *= -1;
            transform.localScale = newPos;
        }
        myList = SpawnManager.GetSpawnerList(myLayer);
        ManageCool(0); // 0번쨰 칸만 쿨관리 한다.
        BarRendering(0); // 0번째 칸만 바 렌더링 관리 한다.
        RenderingCreatures(); //대기열의 모든 요소를 렌더링 한다.
    }
    public void RenderingCreatures()
    {
        for (int i = 0; i < creatures.Length; i++)
        {
            if (i < myList.Count)
            {
                creatures[i].SetActive(true);
                creatures[i].GetComponent<Image>().sprite = RightNav.imageList[int.Parse(SceneVarScript.Instance.GetOptionByName(myList[i][0], "icon", SceneVarScript.Instance.monsterOption))].sprite;
            }
            else
            {
                creatures[i].SetActive(false);
            }
        }
    }
    public void SpawnByNum(int num)
    {
        NetworkMaster.Instance.CreatMonster(myList[num][0], 0, 0, int.Parse(myList[num][2]),NetworkMaster.player);
        DeleteCreature(creatures[num], false);
    }
    public void DeleteCreature(GameObject obj)
    {
        for (int i = 0; i < creatures.Length; i++)
        {
            if (creatures[i] == obj)
            {
                SpawnManager.DeleteSpawnerList(myLayer, i);
            }
        }
    }
    public void DeleteCreature(GameObject obj, bool moneyBack)
    {
        for (int i = 0; i < creatures.Length; i++)
        {
            if (creatures[i] == obj)
            {
                SpawnManager.DeleteSpawnerList(myLayer, i, moneyBack);
            }
        }
    }
    public void ManageCool(int num)
    {
        for (int i = 0; i < myList.Count; i++)
        {
            if (setCool.Count < myList.Count)
            {
                setCool.Add(99999);
                nowCool.Add(0);
            }
            else
            {

            }
            var cool = SceneVarScript.Instance.GetOptionByName(myList[i][0], "cool", SceneVarScript.Instance.monsterOption);
            if (cool == "null")
            {
                Debug.Log("쿨타임 정보가 입력되지 않았습니다.");
                return;
            }
            setCool[i] = (float.Parse(cool));
        }
        nowCool[num] += Time.deltaTime*(1+SpawnManager.Instance.spawnSpeed);
        if (nowCool[num] >= setCool[num])
        {
            SpawnByNum(num);
            nowCool[num] = 0;
        }
    }
    public void BarRendering(int num)
    {
        if (nowCool[0] >= setCool[0])
        {
            bar[num].fillAmount = 1;
        }
        else if (nowCool[0] <= 0)
        {
            bar[num].fillAmount = 0;
        }
        else
        {
            bar[num].fillAmount = Mathf.Lerp(bar[0].fillAmount, nowCool[0] / setCool[0], 0.5f);
        }
    }
}
