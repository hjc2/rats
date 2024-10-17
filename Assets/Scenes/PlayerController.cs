using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 movement;
    public Tilemap wallTilemap;

    private void Start()
    {
        wallTilemap = FindObjectOfType<GameManager>().wallTilemap;
    }

    private void Update()
    {
        // Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Normalize diagonal movement
        movement = movement.normalized;

        // Movement and collision check
        Vector3 newPosition = transform.position + (Vector3)movement * moveSpeed * Time.deltaTime;
        if (!IsCollidingWithWall(newPosition))
        {
            transform.position = newPosition;
        }
        else
        {
            // Try horizontal movement
            Vector3 horizontalMove = transform.position + new Vector3(movement.x, 0, 0) * moveSpeed * Time.deltaTime;
            if (!IsCollidingWithWall(horizontalMove))
            {
                transform.position = horizontalMove;
            }
            else
            {
                // Try vertical movement
                Vector3 verticalMove = transform.position + new Vector3(0, movement.y, 0) * moveSpeed * Time.deltaTime;
                if (!IsCollidingWithWall(verticalMove))
                {
                    transform.position = verticalMove;
                }
            }
        }
    }

    private bool IsCollidingWithWall(Vector3 position)
    {
        // Check the four corners of the player
        Vector3Int[] cellsToCheck = new Vector3Int[]
        {
            wallTilemap.WorldToCell(position + new Vector3(0.4f, 0.4f, 0)),
            wallTilemap.WorldToCell(position + new Vector3(-0.4f, 0.4f, 0)),
            wallTilemap.WorldToCell(position + new Vector3(0.4f, -0.4f, 0)),
            wallTilemap.WorldToCell(position + new Vector3(-0.4f, -0.4f, 0))
        };

        foreach (var cell in cellsToCheck)
        {
            if (wallTilemap.HasTile(cell))
            {
                return true;
            }
        }
        return false;
    }
}