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

    // Dictionary to store original states of components
    private Dictionary<Component, bool> originalStates = new Dictionary<Component, bool>();
    private Dictionary<Renderer, Color> originalColors = new Dictionary<Renderer, Color>();

    private void Start()
    {
        StoreOriginalStates();
    }

    private void StoreOriginalStates()
    {
        originalStates.Clear();
        originalColors.Clear();

        foreach (GameObject light in controlledLights)
        {
            if (light == null) continue;

            // Store states of parent components
            Component[] parentComponents = light.GetComponents<Component>();
            foreach (Component component in parentComponents)
            {
                if (component is Behaviour behaviour)
                {
                    originalStates[component] = behaviour.enabled;
                }
                else if (component is Renderer renderer)
                {
                    originalStates[component] = renderer.enabled;
                }
                else if (component is Collider collider)
                {
                    originalStates[component] = collider.enabled;
                }
            }

            // Store states and colors of child renderers
            Renderer[] childRenderers = light.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer renderer in childRenderers)
            {
                if (renderer.gameObject != light)
                {
                    if (renderer is SpriteRenderer spriteRenderer)
                    {
                        originalColors[renderer] = spriteRenderer.color;
                    }
                }
            }
        }
    }

    public void ResetToOriginal()
    {
        foreach (var kvp in originalStates)
        {
            Component component = kvp.Key;
            bool originalState = kvp.Value;

            if (component is Behaviour behaviour)
            {
                behaviour.enabled = originalState;
            }
            else if (component is Renderer renderer)
            {
                renderer.enabled = originalState;
            }
            else if (component is Collider collider)
            {
                collider.enabled = originalState;
            }
        }

        foreach (var kvp in originalColors)
        {
            Renderer renderer = kvp.Key;
            Color originalColor = kvp.Value;

            if (renderer is SpriteRenderer spriteRenderer)
            {
                spriteRenderer.color = originalColor;
            }
        }

        // Reset the activation state to match the original state
        isActivated = true;
    }

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
            // Disable all components on the parent except Transform
            foreach (Component component in parentComponents)
            {
                if (!(component is Transform))
                {
                    if (component is Behaviour behaviour)
                    {
                        behaviour.enabled = !behaviour.enabled;
                        childStatus = behaviour.enabled;
                        Debug.Log("Behaviour: " + behaviour.enabled);
                    }
                    else if (component is Renderer renderer)
                    {
                        renderer.enabled = !renderer.enabled;
                        childStatus = renderer.enabled;
                        Debug.Log("render: " + renderer.enabled);
                    }
                    else if (component is Collider collider)
                    {
                        Debug.Log("collider: " + collider.enabled);
                        collider.enabled = !collider.enabled;
                        childStatus = collider.enabled;
                    }
                }
            }

            // Update all child renderers' colors
            UpdateChildColors(light, childStatus);
        }
    }

    private void UpdateChildColors(GameObject parent, bool lightStatus)
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
            Component[] parentComponents = light.GetComponents<Component>();
            
            bool childStatus = false;
            foreach (Component component in parentComponents)
            {
                if (component is Behaviour behaviour)
                {
                    childStatus = behaviour.enabled;
                    UpdateChildColors(light, childStatus);
                }
            }
        }
    }
}