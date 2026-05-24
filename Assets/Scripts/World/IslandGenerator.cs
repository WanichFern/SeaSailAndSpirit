using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class IslandGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public Tilemap groundTilemap;
    public RuleTile grassRuleTile;

    [Header("Island Generation")]
    public int targetIslandSize = 300;
    public Vector3 islandOffset = new Vector3(50, 0, 0);

    [Header("Spawn Settings")]
    public GameObject[] resourcePrefabs;
    public GameObject[] enemyPrefabs;
    [Range(0, 1)] public float resourceDensity = 0.1f;
    [Range(0, 1)] public float enemyDensity = 0.05f;

    void Start()
    {
        GenerateIsland();
    }

    void GenerateIsland()
    {
        groundTilemap.transform.position = islandOffset;

        HashSet<Vector3Int> landPositions = new HashSet<Vector3Int>();
        List<Vector3Int> potentialExpansions = new List<Vector3Int>();

        Vector3Int center = Vector3Int.zero;
        landPositions.Add(center);
        AddNeighborsToExpansion(center, potentialExpansions, landPositions);

        while (landPositions.Count < targetIslandSize && potentialExpansions.Count > 0)
        {
            int randomIndex = Random.Range(0, potentialExpansions.Count);
            Vector3Int chosenPos = potentialExpansions[randomIndex];
            potentialExpansions.RemoveAt(randomIndex);

            if (!landPositions.Contains(chosenPos))
            {
                landPositions.Add(chosenPos);
                AddNeighborsToExpansion(chosenPos, potentialExpansions, landPositions);
            }
        }

        groundTilemap.ClearAllTiles();

        int groundLayer = LayerMask.NameToLayer("Ground");

        GameObject colliderContainer = new GameObject("IslandColliders");
        colliderContainer.transform.position = groundTilemap.transform.position;

        foreach (Vector3Int pos in landPositions)
        {
            groundTilemap.SetTile(pos, grassRuleTile);

            Vector3 worldPos = groundTilemap.GetCellCenterWorld(pos);

            GameObject floorCollider = new GameObject($"TileCollider_{pos.x}_{pos.y}");
            floorCollider.layer = LayerMask.NameToLayer("Ground");

            floorCollider.transform.SetParent(colliderContainer.transform);
            floorCollider.transform.position = worldPos;

            BoxCollider bc = floorCollider.AddComponent<BoxCollider>();
            bc.size = new Vector3(2f, 0.1f, 2f);
        }

        PopulateIsland(landPositions);
    }

    void AddNeighborsToExpansion(Vector3Int pos, List<Vector3Int> expansions, HashSet<Vector3Int> currentLand)
    {
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        foreach (Vector3Int dir in directions)
        {
            Vector3Int neighbor = pos + dir;
            if (!currentLand.Contains(neighbor) && !expansions.Contains(neighbor))
            {
                expansions.Add(neighbor);
            }
        }
    }

    void PopulateIsland(HashSet<Vector3Int> landPositions)
    {
        foreach (Vector3Int pos in landPositions)
        {
            if (pos == Vector3Int.zero) continue;

            if (Random.value < enemyDensity)
            {
                SpawnObject(enemyPrefabs, pos);
            }

            else if (Random.value < resourceDensity)
            {
                SpawnObject(resourcePrefabs, pos);
            }
        }
    }

    void SpawnObject(GameObject[] prefabs, Vector3Int gridPos)
    {
        if (prefabs.Length == 0) return;

        GameObject prefabToSpawn = prefabs[Random.Range(0, prefabs.Length)];
        Vector3 spawnPos = groundTilemap.CellToWorld(gridPos);
        spawnPos = groundTilemap.GetCellCenterWorld(gridPos);
        spawnPos.y += 0.5f;

        float checkRadius = 0.4f;

        int layerMask = LayerMask.GetMask("Resource", "Enemy");

        if (Physics.CheckSphere(spawnPos, checkRadius, layerMask))
        {
            return;
        }

        Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform);
    }
}