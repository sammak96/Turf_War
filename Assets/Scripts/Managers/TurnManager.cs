// Scripts/Managers/TurnManager.cs
using UnityEngine;
using System.Collections; // Required for Coroutines

public class TurnManager : MonoBehaviour
{
    [Header("Turn Settings")]
    [SerializeField] private float turnTimeLimit = 60f; // Each turn is 1 to 2 minutes long [1]
    [SerializeField] private CardManager cardManager; // Reference to CardManager
    [SerializeField] private GameManager gameManager; // Direct reference for clarity in prototype

    // Flags and timers for turn management.
    private bool hasPlayedDeployCardThisTurn = false; // Tracks if the player has used their one deploy card this turn [1]
    private float currentTurnTimer; // Current time remaining in the turn
    private Coroutine turnTimerCoroutine; // Reference to the running turn timer coroutine

    // Events for turn state changes
    public static event System.Action<float> OnTurnTimerChanged; // Event for UI updates
    public static event System.Action OnTurnEnded; // Event when turn ends

    void Start()
    {
        // Subscribe to game state changes to react when a turn begins or ends.
        GameManager.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks when the GameObject is destroyed.
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    // Reacts to changes in the overall game state.
    private void OnGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.PlayerTurn)
        {
            StartPlayerTurn(); // Begin the player's turn sequence
        }
        else if (newState == GameManager.GameState.OpponentTurn)
        {
            StartOpponentTurn(); // Begin the opponent's turn sequence
        }
        // If the game transitions out of an active turn state (e.g., to ReactionPhase or GameEnd),
        // stop any running turn timers.
        if (newState != GameManager.GameState.PlayerTurn && newState != GameManager.GameState.OpponentTurn && turnTimerCoroutine != null)
        {
            StopCoroutine(turnTimerCoroutine);
        }
    }

    // Initiates the sequence of events for the active player's turn.
    public void StartPlayerTurn()
    {
        Debug.Log("Player's turn begins!");
        hasPlayedDeployCardThisTurn = false; // Reset deploy card usage for the new turn
        currentTurnTimer = turnTimeLimit; // Reset the turn timer
        turnTimerCoroutine = StartCoroutine(CountdownTurnTimer()); // Start the countdown

        // Player draws 1 card at the start of their turn.[1]
        // This assumes GameManager has a public method to draw a card from the current player's deck.
        if (gameManager != null && cardManager != null)
        {
            // Draw a card for the current player
            CardDataSO drawnCard = gameManager.DrawCardFromDeck(gameManager.Player1Deck);
            if (drawnCard != null)
            {
                cardManager.DrawCard(drawnCard);
            }
        }
    }

    // Coroutine to handle the countdown timer for the current turn.
    private IEnumerator CountdownTurnTimer()
    {
        while (currentTurnTimer > 0)
        {
            currentTurnTimer -= Time.deltaTime; // Decrement timer by time passed since last frame
            // Update UI timer display (this would be handled by a UI script listening to this value)
            OnTurnTimerChanged?.Invoke(currentTurnTimer);
            yield return null; // Wait for the next frame
        }
        Debug.Log("Turn time expired! Ending turn.");
        EndTurn(); // Automatically end the turn if time runs out
    }

    // Method called when the player attempts to play a Deploy card.
    public void PlayDeployCard(CardDisplay card)
    {
        if (!hasPlayedDeployCardThisTurn) // Check if a deploy card has already been played this turn
        {
            // Logic to process the deploy card.
            // This would involve calling PlayerController.ExecuteCardEffect() for the deploy action.
            hasPlayedDeployCardThisTurn = true; // Mark that a deploy card has been played
            Debug.Log($"Played deploy card: {card.CardData.DisplayName}");
        }
        else
        {
            Debug.LogWarning("Already played a deploy card this turn. Only one allowed per turn.");
        }
    }

    // Method called when the player attempts to play an Event card.
    public void PlayEventCard(CardDisplay card)
    {
        // Logic to process the event card. Event cards can be played multiple times per turn.[1]
        // This would involve calling PlayerController.ExecuteCardEffect() for the event action.
        Debug.Log($"Played event card: {card.CardData.DisplayName}");
    }

    // Method called when the player attempts to use their Leader's skill.
    public void UseLeaderSkill()
    {
        // Logic to use the leader skill. This might involve cooldowns or resource costs.
        // This would involve calling LeaderController.UseActiveSkill() or similar.
        Debug.Log("Leader skill used.");
    }

    // Method to manually end the current player's turn.
    public void EndTurn()
    {
        if (turnTimerCoroutine != null)
        {
            StopCoroutine(turnTimerCoroutine); // Stop the turn timer if it's running
        }
        Debug.Log("Player ended turn.");
        
        // Check hand size and discard cards if hand limit (5) is exceeded.[1]
        if (cardManager != null)
        {
            cardManager.DiscardExcessCards();
        }
        
        // Notify listeners that turn has ended
        OnTurnEnded?.Invoke();
        
        // Transition the game state to the opponent's turn.
        if (gameManager != null)
        {
            gameManager.UpdateGameState(GameManager.GameState.OpponentTurn);
        }
    }

    // Initiates the sequence of events for the opponent's turn (AI or remote player).
    public void StartOpponentTurn()
    {
        Debug.Log("Opponent's turn begins!");
        // For a prototype, this can be a simple delay to simulate opponent actions.
        // In a full game, this would involve AI logic or waiting for network input from another player.
        StartCoroutine(SimulateOpponentTurn());
    }

    // Coroutine to simulate the opponent's turn.
    private IEnumerator SimulateOpponentTurn()
    {
        yield return new WaitForSeconds(3f); // Simulate opponent thinking/acting time
        Debug.Log("Opponent's turn ends.");
        // After opponent's actions, transition back to the player's turn.
        if (gameManager != null)
        {
            gameManager.UpdateGameState(GameManager.GameState.PlayerTurn);
        }
    }

    // Getter methods for turn state
    public bool HasPlayedDeployCardThisTurn() => hasPlayedDeployCardThisTurn;
    public float GetCurrentTurnTimer() => currentTurnTimer;
    public float GetTurnTimeLimit() => turnTimeLimit;
} 