using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GuardController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 2f;
    public float flashlightLength = 3f;

    private Vector3Int currentCell;
    private Tilemap wallTilemap;
    public List<Vector2Int> patrolPoints;
    private int currentPatrolIndex = 0;
    private Vector3 targetPosition;
    private bool isMoving = false;


    public void Initialize(Tilemap tilemap, GuardDefinition guardDef)
    {
        wallTilemap = tilemap;
        moveSpeed = guardDef.moveSpeed;
        
        patrolPoints = new List<Vector2Int>(guardDef.patrolPoints);
        currentCell = new Vector3Int(patrolPoints[0].x, patrolPoints[0].y, 0);
        transform.position = wallTilemap.GetCellCenterWorld(currentCell);
        
        if (spriteRenderer != null && guardDef.sprite != null)
        {
            spriteRenderer.sprite = guardDef.sprite;
        }

        name = guardDef.name;
        
        SetNewTargetPosition();
    }

    private void Update()
    {
        Move();
    }


    public void setFlashlightLength(float length)
    {
        flashlightLength = length;
        transform.GetChild(0).localScale = new Vector3(1, flashlightLength, 1);
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
        transform.rotation = Quaternion.Euler(0, 0, angle - 180);
    }

}