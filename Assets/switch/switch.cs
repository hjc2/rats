using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public bool isActivated = true;
    
    [Header("Connected Lights")]
    [Tooltip("Drag and drop light GameObjects here to control them with this switch")]
    public List<GameObject> controlledLights = new List<GameObject>();

    public void ToggleState()
    {
        isActivated = !isActivated;
        UpdateLights();
        ToggleVisual();
    }

    private void UpdateLights()
    {
        // Remove any null references from the list
        controlledLights.RemoveAll(light => light == null);
        
        // Toggle all connected lights
        foreach (GameObject light in controlledLights)
        {
            light.SetActive(isActivated);
        }
    }

    public void ToggleVisual()
    {
        GetComponent<SpriteRenderer>().color = isActivated ? new Color(0.4f, 0.2f, 0) : new Color(0.8f, 0.4f, 0);  // Dark brown : Light brown
    }
}