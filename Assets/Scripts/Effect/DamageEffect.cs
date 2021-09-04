using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffect : MonoBehaviour
{
    public float xpower;
    public float ypower;
    float gra;
    float graScale;
    public float setGraScale;
    public float reduceGraScale;
    public float reduceScale;
    public Text txt;
    // Update is called once per frame
    private void Start()
    {
      
    }
    void Update()
    {
        if (gra > 0)
        {
            graScale = setGraScale;
        }
        else
        {
            graScale = setGraScale * reduceGraScale;
            transform.localScale *= 1-(reduceScale*Time.deltaTime);
        }

        transform.position += Vector3.right * xpower * Time.deltaTime;
        transform.position += Vector3.up * gra * Time.deltaTime;
        gra -= graScale * Time.deltaTime;
        if (transform.localScale.x <= 0.2)
        {
            DamageObjectPool.Instacne.Push(this);
        }
    }
    public void Init(Vector2 startPos,int damage,Color color)
    {
        xpower = Random.Range(-1f, 1f);
        ypower = Random.Range(3f, 5f);

        setGraScale = 8.5f;
        reduceGraScale = 1.5f;

        gra = ypower;

        transform.localScale = new Vector3(1, 1, 1);
        reduceScale = 3.5f;

        txt.text = damage.ToString();
        txt.color = color;

        transform.position = startPos;
    }
    public void TickInit(Vector2 startPos, int damage, Color color)
    {
        xpower = 0.5f;
        ypower = Random.Range(2f, 3f);

        setGraScale = 8.5f;
        reduceGraScale = 1.5f;

        gra = ypower;

        transform.localScale = new Vector3(1, 1, 1);
        reduceScale = 6f;

        txt.text = damage.ToString();
        txt.color = color;

        transform.position = startPos;
    }
}
