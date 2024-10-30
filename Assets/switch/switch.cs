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
    }
}