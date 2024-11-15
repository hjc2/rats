using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class Guard : MonoBehaviour
{
    [Header("Patrol Settings")]
    public List<Vector2> patrolOffsets = new List<Vector2>(); // Relative Offsets Input
    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;
    public bool loopPatrol = true;

    private Transform flashlight;
    private int currentPoint = 0;
    private bool isMovingForward = true;
    private Vector2 startPosition;
    private List<Vector3> worldPatrolPoints = new List<Vector3>(); // Absolute locations for patrol

    void Start()
    {
        // Store the guard's starting position in grid coordinates
        startPosition = new Vector2(
            transform.position.x,
            transform.position.y
        );

        // Convert relative offsets to world positions
        worldPatrolPoints.Clear();
        foreach (Vector2 offset in patrolOffsets)
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

        // search and find flashlight
        flashlight = transform.Find("Flashlight");
        if (flashlight == null){ Debug.LogError("Flashlight not found as child of guard! Please ensure there's a child object named 'Flashlight'"); }
    }

    void Update()
    {
        if (worldPatrolPoints.Count <= 1) return;

        // Get current target point
        Vector3 targetPoint = worldPatrolPoints[currentPoint];

        // Get direction you want to move (vector from current position to target position)
        Vector3 movementDirection = (targetPoint - transform.position).normalized;

        // Current orientation of the guard
        float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
        
        // Desired orientation of the guard
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);

        // If the guard is not facing the target position
        if (transform.rotation != targetRotation)
        { // rotate, don't move
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                        targetRotation,
                                                        rotationSpeed * Time.deltaTime);
        } else { // else guard is facing target position, so move, but don't rotate
        transform.position = Vector3.MoveTowards(transform.position,
                                               targetPoint,
                                               moveSpeed * Time.deltaTime);
        }

        // Check if reached target point
        if (Vector3.Distance(transform.position, targetPoint) == 0f)
        {
            //walkPath();
            if (loopPatrol)
            {
                currentPoint = (currentPoint + 1) % worldPatrolPoints.Count;
            }
            else
            {
                if (isMovingForward)
                {
                    // currentPoint++;
                    // if (currentPoint >= worldPatrolPoints.Count - 1)
                    // {
                    //     currentPoint = worldPatrolPoints.Count - 1;
                    //     isMovingForward = false;
                    // }
                    walkPathForward();
                }
                else
                {
                    // currentPoint--;
                    // if (currentPoint <= 0)
                    // {
                    //     currentPoint = 0;
                    //     isMovingForward = true;
                    // }
                    walkPathBackward();
                }
            }
        }
    }

void walkPathForward() {
    // currentPoint++;
    // if (currentPoint >= worldPatrolPoints.Count - 1)
    // {
    //     // currentPoint = worldPatrolPoints.Count - 1;
    //     // isMovingForward = false;
    //     currentPoint = 0;
    //     walkPathForward();
    //     // currentPoint = currentPoint % worldPatrolPoints.Count;
    //     // isMovingForward = false;
    // }
    // if (currentPoint == worldPatrolPoints.Count - 1) {
    //     currentPoint = 0;
    // } else {
    //     currentPoint++;
    // }
    currentPoint = (currentPoint + 1) % worldPatrolPoints.Count;
}

void walkPathBackward() {
    // currentPoint--;
    // if (currentPoint <= 0)
    // {
    //     // currentPoint = 0;
    //     // isMovingForward = true;
    //     currentPoint = worldPatrolPoints.Count;
    //     walkPathBackward();
    //     // currentPoint = currentPoint % worldPatrolPoints.Count;
    //     // isMovingForward = true;
    // }
    // if (currentPoint == 0) {
    //     currentPoint = worldPatrolPoints.Count - 1;
    // } else {
    //     currentPoint--;
    // }
    if (currentPoint == 0) {
        currentPoint = worldPatrolPoints.Count - 1;
    } else {
        currentPoint = currentPoint - 1;
    }
}

void OnTriggerEnter2D(Collider2D other) {
    Debug.Log("collision detected");
    if (other.gameObject.tag == "Box") {
        Debug.Log("box collision detected");
        //currentPoint--;
        if (isMovingForward) {
            //currentPoint--;
            //isMovingForward = false;
            walkPathBackward();
        } else if (!isMovingForward) {
            //currentPoint++;
            //isMovingForward = true;
            walkPathForward();
        }
        isMovingForward = !isMovingForward;
    }
}

void OnDrawGizmosSelected()
{
    // Check if we should use absolute or relative positions
    // relative for editor - absolute for runtime
    bool useAbsolute = worldPatrolPoints != null && worldPatrolPoints.Count > 0;
    
    Vector2 previewStart = new Vector2(
        transform.position.x,
        transform.position.y
    );

    // Draw points
    if (useAbsolute)
    {
        // Draw using absolute world positions
        for (int i = 0; i < worldPatrolPoints.Count; i++)
        {
            // Gradient from yellow to red based on position in sequence
            float t = (float)i / Mathf.Max(1, worldPatrolPoints.Count - 1);
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, t);
            Gizmos.DrawWireSphere(worldPatrolPoints[i], 0.3f);
        }

        // Draw lines between points with gradient and direction arrows
        for (int i = 0; i < worldPatrolPoints.Count - 1; i++)
        {
            float t = (float)i / Mathf.Max(1, worldPatrolPoints.Count - 1);
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, t);
            
            Vector3 start = worldPatrolPoints[i];
            Vector3 end = worldPatrolPoints[i + 1];
            Gizmos.DrawLine(start, end);
            
            // Draw direction arrow
            DrawDirectionArrow(start, end);
        }

        // Draw line between last and first point if looping
        if (loopPatrol && worldPatrolPoints.Count > 1)
        {
            Gizmos.color = Color.red;
            Vector3 start = worldPatrolPoints[worldPatrolPoints.Count - 1];
            Vector3 end = worldPatrolPoints[0];
            Gizmos.DrawLine(start, end);
            DrawDirectionArrow(start, end);
        }
    }
    else
    {
        // Draw using relative offsets
        List<Vector3> previewPoints = new List<Vector3>();
        
        // Convert offsets to world points for preview
        foreach (Vector2 offset in patrolOffsets)
        {
            previewPoints.Add(new Vector3(previewStart.x + offset.x, previewStart.y + offset.y, 0));
        }

        // Draw points with gradient
        for (int i = 0; i < previewPoints.Count; i++)
        {
            float t = (float)i / Mathf.Max(1, previewPoints.Count - 1);
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, t);
            Gizmos.DrawWireSphere(previewPoints[i], 0.3f);
        }

        // Draw lines between points with gradient and direction arrows
        for (int i = 0; i < previewPoints.Count - 1; i++)
        {
            float t = (float)i / Mathf.Max(1, previewPoints.Count - 1);
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, t);
            
            Vector3 start = previewPoints[i];
            Vector3 end = previewPoints[i + 1];
            Gizmos.DrawLine(start, end);
            
            // Draw direction arrow
            DrawDirectionArrow(start, end);
        }

        // Draw line between last and first point if looping
        if (loopPatrol && previewPoints.Count > 1)
        {
            Gizmos.color = Color.red;
            Vector3 start = previewPoints[previewPoints.Count - 1];
            Vector3 end = previewPoints[0];
            Gizmos.DrawLine(start, end);
            DrawDirectionArrow(start, end);
        }
    }
}

private void DrawDirectionArrow(Vector3 start, Vector3 end)
{
    Vector3 direction = (end - start).normalized;
    Vector3 middle = Vector3.Lerp(start, end, 0.5f);
    
    // Calculate perpendicular vectors for arrow
    Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0) * 0.2f;
    
    // Draw arrow head
    Vector3 arrowStart = middle - direction * 0.2f;
    Vector3 tip = middle + direction * 0.2f;
    
    Gizmos.DrawLine(arrowStart + perpendicular, tip);
    Gizmos.DrawLine(arrowStart - perpendicular, tip);
}
}
