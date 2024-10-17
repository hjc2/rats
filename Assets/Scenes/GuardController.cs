using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GuardController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float moveSpeed = 2f;
    public float flashlightLength = 3f;
    public Color flashlightColor = Color.yellow;

    private Vector3Int currentCell;
    private Tilemap wallTilemap;
    private List<Vector2Int> patrolPoints;
    private int currentPatrolIndex = 0;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private GameObject flashlight;

    public void Initialize(Tilemap tilemap, GuardDefinition guardDef)
    {
        wallTilemap = tilemap;
        moveSpeed = guardDef.moveSpeed;
        flashlightLength = guardDef.flashlightLength;
        flashlightColor = guardDef.flashlightColor;
        patrolPoints = new List<Vector2Int>(guardDef.patrolPoints);
        currentCell = new Vector3Int(patrolPoints[0].x, patrolPoints[0].y, 0);
        transform.position = wallTilemap.GetCellCenterWorld(currentCell);
        
        if (spriteRenderer != null && guardDef.sprite != null)
        {
            spriteRenderer.sprite = guardDef.sprite;
        }

        name = guardDef.name;
        
        CreateFlashlight();
        SetNewTargetPosition();
    }

    private void CreateFlashlight()
    {
        flashlight = new GameObject("Flashlight");
        flashlight.transform.SetParent(transform);
        flashlight.transform.localPosition = Vector3.zero;

        MeshFilter meshFilter = flashlight.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = flashlight.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[3];
        int[] triangles = new int[3];

        vertices[0] = Vector3.zero;
        vertices[1] = new Vector3(-0.25f, flashlightLength, 0);
        vertices[2] = new Vector3(0.25f, flashlightLength, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = flashlightColor;

        flashlight.transform.localPosition = new Vector3(0, 0.5f, 0);
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
            UpdateFlashlightRotation(moveDirection);
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

    private void UpdateFlashlightRotation(Vector3Int direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        flashlight.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}