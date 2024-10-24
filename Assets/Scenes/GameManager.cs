using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;


[Serializable]
public class GuardDefinition
{
    public string name;
    public Sprite sprite;
    public float moveSpeed = 2f;
    public float flashlightLength = 3f;
    public List<Vector2Int> patrolPoints = new List<Vector2Int>();
}

public class GameManager : MonoBehaviour
{
    public Tilemap wallTilemap;
    public TileBase wallTile;
    public GameObject playerPrefab;
    public Vector3Int playerSpawnCell;
    public GameObject guardPrefab;
    public List<GuardDefinition> guards;

    private void Start()
    {
        SpawnPlayer();
        // SpawnGuards();

        // add the already made guards to the list
        foreach (GuardController guard in FindObjectsOfType<GuardController>())
        {

            guards.Add(new GuardDefinition
            {
                name = guard.name,
                sprite = guard.spriteRenderer.sprite,
                moveSpeed = guard.moveSpeed,
                flashlightLength = guard.flashlightLength,
                patrolPoints = new List<Vector2Int>(guard.patrolPoints)
            });

            // terrible solution but it works
            guard.gameObject.SetActive(false);

            Debug.Log("Guard added to list");
        }

        SpawnGuards();
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPosition = wallTilemap.GetCellCenterWorld(playerSpawnCell);
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }

    private void SpawnGuards()
    {
        foreach (GuardDefinition guard in guards)
        {
            if (guard.patrolPoints.Count < 2)
            {
                Debug.LogWarning($"Guard {guard.name} has fewer than 2 patrol points. Skipping.");
                continue;
            }

            Vector3 spawnPosition = wallTilemap.GetCellCenterWorld(new Vector3Int(guard.patrolPoints[0].x, guard.patrolPoints[0].y, 0));
            GameObject guardObject = Instantiate(guardPrefab, spawnPosition, Quaternion.identity);

            GuardController guardController = guardObject.GetComponent<GuardController>();
            guardController.Initialize(wallTilemap, guard);
            
            guardController.setFlashlightLength(guard.flashlightLength);
        }
    }
}