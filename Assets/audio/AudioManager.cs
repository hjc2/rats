using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip menu;
    public AudioClip level;
    public AudioClip celebrate;
    public AudioClip credits;
    public AudioClip squeak;
    public AudioClip button;
    public AudioClip lightSwitch;
    public AudioClip timer;

    private static AudioManager instance; 
    private void Awake() {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        musicSource.clip = menu;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayTimer(float duration)
    {
        AudioSource timerSource = gameObject.AddComponent<AudioSource>();
        timerSource.clip = timer;
        timerSource.loop = true;
        timerSource.Play();
        StartCoroutine(RemoveSourceAfterPlay(timerSource, duration));
    }

    IEnumerator RemoveSourceAfterPlay(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(source);
    }
}
