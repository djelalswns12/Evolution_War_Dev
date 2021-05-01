using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
public class SceneVarScript : MonoBehaviour
{
    public static SceneVarScript Instance;
	public IDictionary[] monsterOption;
    private void Awake()
    {
		Instance = this;
		DontDestroyOnLoad(gameObject);
		StartCoroutine(ReRequst());
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	IEnumerator ReRequst()
	{
		for (; ; )
		{
			RequestMonsterDB();
			yield return new WaitForSeconds(10f);
		}
	}
	public void RequestMonsterDB()
    {
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		FirebaseDatabase.DefaultInstance
		.GetReference("Monster")
		.GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted)
			{
				Debug.Log("DB 연결 실패");
				// Handle the error...
			}
			else if (task.IsCompleted)
			{
				int index = 0;
				//Debug.Log("찾았어요!!");
				DataSnapshot snapshot = task.Result;
				monsterOption = new IDictionary[snapshot.ChildrenCount];
				foreach (DataSnapshot data in snapshot.Children)
				{
					//받은 데이터들을 하나씩 잘라 string 배열에 저장
					monsterOption[index] = (IDictionary)data.Value;
					//Debug.Log(monsterOption[index]["name"]);
					index++;
				}
				// Do something with snapshot...
			}
		});
	}
}
