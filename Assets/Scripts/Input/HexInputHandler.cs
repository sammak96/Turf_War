// Scripts/Input/HexInputHandler.cs
using UnityEngine;

public class HexInputHandler : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private Camera mainCamera; // Assign your main camera here in the Inspector
    [SerializeField] private LayerMask hexTileLayer; // Create a new Layer in Unity (e.g., "HexTile") and assign it to your hexTilePrefab.
                                                    // Then, select this layer mask in the Inspector for this script.

    // C# event that other systems can subscribe to, to be notified when a hex tile is clicked.
    // This promotes loose coupling between the input system and game logic.
    public static event System.Action<HexTile> OnHexTileClicked;

    void Start()
    {
        // If mainCamera is not assigned, try to find it automatically
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Check for left mouse button click.
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(Input.mousePosition);
        }
        // For mobile touch input, you would check Input.touchCount > 0 and Input.GetTouch(0).phase == TouchPhase.Began.
        // The HandleClick method can then be reused for touch positions.
    }

    // Handles the actual raycasting logic based on a screen position (mouse or touch).
    private void HandleClick(Vector3 screenClickPosition)
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not assigned to HexInputHandler!");
            return;
        }

        // Creates a ray originating from the camera and passing through the given screen point.
        Ray ray = mainCamera.ScreenPointToRay(screenClickPosition);
        RaycastHit hit; // A struct to store information about what the ray hit.

        // Performs the raycast.
        // 'ray.origin': starting point of the ray.
        // 'ray.direction': direction of the ray.
        // 'out hit': outputs the RaycastHit information.
        // '100f': max distance the ray will travel.
        // 'hexTileLayer': ensures the ray only detects colliders on the specified layer, improving performance and accuracy.
        if (Physics.Raycast(ray, out hit, 100f, hexTileLayer))
        {
            // Attempt to get the HexTile component from the GameObject that was hit.
            HexTile clickedTile = hit.collider.GetComponent<HexTile>();
            if (clickedTile != null)
            {
                Debug.Log($"Clicked on Hex Tile: {clickedTile.HexCoordinates.q}, {clickedTile.HexCoordinates.r}");
                OnHexTileClicked?.Invoke(clickedTile); // Invoke the event, passing the clicked HexTile.
                clickedTile.Highlight(true); // Example: Visually highlight the clicked tile.
            }
        }
    }
} 