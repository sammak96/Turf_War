// Scripts/ScriptableObjects/TokenDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewTokenData", menuName = "TurfWar/Token Data", order = 4)]
public class TokenDataSO : BaseGameDataSO
{
    // Enum for different token levels as described in the game design.[1]
    public enum TokenLevel { Level1, Level2, Level3, Level4_Alpha }

   
    [SerializeField] private TokenLevel tokenLevel; // The level of this token
    [SerializeField] private GameObject tokenPrefab; // The GameObject Prefab representing the visual of this token
    [SerializeField] private int attackValue; // Example: The attack power of the token
    [SerializeField] private int healthValue; // Example: The health points of the token
    [SerializeField] private bool isSpecialUnit; // Flag for Level 3 tokens, indicating special effects [1]
    [SerializeField] private bool isAlphaToken; // Flag for the unique Level 4 Alpha token [1]

    // Public properties for read-only access.
    public TokenLevel Level => tokenLevel;
    public GameObject Prefab => tokenPrefab;
    public int Attack => attackValue;
    public int Health => healthValue;
    public bool IsSpecial => isSpecialUnit;
    public bool IsAlpha => isAlphaToken;
}