using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollAddManager : MonoBehaviour
{
    Queue<GameObject> list=new Queue<GameObject>();
    public GameObject obj;
    [SerializeField]
    public IDictionary[] data;
    public Transform contents;
    // Start is called before the first frame update
    // Update is called once per frame
    protected virtual void SetData(GameObject selectObj, int index) { }
    public void PushElements()
    {
        for (int i = data.Length - 1; i >= 0; i--)
        {
           var element=Instantiate(obj, contents, false);
           list.Enqueue(element);
           SetData(element,i);
        }
    }
    public void Clear()
    {

        while (list.Count > 0)
        {
            Destroy(list.Dequeue());
        }
    }
}
