// Scripts/HexGrid/HexGridGenerator.cs
using UnityEngine;
using System.Collections.Generic;

public class HexGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private GameObject hexTilePrefab; // Assign your HexTile prefab (a 3D model with HexTile script) here in the Inspector
    [SerializeField] private int gridRadius = 3; // Defines the size of the hexagonal map. A radius of 3 creates a 37-hex grid (center + 3 rings).
    [SerializeField] private float tileYOffsetStep = 0.1f; // Controls the height difference between "stair" rows [1]

    // A dictionary to quickly access HexTile objects by their logical Hex coordinates.
    // This allows for efficient lookup of tiles based on their grid position.
    public Dictionary<Hex, HexTile> hexTiles = new Dictionary<Hex, HexTile>();

    void Start()
    {
        GenerateGrid();
    }

    // [ContextMenu] attribute allows calling this method directly from the Inspector's context menu.
    // Useful for testing grid generation during development.
    [ContextMenu("Generate Grid")]
    public void GenerateGrid()
    {
        // Clear any existing tiles before generating a new grid.
        // This is important for regenerating the board in editor or at runtime.
        // DestroyImmediate is used in editor mode for instant cleanup.
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        hexTiles.Clear(); // Clear the dictionary as well

        // Loop through axial coordinates to generate hexes in a radial pattern based on gridRadius.
        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            // Calculate the valid range for 'r' coordinate for the current 'q' to form a hexagonal shape.
            int r1 = Mathf.Max(-gridRadius, -q - gridRadius);
            int r2 = Mathf.Min(gridRadius, -q + gridRadius);
            for (int r = r1; r <= r2; r++)
            {
                Hex hex = new Hex(q, r); // Create a new Hex struct for the current coordinates

                // Calculate Y offset for the "stair" effect.[1]
                // A simple approach: Y increases with the Manhattan distance from the center (0,0,0).
                float yOffset = (Mathf.Abs(hex.q) + Mathf.Abs(hex.r) + Mathf.Abs(hex.s)) / 2 * tileYOffsetStep;
                Vector3 worldPos = hex.ToWorldPosition(yOffset); // Convert hex coordinates to world position with Y offset

                // Instantiate the visual hex tile prefab at the calculated world position.
                // It's parented to this GameObject for organization in the Hierarchy.
                GameObject tileGO = Instantiate(hexTilePrefab, worldPos, Quaternion.identity, transform);
                HexTile hexTile = tileGO.GetComponent<HexTile>(); // Get the HexTile script component

                if (hexTile != null)
                {
                    hexTile.Initialize(hex, worldPos, transform); // Initialize the HexTile script
                    hexTiles.Add(hex, hexTile); // Add the tile to the dictionary for easy lookup
                }
            }
        }
        Debug.Log($"Generated {hexTiles.Count} hex tiles.");
    }

    // Retrieves a HexTile object given its logical Hex coordinates.
    public HexTile GetHexTile(Hex hex)
    {
        hexTiles.TryGetValue(hex, out HexTile tile); // TryGetValue is safer than direct access if key might not exist
        return tile;
    }

    // Returns all generated HexTiles.
    public IEnumerable<HexTile> GetAllHexTiles()
    {
        return hexTiles.Values;
    }

    // Placeholder method for distributing different "block" types randomly on the board.
    // This would involve assigning different materials, colors, or child GameObjects to each HexTile.
    public void DistributeBlocksRandomly()
    {
        // Example: Iterate through all generated hex tiles and assign a random block type.
        // This would involve a list of block prefabs or materials.
        Debug.Log("Randomly distributing blocks on the board.");
        foreach (HexTile tile in hexTiles.Values)
        {
            // For a simple visual, you could change the tile's material or color.
            // For complex blocks, you might instantiate a BlockPrefab on the tile.
            // tile.SetBlockType(RandomBlockType()); // Assuming a method to set block type
        }
    }
} 