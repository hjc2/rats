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
    private Color darkYellow = new Color(0.5f, 0.5f, 0f);    // Dark yellow

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
            // Get all components on the parent object only
            Component[] parentComponents = light.GetComponents<Component>();
            
            // Disable all components on the parent except Transform
            foreach (Component component in parentComponents)
            {
                if (!(component is Transform))
                {
                    if (component is Behaviour behaviour)
                    {
                        behaviour.enabled = isActivated;
                    }
                    else if (component is Renderer renderer)
                    {
                        renderer.enabled = isActivated;
                    }
                    else if (component is Collider collider)
                    {
                        collider.enabled = isActivated;
                    }
                }
            }

            // Update all child renderers' colors
            UpdateChildColors(light);
        }
    }

    private void UpdateChildColors(GameObject parent)
    {
        // Get all renderers in children
        Renderer[] childRenderers = parent.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in childRenderers)
        {
            // Only update child objects, not the parent
            if (renderer.gameObject != parent)
            {
                if (renderer is SpriteRenderer spriteRenderer)
                {
                    spriteRenderer.color = isActivated ? brightYellow : darkYellow;
                }
                else
                {
                    // For other types of renderers (MeshRenderer, etc.)
                    if (renderer.material != null)
                    {
                        renderer.material.color = isActivated ? brightYellow : darkYellow;
                    }
                }
            }
        }
    }

    public void ToggleVisual()
    {
        GetComponent<SpriteRenderer>().color = isActivated ? new Color(0.4f, 0.2f, 0) : new Color(0.8f, 0.4f, 0);  // Dark brown : Light brown
    }
}