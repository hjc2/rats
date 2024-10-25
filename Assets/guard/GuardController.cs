using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Guard : MonoBehaviour
{
    [Header("Patrol Settings")]
    public List<Vector2Int> patrolOffsets = new List<Vector2Int>(); // These are now relative offsets
    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;
    public bool loopPatrol = true;
    
    [Header("References")]
    public Transform flashlight;

    private int currentPoint = 0;
    private bool isMovingForward = true;
    private Vector2Int startPosition;
    private List<Vector3> worldPatrolPoints = new List<Vector3>();

    void Start()
    {
        // Store the guard's starting position in grid coordinates
        startPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        // Convert relative offsets to world positions
        worldPatrolPoints.Clear();
        foreach (Vector2Int offset in patrolOffsets)
        {
            Vector3 worldPoint = new Vector3(
                startPosition.x + offset.x,
                startPosition.y + offset.y,
                0
            );
            worldPatrolPoints.Add(worldPoint);
        }

        // Add starting position if no patrol points
        if (worldPatrolPoints.Count == 0)
        {
            worldPatrolPoints.Add(transform.position);
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
        if (worldPatrolPoints.Count <= 1) return;

        // Get current target point
        Vector3 targetPoint = worldPatrolPoints[currentPoint];
        
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
                currentPoint = (currentPoint + 1) % worldPatrolPoints.Count;
            }
            else
            {
                if (isMovingForward)
                {
                    currentPoint++;
                    if (currentPoint >= worldPatrolPoints.Count - 1)
                    {
                        currentPoint = worldPatrolPoints.Count - 1;
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
        // Draw the patrol path preview using offsets from current position
        Vector2Int previewStart = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        Gizmos.color = Color.yellow;
        
        // Draw points
        foreach (Vector2Int offset in patrolOffsets)
        {
            Vector3 worldPoint = new Vector3(
                previewStart.x + offset.x,
                previewStart.y + offset.y,
                0
            );
            Gizmos.DrawWireSphere(worldPoint, 0.3f);
        }

        // Draw lines between points
        for (int i = 0; i < patrolOffsets.Count - 1; i++)
        {
            Vector3 start = new Vector3(
                previewStart.x + patrolOffsets[i].x,
                previewStart.y + patrolOffsets[i].y,
                0
            );
            Vector3 end = new Vector3(
                previewStart.x + patrolOffsets[i + 1].x,
                previewStart.y + patrolOffsets[i + 1].y,
                0
            );
            Gizmos.DrawLine(start, end);
        }

        // Draw line between last and first point if looping
        if (loopPatrol && patrolOffsets.Count > 1)
        {
            Vector3 start = new Vector3(
                previewStart.x + patrolOffsets[patrolOffsets.Count - 1].x,
                previewStart.y + patrolOffsets[patrolOffsets.Count - 1].y,
                0
            );
            Vector3 end = new Vector3(
                previewStart.x + patrolOffsets[0].x,
                previewStart.y + patrolOffsets[0].y,
                0
            );
            Gizmos.DrawLine(start, end);
        }
    }
}