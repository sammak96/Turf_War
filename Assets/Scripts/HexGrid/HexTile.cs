// Scripts/HexGrid/HexTile.cs
using UnityEngine;

public class HexTile : MonoBehaviour
{
    // [SerializeField] exposes the private backing field of the auto-property in the Inspector.
    [SerializeField] public Hex HexCoordinates { get; private set; } // The logical coordinates of this tile on the hex grid
    [SerializeField] public MeshRenderer TileRenderer { get; private set; } // Reference to the tile's visual MeshRenderer component

    // Optional: Add properties for game state
    [SerializeField] private GameManager.PlayerID owner = GameManager.PlayerID.Player1; // Which player owns this tile
    [SerializeField] private bool isTurf = false; // Whether this tile is a turf objective
    [SerializeField] private bool isOccupied = false; // Whether a token is currently on this tile

    // Initializes the HexTile with its logical coordinates, world position, and parent transform.
    public void Initialize(Hex hex, Vector3 worldPos, Transform parentTransform)
    {
        HexCoordinates = hex; // Assign the logical hex coordinates
        transform.position = worldPos; // Set the tile's position in the Unity world
        transform.SetParent(parentTransform); // Organize the tile under a parent GameObject in the Hierarchy
        name = $"Hex ({hex.q}, {hex.r})"; // Name the GameObject for clarity in the Hierarchy
        TileRenderer = GetComponent<MeshRenderer>(); // Get the MeshRenderer component attached to this GameObject
    }

    // Method to visually highlight or unhighlight the tile, e.g., on mouse hover or selection.
    public void Highlight(bool enable)
    {
        if (TileRenderer != null)
        {
            // Example: Change the material color to yellow when highlighted, or back to white.
            // In a real game, this might involve enabling/disabling an outline, a glow effect, etc.
            TileRenderer.material.color = enable ? Color.yellow : Color.white;
        }
    }

    // Method to set the owner of this tile
    public void SetOwner(GameManager.PlayerID playerID)
    {
        owner = playerID;
        // Update visual representation based on ownership
        UpdateVisuals();
    }

    // Method to mark this tile as a turf
    public void SetAsTurf(bool turf)
    {
        isTurf = turf;
        UpdateVisuals();
    }

    // Method to set occupation status
    public void SetOccupied(bool occupied)
    {
        isOccupied = occupied;
        UpdateVisuals();
    }

    // Update the visual appearance based on tile state
    private void UpdateVisuals()
    {
        if (TileRenderer != null)
        {
            Color tileColor = Color.white; // Default color

            if (isTurf)
            {
                tileColor = Color.green; // Turfs are green
            }
            else if (isOccupied)
            {
                tileColor = owner == GameManager.PlayerID.Player1 ? Color.blue : Color.red; // Player colors
            }

            TileRenderer.material.color = tileColor;
        }
    }

    // Getter methods for tile state
    public GameManager.PlayerID Owner => owner;
    public bool IsTurf => isTurf;
    public bool IsOccupied => isOccupied;
} 