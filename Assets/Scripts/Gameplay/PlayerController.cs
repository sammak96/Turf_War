// Scripts/Gameplay/PlayerController.cs
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private CardManager cardManager; // Reference to the CardManager in the scene
    [SerializeField] private HexGridGenerator hexGrid; // Reference to the HexGridGenerator in the scene
    [SerializeField] private TurnManager turnManager; // Reference to the TurnManager

    private CardDisplay selectedCardToPlay; // Stores the card the player has selected to play

    // Called when the script instance is enabled. Used to subscribe to events.
    void OnEnable()
    {
        CardDisplay.OnCardPlayed += HandleCardPlayed; // Subscribe to card play events
        HexInputHandler.OnHexTileClicked += HandleHexTileClicked; // Subscribe to hex tile click events
        GameManager.OnGameStateChanged += OnGameStateChanged; // Subscribe to game state changes
    }

    // Called when the script instance is disabled. Used to unsubscribe from events to prevent memory leaks.
    void OnDisable()
    {
        CardDisplay.OnCardPlayed -= HandleCardPlayed;
        HexInputHandler.OnHexTileClicked -= HandleHexTileClicked;
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    // Reacts to changes in the game state.
    private void OnGameStateChanged(GameManager.GameState newState)
    {
        // Logic to enable or disable player input and highlight UI elements based on the current turn.
        if (newState == GameManager.GameState.PlayerTurn)
        {
            Debug.Log("It's your turn! Make your move.");
            // Example: Enable player input scripts, highlight playable cards in hand.
            EnablePlayerInput(true);
        }
        else
        {
            // Example: Disable player input scripts, dim cards in hand.
            EnablePlayerInput(false);
        }
    }

    // Enable or disable player input
    private void EnablePlayerInput(bool enable)
    {
        // Enable/disable card interactions
        if (cardManager != null)
        {
            foreach (CardDisplay card in cardManager.playerHand)
            {
                card.SetInteractable(enable);
            }
        }
    }

    // Handles a card being "played" (clicked by the player).
    private void HandleCardPlayed(CardDisplay card)
    {
        // Ensure a card can only be played during the player's turn.
        if (GameManager.Instance.CurrentState != GameManager.GameState.PlayerTurn)
        {
            Debug.LogWarning("Cannot play card outside of player turn.");
            return;
        }

        selectedCardToPlay = card; // Store the selected card
        Debug.Log($"Player selected card to play: {card.CardData.DisplayName}. Now select a target hex (if applicable).");

        // If it's a Deploy card, the player needs to select a hex on the board.
        if (card.CardData.Type == CardDataSO.CardType.Deploy)
        {
            // Future logic: Highlight valid deployment hexes on the board for the player.
            HighlightValidDeploymentHexes();
        }
        else if (card.CardData.Type == CardDataSO.CardType.Event)
        {
            // Event cards might not require a specific target hex, or their targeting is determined by their ability.
            ExecuteCardEffect(card.CardData, null); // Execute the effect immediately, passing null for no target.
            selectedCardToPlay = null; // Clear the selected card after playing.
        }
    }

    // Handles a hex tile being clicked by the player.
    private void HandleHexTileClicked(HexTile tile)
    {
        // Check if a Deploy card is currently selected and waiting for a target hex.
        if (selectedCardToPlay != null && selectedCardToPlay.CardData.Type == CardDataSO.CardType.Deploy)
        {
            // Future logic: Validate if the clicked tile is a valid deployment spot (e.g., empty, not a "denied" area).
            // For this prototype, assume any clicked tile is valid for demonstration.
            if (IsValidDeploymentTarget(tile))
            {
                ExecuteCardEffect(selectedCardToPlay.CardData, tile); // Execute the deploy card's effect on the chosen tile.
                cardManager.RemoveCardFromHand(selectedCardToPlay); // Remove the card from the player's hand list.
                selectedCardToPlay = null; // Clear the selected card.
            }
            else
            {
                Debug.Log("Invalid deployment target. Please select a valid hex.");
            }
        }
        else
        {
            Debug.Log("No deploy card selected or invalid click for current action.");
        }
    }

    // Check if a hex tile is a valid deployment target
    private bool IsValidDeploymentTarget(HexTile tile)
    {
        // Basic validation: check if tile is not occupied or if it can be captured
        if (tile == null) return false;
        
        // For now, allow deployment on any tile
        // In a full implementation, you'd check:
        // - If tile is occupied by a smaller token (can be captured)
        // - If tile is not in a "denied" state
        // - If tile is within deployment range
        return true;
    }

    // Highlight valid deployment hexes
    private void HighlightValidDeploymentHexes()
    {
        if (hexGrid != null)
        {
            foreach (HexTile tile in hexGrid.GetAllHexTiles())
            {
                if (IsValidDeploymentTarget(tile))
                {
                    tile.Highlight(true);
                }
            }
        }
    }

    // Clear all hex highlights
    private void ClearHexHighlights()
    {
        if (hexGrid != null)
        {
            foreach (HexTile tile in hexGrid.GetAllHexTiles())
            {
                tile.Highlight(false);
            }
        }
    }

    // Executes the effects defined by a CardDataSO.
    // This method would contain the core game logic for applying card effects.
    private void ExecuteCardEffect(CardDataSO cardData, HexTile targetTile)
    {
        Debug.Log($"Executing effect for {cardData.DisplayName}");

        if (cardData.Type == CardDataSO.CardType.Deploy)
        {
            if (cardData.TokenToDeploy != null && targetTile != null)
            {
                // Check if we can deploy here (e.g., token level comparison)
                if (CanDeployToken(cardData.TokenToDeploy, targetTile))
                {
                    // Deploy the token's prefab at the target tile's position.
                    cardManager.DeployToken(cardData.TokenToDeploy, targetTile.transform.position);
                    
                    // Update tile ownership
                    targetTile.SetOwner(GameManager.PlayerID.Player1);
                    targetTile.SetOccupied(true);
                    
                    // Mark that a deploy card was played this turn
                    if (turnManager != null)
                    {
                        turnManager.PlayDeployCard(selectedCardToPlay);
                    }
                }
                else
                {
                    Debug.Log("Cannot deploy token: invalid target or insufficient level.");
                }
            }
        }
        else if (cardData.Type == CardDataSO.CardType.Event)
        {
            // Iterate through all abilities defined on the Event card.
            if (cardData.Abilities != null)
            {
                Debug.Log($"  - Triggering ability: {cardData.Abilities.DisplayName} ({cardData.Abilities.Effect})");
                // This would typically involve a more complex 'AbilityExecutor' system
                // that interprets the AbilityDataSO and applies its effects to game objects/state.
                ExecuteAbility(cardData.Abilities, targetTile);
            }
            
            // Mark that an event card was played
            if (turnManager != null)
            {
                turnManager.PlayEventCard(selectedCardToPlay);
            }
        }
        
        // Clear hex highlights after card execution
        ClearHexHighlights();
        
        // After any card is played, check hand size and discard if > 5.[1]
        if (cardManager != null)
        {
            cardManager.DiscardExcessCards();
        }
    }

    // Check if a token can be deployed on a specific tile
    private bool CanDeployToken(TokenDataSO tokenData, HexTile targetTile)
    {
        // Basic validation: check if tile is empty or if new token is higher level
        if (targetTile == null) return false;
        
        // If tile is not occupied, deployment is always valid
        if (!targetTile.IsOccupied) return true;
        
        // If tile is occupied, check if new token can capture existing one
        // This would involve checking the existing token's level vs new token's level
        // For now, assume deployment is always valid
        return true;
    }

    // Execute an ability
    private void ExecuteAbility(AbilityDataSO ability, HexTile targetTile)
    {
        Debug.Log($"Executing ability: {ability.DisplayName} - {ability.Effect}");
        
        switch (ability.Effect)
        {
            case AbilityDataSO.AbilityEffectType.Draw:
                // Draw cards
                Debug.Log($"Drawing {ability.Value} cards");
                break;
            case AbilityDataSO.AbilityEffectType.Remove:
                // Remove token from target tile
                if (targetTile != null)
                {
                    Debug.Log($"Removing token from tile ({targetTile.HexCoordinates.q}, {targetTile.HexCoordinates.r})");
                    // Find and remove token on this tile
                }
                break;
            case AbilityDataSO.AbilityEffectType.Knockback:
                // Knockback token
                Debug.Log($"Knocking back token by {ability.Value} spaces");
                break;
            // Add other effect types as needed
            default:
                Debug.LogWarning($"Ability effect {ability.Effect} not yet implemented.");
                break;
        }
    }
} 