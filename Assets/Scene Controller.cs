using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private float total;
    void Start() {
      total = SceneManager.sceneCountInBuildSettings;
    }
    // Update is called once per frame
    void OnSceneInc() {
      int index = (int) SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(nfmod(index + 1, total), LoadSceneMode.Single);
    }

    void OnSceneDec(InputValue value) {
      int index = (int) SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(nfmod(index - 1, total), LoadSceneMode.Single);
    }

    int nfmod(float a,float b)
    {
      return (int) (a - b * Mathf.Floor(a / b));
    }

    void OnTriggerEnter2D(Collider2D other) {
      if (other.gameObject.tag == "Player") {
        SceneManager.LoadScene(6, LoadSceneMode.Single);
      }
    }
}
