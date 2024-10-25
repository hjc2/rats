using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Guard : MonoBehaviour
{
    [Header("Patrol Settings")]
    public List<Vector3> patrolPoints = new List<Vector3>();
    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;
    public bool loopPatrol = true;
    
    [Header("Flashlight Settings")]
    public Transform flashlight; // Reference to the child flashlight object

    private int currentPoint = 0;
    private bool isMovingForward = true;

    void Start()
    {
        // Add current position as first patrol point if none set
        if (patrolPoints.Count == 0)
        {
            patrolPoints.Add(transform.position);
        }

        // Verify flashlight reference
        if (flashlight == null)
        {
            flashlight = transform.Find("Flashlight");
            if (flashlight == null)
            {
                Debug.LogError("Flashlight not found as child of guard! Please ensure there's a child object named 'Flashlight'");
            }
        }
    }

    void Update()
    {
        if (patrolPoints.Count <= 1) return;

        // Get current target point
        Vector3 targetPoint = patrolPoints[currentPoint];
        
        // Move towards target
        transform.position = Vector3.MoveTowards(transform.position, 
                                               targetPoint, 
                                               moveSpeed * Time.deltaTime);

        // Rotate towards movement direction
        Vector3 direction = (targetPoint - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                        targetRotation,
                                                        rotationSpeed * Time.deltaTime);
        }

        // Check if reached target point
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            if (loopPatrol)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Count;
            }
            else
            {
                if (isMovingForward)
                {
                    currentPoint++;
                    if (currentPoint >= patrolPoints.Count - 1)
                    {
                        currentPoint = patrolPoints.Count - 1;
                        isMovingForward = false;
                    }
                }
                else
                {
                    currentPoint--;
                    if (currentPoint <= 0)
                    {
                        currentPoint = 0;
                        isMovingForward = true;
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (patrolPoints.Count == 0) return;

        // Draw patrol path when guard is selected
        Gizmos.color = Color.yellow;
        
        // Draw points
        foreach (Vector3 point in patrolPoints)
        {
            Gizmos.DrawWireSphere(point, 0.3f);
        }

        // Draw lines between points
        for (int i = 0; i < patrolPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(patrolPoints[i], patrolPoints[i + 1]);
        }

        // Draw line between last and first point if looping
        if (loopPatrol && patrolPoints.Count > 1)
        {
            Gizmos.DrawLine(patrolPoints[patrolPoints.Count - 1], patrolPoints[0]);
        }
    }
}