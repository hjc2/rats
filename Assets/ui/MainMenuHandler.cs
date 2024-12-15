using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MainMenuHandler : MonoBehaviour
{
    public void QuitGame() {
#if UNITY_EDITOR
        if (Application.isEditor) {
            EditorApplication.ExitPlaymode();
        }
#else
        Application.Quit();
#endif
    }
}
