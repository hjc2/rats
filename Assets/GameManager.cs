using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private AudioManager audioManager;

    private int numScenes;

    private static GameManager instance; 

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

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        numScenes = SceneManager.sceneCountInBuildSettings;
    }

    public void AdvanceScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        if (index < numScenes)
        {
            SceneManager.LoadScene(index);
            if (index == numScenes - 1)
            {
                audioManager.musicSource.clip = audioManager.credits;
                audioManager.musicSource.Play();
            }
            else
            {
                if (audioManager.musicSource.clip != audioManager.level)
                {
                    audioManager.musicSource.clip = audioManager.level;
                    audioManager.musicSource.Play();
                }
            }
        }
        else
        {
            SceneManager.LoadScene(0);
            audioManager.musicSource.clip = audioManager.menu;
            audioManager.musicSource.Play();
        }
        
    }

    public void GoToCredits()
    {
        SceneManager.LoadScene(numScenes - 1);
        audioManager.musicSource.clip = audioManager.credits;
        audioManager.musicSource.Play();
    }
}
