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
				Debug.Log("DB ���� ����");
				// Handle the error...
			}
			else if (task.IsCompleted)
			{
				int index = 0;
				//Debug.Log("ã�Ҿ��!!");
				DataSnapshot snapshot = task.Result;
				monsterOption = new IDictionary[snapshot.ChildrenCount];
				foreach (DataSnapshot data in snapshot.Children)
				{
					//���� �����͵��� �ϳ��� �߶� string �迭�� ����
					monsterOption[index] = (IDictionary)data.Value;
					//Debug.Log(monsterOption[index]["name"]);
					index++;
				}
				// Do something with snapshot...
			}
		});
	}
}
