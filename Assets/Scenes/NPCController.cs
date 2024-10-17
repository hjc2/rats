using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private float moveSpeed;
    private Vector3Int currentCell;
    private Tilemap wallTilemap;
    private List<Vector2Int> patrolPoints;
    private int currentPatrolIndex = 0;
    private Vector3 targetPosition;
    private bool isMoving = false;

    public void Initialize(Tilemap tilemap, GuardDefinition guard)
    {
        wallTilemap = tilemap;
        moveSpeed = guard.moveSpeed;
        patrolPoints = new List<Vector2Int>(guard.patrolPoints);
        currentCell = new Vector3Int(patrolPoints[0].x, patrolPoints[0].y, 0);
        transform.position = wallTilemap.GetCellCenterWorld(currentCell);
        
        if (spriteRenderer != null && guard.sprite != null)
        {
            spriteRenderer.sprite = guard.sprite;
        }

        name = guard.name; // Set the GameObject's name to the guard's name
        
        SetNewTargetPosition();
    }

    private void Update()
    {
        Move();
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
}