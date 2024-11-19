using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchController : MonoBehaviour
{
    public bool isActivated = true;
    
    [Header("Connected Lights")]
    [Tooltip("Drag and drop light GameObjects here to control them with this switch")]
    public List<GameObject> controlledLights = new List<GameObject>();

    private Color brightYellow = new Color(1f, 1f, 0f);      // Bright yellow
    private Color darkYellow = new Color(0.1f, 0.2f, 0.1f);    // Dark yellow

    public void ToggleState()
    {
        isActivated = !isActivated;
        UpdateLights();
    }

    private void UpdateLights()
    {
        // Remove any null references from the list
        controlledLights.RemoveAll(light => light == null);
        
        // Toggle all connected lights
        foreach (GameObject light in controlledLights)
        {
            // Get all components on the parent object only
            Component[] parentComponents = light.GetComponents<Component>();
            
            bool childStatus = false;

            var behaviours = light.GetComponents<Behaviour>();
            var renderer = light.GetComponent<Renderer>();
            var collider = light.GetComponent<Collider>();

            bool newState = false;
            
            // Toggle all Behaviour components except Transform
            foreach (var behaviour in behaviours)
            {   
                if (!(behaviour is Transform))
                {
                    behaviour.enabled = !behaviour.enabled;
                    newState = behaviour.enabled;
                }
            }
    
            if (renderer != null)
            {
                renderer.enabled = !renderer.enabled;
                newState = renderer.enabled;
            }
            
            if (collider != null)
            {
                collider.enabled = !collider.enabled;
                newState = collider.enabled;
            }
            
            UpdateChildColors(light, newState);

        }
    }

    private void UpdateChildColors(GameObject parent, bool lightStatus)
    {
        Renderer[] childRenderers = parent.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in childRenderers)
        {
            // Only update child objects, not the parent
            if (renderer.gameObject != parent)
            {
                if (renderer is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = lightStatus ? brightYellow : darkYellow;
                }
            }
        }
    }

    private void OnValidate()
    {
        LightImages();
    }

    private void OnEnable()
    {
        LightImages();
    }

    private void OnDisable()
    {
        LightImages();
    }

    private void LightImages()
    {
        controlledLights.RemoveAll(light => light == null);

        foreach (GameObject light in controlledLights)
        {
            var behaviour = light.GetComponent<Behaviour>();
            if (behaviour != null)
            {
                UpdateChildColors(light, behaviour.enabled);
            }
        }
    }
}