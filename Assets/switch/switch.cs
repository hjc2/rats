using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public bool isActivated = true;

    public void ToggleState()
    {
        isActivated = !isActivated;
        foreach (Transform child in transform)
        {
            if (child.CompareTag("light"))
            {
                child.gameObject.SetActive(isActivated);
            }
        }
        ToggleVisual();
    }

    public void ToggleVisual(){
        GetComponent<SpriteRenderer>().color = isActivated ? new Color(0.4f, 0.2f, 0) : new Color(0.8f, 0.4f, 0);  // Dark brown : Light brown
    }
}