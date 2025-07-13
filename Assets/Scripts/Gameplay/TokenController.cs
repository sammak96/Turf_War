// Scripts/Gameplay/TokenController.cs
using UnityEngine;

public class TokenController : MonoBehaviour
{
    [Header("Token Properties")]
    [SerializeField] public TokenDataSO TokenData { get; private set; } // The data for this specific token instance
    [SerializeField] public Hex HexPosition { get; private set; } // The current logical hex coordinates of this token
    [SerializeField] private GameManager.PlayerID owner = GameManager.PlayerID.Player1; // Which player owns this token

    // Visual components
    [SerializeField] private MeshRenderer tokenRenderer;
    [SerializeField] private Transform tokenTransform;

    // Status effects
    private bool hasImmunity = false;
    private float immunityEndTime = 0f;

    // Initializes the token with its data and position.
    public void Initialize(TokenDataSO data, Hex hexCoords)
    {
        TokenData = data; // Assign the ScriptableObject data
        HexPosition = hexCoords; // Set the token's logical position on the grid

        // Apply visual properties from TokenDataSO.
        // Example: Scale the token based on its level, making higher-level tokens visually larger.
        if (tokenTransform != null)
        {
            tokenTransform.localScale = Vector3.one * (1f + (int)TokenData.Level * 0.2f);
        }
        
        // You might also set its material, color, or other visual aspects here.
        UpdateVisuals();
    }

    // Update visual appearance based on token state
    private void UpdateVisuals()
    {
        if (tokenRenderer != null && TokenData != null)
        {
            // Set color based on owner
            Color tokenColor = owner == GameManager.PlayerID.Player1 ? Color.blue : Color.red;
            
            // Apply special visual effects for special units and alpha tokens
            if (TokenData.IsAlpha)
            {
                tokenColor = Color.gold; // Alpha tokens are gold
            }
            else if (TokenData.IsSpecial)
            {
                tokenColor = Color.magenta; // Special units are magenta
            }

            tokenRenderer.material.color = tokenColor;
        }
    }

    // Applies a specified effect to this token. This method would be called by an AbilityExecutor.
    public void ApplyEffect(AbilityDataSO.AbilityEffectType effectType, int value, float duration)
    {
        Debug.Log($"Token {TokenData.DisplayName} at {HexPosition} applying effect: {effectType}");
        
        switch (effectType)
        {
            case AbilityDataSO.AbilityEffectType.Remove:
                if (!hasImmunity)
                {
                    Destroy(gameObject); // Remove the token from the game
                    Debug.Log($"Token {TokenData.DisplayName} was removed.");
                    // Additional logic: Notify BoardController to update tile state.
                }
                else
                {
                    Debug.Log($"Token {TokenData.DisplayName} is immune to removal.");
                }
                break;
                
            case AbilityDataSO.AbilityEffectType.Knockback:
                if (!hasImmunity)
                {
                    // Implement movement logic based on 'value' (number of spots to move).
                    Debug.Log($"Token {TokenData.DisplayName} was knocked back by {value} spots.");
                    // This would involve calculating new hex position and moving the token
                }
                break;
                
            case AbilityDataSO.AbilityEffectType.Immunity:
                // Apply an immunity status for a 'duration'. This would involve a status effect system.
                hasImmunity = true;
                immunityEndTime = Time.time + duration;
                Debug.Log($"Token {TokenData.DisplayName} gained immunity for {duration} seconds.");
                break;
                
            case AbilityDataSO.AbilityEffectType.UpgradeToken:
                // Logic to upgrade this token to a higher level.
                // This would involve changing its TokenDataSO or modifying its stats directly.
                Debug.Log($"Token {TokenData.DisplayName} was upgraded.");
                break;
                
            case AbilityDataSO.AbilityEffectType.Deny:
                // Prevent the token from taking actions
                Debug.Log($"Token {TokenData.DisplayName} was denied actions for {duration} seconds.");
                break;
                
            //... handle other AbilityEffectType cases as defined in AbilityDataSO
            default:
                Debug.LogWarning($"Effect type {effectType} not yet implemented for tokens.");
                break;
        }
    }

    // Check and update status effects
    void Update()
    {
        // Check if immunity has expired
        if (hasImmunity && Time.time >= immunityEndTime)
        {
            hasImmunity = false;
            Debug.Log($"Token {TokenData.DisplayName} immunity expired.");
        }
    }

    // Method to move token to a new hex position
    public void MoveToHex(Hex newHex)
    {
        HexPosition = newHex;
        Vector3 newWorldPos = newHex.ToWorldPosition();
        transform.position = newWorldPos;
        Debug.Log($"Token {TokenData.DisplayName} moved to hex ({newHex.q}, {newHex.r})");
    }

    // Method to set the owner of this token
    public void SetOwner(GameManager.PlayerID playerID)
    {
        owner = playerID;
        UpdateVisuals();
    }

    // Getter methods
    public TokenDataSO GetTokenData() => TokenData;
    public Hex GetHexPosition() => HexPosition;
    public GameManager.PlayerID GetOwner() => owner;
    public bool HasImmunity() => hasImmunity;
} 