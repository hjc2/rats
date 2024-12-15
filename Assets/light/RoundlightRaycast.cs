using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundlightRaycast : MonoBehaviour
{
    private Transform player;
    public LayerMask obstacleMask; // Layer mask for obstacles (e.g., walls)
    private UnityEngine.Rendering.Universal.Light2D light2D;

    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        // Find AudioManager
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        // Automatically find the player and respawn point in the scene
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Get the Light2D component
        light2D = GetComponent<UnityEngine.Rendering.Universal.Light2D>();

        // Optional: Add error checking if player, respawn point, or light2D is not found
        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Ensure the player GameObject has the 'Player' tag.");
        }
        if (light2D == null)
        {
            Debug.LogError("Light2D component not found on the round light. Ensure the round light has a Light2D component.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || light2D == null || !light2D.enabled) return;

        // Use the light's radius as the raycast distance
        float raycastDistance = light2D.pointLightOuterRadius;
        //float raycastDistance = 1000;

        // Get the direction the light beam is facing
        Vector2 direction = player.position - transform.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, raycastDistance, obstacleMask);

        Debug.DrawRay(transform.position, direction * raycastDistance, Color.white);

        if (hits.Length > 0 && hits[0].collider != null) 
        { 
            // Debug.Log("Raycast hit: " + hit.collider.name);
            if (hits[0].collider.CompareTag("Player"))
            {
                Debug.Log("Player detected in round light");
                audioManager.PlaySFX(audioManager.squeak);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } 
            else if (hits[0].collider.CompareTag("switch") && hits[1].collider.CompareTag("Player"))
            {
                Debug.Log("Player detected in round light");
                audioManager.PlaySFX(audioManager.squeak);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        } else
        { 
            // Debug.Log("No collision detected.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box") && light2D != null)
        {
            light2D.enabled = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box") && light2D != null)
        {
            light2D.enabled = true;
        }
    }
    
}
