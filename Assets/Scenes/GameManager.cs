using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Tilemap wallTilemap;
    public TileBase wallTile;
    public GameObject playerPrefab;

    private void Start()
    {
        GenerateMap();
        SpawnPlayer();
    }

    private void GenerateMap()
    {
        // Create a simple 10x10 room outline
        // for (int x = 0; x < 10; x++)
        // {
        //     for (int y = 0; y < 10; y++)
        //     {
        //         if (x == 0 || x == 9 || y == 0 || y == 9)
        //         {
        //             wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
        //         }
        //     }
        // }
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(1.5f, 1.5f, 0); // Adjusted to center of the first cell
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    }
}

