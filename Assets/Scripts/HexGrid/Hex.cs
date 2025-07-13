// Scripts/HexGrid/Hex.cs
using UnityEngine;
using System.Collections.Generic;

// Represents a single hexagonal cell in axial coordinates.
// [System.Serializable] allows instances of this struct to be displayed and edited in the Unity Inspector.
[System.Serializable]
public struct Hex
{
    public readonly int q; // The Q-coordinate (horizontal axis in axial system)
    public readonly int r; // The R-coordinate (diagonal axis in axial system)
    public int s => -q - r; // The S-coordinate, derived for cube coordinate compatibility (q + r + s = 0)

    // Constructor to initialize a Hex with its q and r coordinates.
    public Hex(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    // Defines the outer radius of a hexagon in Unity world units.
    // This value should be adjusted based on the size of your 3D models for hex tiles.
    public static float HEX_SIZE = 1f;

    // Basis vectors for converting axial coordinates to world coordinates.
    // These are derived from hexagonal geometry, specifically for a pointy-top orientation.
    // (Reference: Red Blob Games - https://www.redblobgames.com/grids/hexagons/)
    private static readonly Vector2 Q_BASIS = new Vector2(Mathf.Sqrt(3f) * HEX_SIZE, 0f);
    private static readonly Vector2 R_BASIS = new Vector2(Mathf.Sqrt(3f) / 2f * HEX_SIZE, 1.5f * HEX_SIZE);

    // Converts the logical hex coordinates to a 3D world position (typically on the XZ plane in Unity).
    // yOffset allows for vertical positioning, useful for layered effects or "stair" visuals.
    public Vector3 ToWorldPosition(float yOffset = 0f)
    {
        float x = Q_BASIS.x * q + R_BASIS.x * r;
        float z = Q_BASIS.y * q + R_BASIS.y * r; // Unity's Z-axis is typically depth, while Y is up.
        return new Vector3(x, yOffset, z);
    }

    // Returns an enumerable collection of all six direct neighbors of this hex.
    // This is crucial for movement, range calculations, and pathfinding.
    public IEnumerable<Hex> GetNeighbors()
    {
        yield return new Hex(q + 1, r);     // East
        yield return new Hex(q, r + 1);     // Northeast
        yield return new Hex(q - 1, r + 1); // Northwest
        yield return new Hex(q - 1, r);     // West
        yield return new Hex(q, r - 1);     // Southwest
        yield return new Hex(q + 1, r - 1); // Southeast
    }

    // Overrides the default Equals method to compare Hex structs by their q and r coordinates.
    // Essential for using Hex as keys in Dictionaries or elements in HashSets.
    public override bool Equals(object obj)
    {
        if (obj is Hex other)
        {
            return q == other.q && r == other.r;
        }
        return false;
    }

    // Overrides the default GetHashCode method for efficient storage and retrieval in hash-based collections.
    public override int GetHashCode()
    {
        return (q * 31) + r; // A simple prime-number based hash for structs.
    }

    // Overloads the equality (==) and inequality (!=) operators for easier comparison of Hex structs.
    public static bool operator ==(Hex a, Hex b) => a.Equals(b);
    public static bool operator !=(Hex a, Hex b) => !(a == b);
} 