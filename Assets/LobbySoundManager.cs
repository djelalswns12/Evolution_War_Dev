using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbySoundManager : MonoBehaviour
{
    public static LobbySoundManager Instance;
    public AudioSource UiOpenSound;
    public AudioSource UiCloseSound;
    public AudioSource UiClickSound;
    public AudioSource BGMSound;
    public AudioSource MatchingSound;
    public AudioSource CoinSound;
    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        BGMSoundPlay();
    }

    // Update is called once per frame
    void Update()
    {
 
    }
    public void UiOpenSoundPlay()
    {
        UiOpenSound.PlayOneShot(UiOpenSound.clip);
    }
    public void UiCloseSoundPlay()
    {
        UiCloseSound.PlayOneShot(UiCloseSound.clip);
    }
    public void BtnClickSoundPlay()
    {
        UiClickSound.PlayOneShot(UiClickSound.clip);
    }
    public void BGMSoundPlay()
    {
        BGMSound.loop = true;
        BGMSound.Play();
    }
    public void BGMSoundStop()
    {
        BGMSound.Stop();
    }
    public void CoinSoundPlay()
    {
        CoinSound.PlayOneShot(CoinSound.clip);
    }
    public void MatchingSoundPlay()
    {
        MatchingSound.Play();
    }
}
