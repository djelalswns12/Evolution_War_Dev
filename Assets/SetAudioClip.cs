using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAudioClip : MonoBehaviour
{
    public AudioClip clip;
    public AudioSource myAudio;
    // Start is called before the first frame update
    void Start()
    {
        myAudio.clip = clip;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
