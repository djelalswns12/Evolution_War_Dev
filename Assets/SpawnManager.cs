using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public int maxCount;
    public static SpawnManager Instance;
    public GameObject up, down;

    [Header("0:name 1:PrimaryKey 2:GroundLayer")]
    private List<string[]> Spawner=new List<string[]>();
    private List<string[]> DownSpawner = new List<string[]>();
    public int createCnt;
    public float spawnSpeed;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (Spawner.Count > 0)
        {
            up.SetActive(true);
        }
        else
        {
            up.SetActive(false);
        }
        if (DownSpawner.Count > 0)
        {
            down.SetActive(true);
        }
        else
        {
            down.SetActive(false);
        }
    }
    public static string GetCreateCnt()
    {
        return Instance.createCnt.ToString();
    }
    public static void AddCreateCnt()
    {
        Instance.createCnt += 1;
    }
    public static void AddSpawnerList(string monsterName,int layer)
    {
        var data=new string[]{ monsterName, GetCreateCnt(),layer.ToString() };
        if (!IsListFull(layer))
        {
            GetSpawnerList(layer).Add(data);
            AddCreateCnt();
        }
        else
        {
            Debug.Log("대기열에 추가하려 하였으나 이미 꽉 차 있습니다.");
        }
    }
    public static bool IsListFull(int layer)
    {
        if (GetSpawnerList(layer).Count < Instance.maxCount)
        {
            return false;
        }
            return true;
    }
    public static List<string[]> GetSpawnerList(int layer)
    {
        //0: 다운레이어 , 1:업 레이어;
        if (layer == 1)
        {
            return Instance.Spawner;
        }else
        {
            return Instance.DownSpawner;
        }
    }
    public static void DeleteSpawnerList(int layer, int index,bool moneyBack=true)
    {
        if (GetSpawnerList(layer).Count > index)
        {
            if (moneyBack)
            {
                MainGameManager.mainGameManager.CountMoney(int.Parse(SceneVarScript.Instance.GetOptionByName(GetSpawnerList(layer)[index][0], "cost", SceneVarScript.Instance.monsterOption)));
            }
            GetSpawnerList(layer).RemoveAt(index);
        }
    }
}
