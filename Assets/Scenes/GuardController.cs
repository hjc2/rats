using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GuardController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 2f;
    public float flashlightLength = 3f;
    public float rotationSpeed = 180f; // Degrees per second for rotation

    private Vector3Int currentCell;
    private Tilemap wallTilemap;
    public List<Vector2Int> patrolPoints;
    private int currentPatrolIndex = 0;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private Transform flashlightTransform;
    private Vector2Int currentDirection = Vector2Int.right;
    private Quaternion targetRotation;

    public void Initialize(Tilemap tilemap, GuardDefinition guardDef)
    {
        wallTilemap = tilemap;
        moveSpeed = guardDef.moveSpeed;
        flashlightLength = guardDef.flashlightLength;
        
        patrolPoints = new List<Vector2Int>(guardDef.patrolPoints);
        currentCell = new Vector3Int(patrolPoints[0].x, patrolPoints[0].y, 0);
        transform.position = wallTilemap.GetCellCenterWorld(currentCell);
        
        if (spriteRenderer != null && guardDef.sprite != null)
        {
            spriteRenderer.sprite = guardDef.sprite;
        }

        name = guardDef.name;
        flashlightTransform = transform.GetChild(0);
        
        SetNewTargetPosition();
    }

    private void Update()
    {
        Move();
        UpdateSmoothRotation();
        UpdateFlashlightLength(); // Moved after rotation update
    }

    private void UpdateFlashlightLength()
    {
        // Calculate the actual facing direction based on current rotation
        float currentAngle = transform.rotation.eulerAngles.z + 270; // Adjust for the -90 offset
        float radians = currentAngle * Mathf.Deg2Rad;
        Vector2 facingDirection = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
        
        Vector3Int currentPos = wallTilemap.WorldToCell(transform.position);
        float length = flashlightLength;

        // Check for walls along the actual facing direction
        for (int i = 1; i <= Mathf.CeilToInt(flashlightLength); i++)
        {
            Vector3Int checkPos = currentPos + new Vector3Int(
                Mathf.RoundToInt(facingDirection.x * i),
                Mathf.RoundToInt(facingDirection.y * i),
                0
            );
            
            if (wallTilemap.HasTile(checkPos))
            {
                Vector3 wallWorldPos = wallTilemap.GetCellCenterWorld(checkPos);
                length = Vector2.Distance(transform.position, wallWorldPos);
                break;
            }
        }

        flashlightTransform.localScale = new Vector3(1, length, 1);
    }

    private void UpdateSmoothRotation()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRotation, 
            rotationSpeed * Time.deltaTime
        );
    }

    public void setFlashlightLength(float length)
    {
        flashlightLength = length;
        UpdateFlashlightLength();
    }
    
    private void SetNewTargetPosition()
    {
        Vector2Int targetPoint = patrolPoints[currentPatrolIndex];
        Vector3Int nextCell = new Vector3Int(targetPoint.x, targetPoint.y, 0);

        if (currentCell == nextCell)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
            SetNewTargetPosition();
            return;
        }

        Vector3Int moveDirection = nextCell - currentCell;
        moveDirection = new Vector3Int(Mathf.Clamp(moveDirection.x, -1, 1), Mathf.Clamp(moveDirection.y, -1, 1), 0);

        Vector3Int targetCell = currentCell + moveDirection;
        if (!wallTilemap.HasTile(targetCell))
        {
            targetPosition = wallTilemap.GetCellCenterWorld(targetCell);
            isMoving = true;
            currentDirection = new Vector2Int(moveDirection.x, moveDirection.y);
            UpdateRotation(moveDirection);
        }
    }

    private void Move()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                currentCell = wallTilemap.WorldToCell(transform.position);
                isMoving = false;
                SetNewTargetPosition();
            }
        }
    }

    private void UpdateRotation(Vector3Int direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        targetRotation = Quaternion.Euler(0, 0, angle - 180);
    }
}