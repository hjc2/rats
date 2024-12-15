using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 // Make sure to use the URP namespace

public class FlashlightRaycast : MonoBehaviour
{
    private Transform player;
    public LayerMask obstacleMask; // Layer mask for obstacles (e.g., walls)
    private UnityEngine.Rendering.Universal.Light2D light2D;

    private AudioManager audioManager;
    public Animator animator;

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
            Debug.LogError("Light2D component not found on the light beam. Ensure the light beam has a Light2D component.");
        }
    }

    void Update()
    {
        if (player == null || light2D == null) return;

        // Use the light's radius as the raycast distance
        float raycastDistance = light2D.pointLightOuterRadius;
        //float raycastDistance = 1000;

        // Get the direction the light beam is facing
        Vector2 direction = transform.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, raycastDistance, obstacleMask);

        Debug.DrawRay(transform.position, direction * raycastDistance, Color.white);

        if (hit.collider != null) 
        { 
            //Debug.Log("Raycast hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Player detected in light beam");
                // Animator animator = Player.GetComponent<animator>();

                audioManager.PlaySFX(audioManager.squeak);
                StartCoroutine("Caught");



                animator.Play("player_walk");

                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            } 
        } else 
        { 
            // Debug.Log("No collision detected.");
        }
        // if (hit.collider != null && hit.collider.CompareTag("Player"))
        // {
        //     Debug.Log("Player detected in light beam");
        //     player.GetComponent<PlayerController>().ResetPlayerPosition();
        // }
        // else
        // {
        //     Debug.Log("Player not detected or obstacle in the way");
        // }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            light2D.enabled = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            light2D.enabled = true;
        }
    }

    IEnumerator Caught() {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}