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
    public GameObject cage;

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
        

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, raycastDistance, obstacleMask);

        Debug.DrawRay(transform.position, direction * raycastDistance, Color.white);

        GameObject deathObject = null;
        
        // GameObject deathObject = GameObject.Find("death");
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.name == "death")
            {
                deathObject = obj;
                break;
            }
        }

        
        if (hits.Length > 0 && hits[0].collider != null && deathObject != null) 
        {   

            
            // Debug.Log("Raycast hit: " + hits[0].collider.name);
            Debug.Log(hits.Length);
            if(hits.Length == 3){
                Debug.Log(hits[0].collider.name);
                Debug.Log(hits[1].collider.name);
                if(hits[1].collider.name == "player"){
                    Debug.Log("player found");
                    audioManager.PlaySFX(audioManager.squeak);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                Debug.Log(hits[2].collider.name);
            }
            // Debug.Log("Raycast hit: " + hit.collider.name);
            if (hits[0].collider.CompareTag("Player") && deathObject.activeInHierarchy )
            {
                Debug.Log("Player detected in round light");
                audioManager.PlaySFX(audioManager.squeak);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);


                if (hit.collider != null && deathObject != null)
                { 
                    //Debug.Log("Raycast hit: " + hit.collider.name);
                    if (hit.collider.CompareTag("Player") && deathObject.activeInHierarchy)
                    {
                        //Debug.Log("Player detected in light beam");
                        StartCoroutine("Caught");
                        deathObject.SetActive(false);
                    } 
                    else if (hits.Length > 1 && hits[0].collider.CompareTag("switch") && hits[1].collider.CompareTag("Player"))
                    {
                        Debug.Log("Player detected in round light");
                        audioManager.PlaySFX(audioManager.squeak);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }

                if(hits.Length >= 3){
                    if(hits[2].collider.CompareTag("Player") && deathObject.activeInHierarchy){
                        Debug.Log("yay!");
                    }
                    if(hits[1].collider.CompareTag("Player") && deathObject.activeInHierarchy){
                        Debug.Log("yay! 2");
                    }
                }
            }
        }
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
        audioManager.PlaySFX(audioManager.squeak);
        yield return new WaitForSeconds(.25f);
        cage.SetActive(true);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}