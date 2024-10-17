using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections.Generic;


[Serializable]
public class GuardDefinition
{
    public string name;
    public Sprite sprite;
    public float moveSpeed = 2f;
    public List<Vector2Int> patrolPoints = new List<Vector2Int>();
    public float flashlightLength = 3f;
    public Color flashlightColor = Color.yellow;
}

public class GameManager : MonoBehaviour
{
    public Tilemap wallTilemap;
    public TileBase wallTile;
    public GameObject playerPrefab;
    public GameObject guardPrefab;
    public List<GuardDefinition> guards;

    private void Start()
    {
        GenerateMap();
        SpawnPlayer();
        SpawnGuards();
    }

    private void GenerateMap()
    {
        // Create a simple 10x10 room outline
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                if (x == 0 || x == 9 || y == 0 || y == 9)
                {
                    // wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }
    }

    private void SpawnPlayer()
    {
        Vector3Int spawnCell = new Vector3Int(1, 1, 0);
        Vector3 spawnPosition = wallTilemap.GetCellCenterWorld(spawnCell);
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
        }
    }
}