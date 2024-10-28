using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float moveDuration = 0.2f; // Duration of movement between tiles
    private Vector3Int currentCell;
    private Vector3Int targetCell;
    private bool isMoving = false;
    private Tilemap wallTilemap;
    private float moveTimer = 0f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3Int moveDirection = Vector3Int.zero;
    private Vector3 resetPosition;

    private void Start()
    {
        wallTilemap = FindObjectOfType<Tilemap>();

        currentCell = wallTilemap.WorldToCell(transform.position);
        targetCell = currentCell;
        SnapToGridCenter();
        resetPosition = transform.position;
    }

    private void Update()
    {
        HandleInput();
        
        if (isMoving)
        {
            SmoothMove();
        }
        else if (moveDirection != Vector3Int.zero)
        {
            TryMove(moveDirection);
        }
    }

    private void HandleInput()
    {
        moveDirection = Vector3Int.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            moveDirection.y = 1;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            moveDirection.y = -1;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveDirection.x = -1;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveDirection.x = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player Spotted");
        // resetPosition = transform.position;
        transform.position = resetPosition;
        currentCell = wallTilemap.WorldToCell(transform.position);
        targetCell = currentCell;
        startPosition = transform.position;
        endPosition = wallTilemap.GetCellCenterWorld(targetCell);
        // SnapToGridCenter();

    }

    private void TryMove(Vector3Int direction)
    {
        Vector3Int newTargetCell = currentCell + direction;
        if (!wallTilemap.HasTile(newTargetCell))
        {
            targetCell = newTargetCell;
            startPosition = transform.position;
            endPosition = wallTilemap.GetCellCenterWorld(targetCell);
            isMoving = true;
            moveTimer = 0f;
        }
    }

    private void SmoothMove()
    {
        moveTimer += Time.deltaTime;
        float t = Mathf.Clamp01(moveTimer / moveDuration);
        
        // Use smoothstep for easing
        t = t * t * (3f - 2f * t);
        
        transform.position = Vector3.Lerp(startPosition, endPosition, t);

        if (moveTimer >= moveDuration)
        {
            transform.position = endPosition;
            currentCell = targetCell;
            isMoving = false;
            
            // Immediately try to move again in the held direction
            if (moveDirection != Vector3Int.zero)
            {
                TryMove(moveDirection);
            }
        }
    }

    private void SnapToGridCenter()
    {
        transform.position = wallTilemap.GetCellCenterWorld(currentCell);
    }
}