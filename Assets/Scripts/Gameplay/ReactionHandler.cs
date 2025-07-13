// Scripts/Gameplay/ReactionHandler.cs
using UnityEngine;
using System.Collections; // Required for Coroutines
using UnityEngine.UI; // For UI elements like GameObject, Text, Button
using TMPro; // For TextMeshProUGUI for better text rendering

public class ReactionHandler : MonoBehaviour
{
    [Header("Reaction UI")]
    [SerializeField] private GameObject reactionWindowPanel; // The UI Panel GameObject to show/hide
    [SerializeField] private TextMeshProUGUI reactionTimerText; // Text element to display the countdown
    [SerializeField] private Button dismissButton; // Button for the opponent to manually dismiss the reaction window
    [SerializeField] private Button reactButton; // Button for the opponent to play a reactive card

    [Header("Reaction Settings")]
    [SerializeField] private float reactionTimeLimit = 10f; // The fixed duration for the reaction window [1]

    private Coroutine reactionTimerCoroutine; // Reference to the running countdown coroutine
    private bool isReactionActive = false; // Track if reaction phase is currently active

    void Start()
    {
        // Subscribe to game state changes to activate/deactivate the reaction window.
        GameManager.OnGameStateChanged += OnGameStateChanged;
        
        // Add listeners to the buttons
        if (dismissButton != null)
        {
            dismissButton.onClick.AddListener(DismissReaction);
        }
        if (reactButton != null)
        {
            reactButton.onClick.AddListener(PlayReactiveCard);
        }
        
        // Initially hide the reaction window panel.
        if (reactionWindowPanel != null)
        {
            reactionWindowPanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks.
        GameManager.OnGameStateChanged -= OnGameStateChanged;
        
        // Remove button listeners
        if (dismissButton != null)
        {
            dismissButton.onClick.RemoveListener(DismissReaction);
        }
        if (reactButton != null)
        {
            reactButton.onClick.RemoveListener(PlayReactiveCard);
        }
    }

    // Callback method for GameManager.OnGameStateChanged event.
    private void OnGameStateChanged(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.ReactionPhase)
        {
            StartReactionPhase(); // Activate the reaction phase
        }
        else
        {
            EndReactionPhase(); // Deactivate the reaction phase (hide UI)
        }
    }

    // Initiates the reaction phase, showing the UI and starting the timer.
    private void StartReactionPhase()
    {
        Debug.Log("Reaction phase started for opponent.");
        isReactionActive = true;
        
        if (reactionWindowPanel != null)
        {
            reactionWindowPanel.SetActive(true); // Show the UI panel [42, 43]
        }
        
        reactionTimerCoroutine = StartCoroutine(CountdownReactionTimer()); // Start the countdown coroutine
        
        // At this point, the opponent would be enabled to play reactive event cards.
        // This would involve enabling their hand UI for event cards and disabling other inputs.
        EnableReactiveInputs(true);
    }

    // Coroutine to handle the countdown for the reaction window.
    private IEnumerator CountdownReactionTimer()
    {
        float timer = reactionTimeLimit; // Initialize timer with the limit
        while (timer > 0 && isReactionActive)
        {
            timer -= Time.deltaTime; // Decrement timer by the time passed since last frame
            // Update the UI text to display the remaining time, rounded up to the nearest whole second.
            if (reactionTimerText != null)
            {
                reactionTimerText.text = Mathf.CeilToInt(timer).ToString();
            }
            yield return null; // Wait for the next frame
        }
        
        if (isReactionActive)
        {
            Debug.Log("Reaction time expired. Auto-dismissing.");
            DismissReaction(); // Automatically dismiss the window if the timer runs out
        }
    }

    // Method to dismiss the reaction window, either manually or automatically.
    public void DismissReaction()
    {
        if (reactionTimerCoroutine != null)
        {
            StopCoroutine(reactionTimerCoroutine); // Stop the countdown coroutine if it's still running
        }
        
        isReactionActive = false;
        Debug.Log("Reaction dismissed.");
        
        // Disable reactive inputs
        EnableReactiveInputs(false);
        
        // Transition the game state back to the main turn flow.
        // GameManager would need to determine the correct next state (e.g., back to PlayerTurn or OpponentTurn).
        if (GameManager.Instance != null)
        {
            // For now, transition back to player turn
            // In a full implementation, this would depend on whose turn it was before the reaction
            GameManager.Instance.UpdateGameState(GameManager.GameState.PlayerTurn);
        }
    }

    // Method to play a reactive card
    public void PlayReactiveCard()
    {
        Debug.Log("Opponent played a reactive card!");
        
        // This would involve:
        // 1. Showing a card selection UI for the opponent
        // 2. Allowing them to choose an event card to play
        // 3. Executing the card's effect
        // 4. Then dismissing the reaction window
        
        // For now, just log the action and dismiss
        DismissReaction();
    }

    // Enable or disable reactive inputs
    private void EnableReactiveInputs(bool enable)
    {
        // Enable/disable the reaction buttons
        if (dismissButton != null)
        {
            dismissButton.interactable = enable;
        }
        if (reactButton != null)
        {
            reactButton.interactable = enable;
        }
        
        // In a full implementation, you would also:
        // - Enable/disable the opponent's hand UI for event cards
        // - Show/hide targeting UI if needed
        // - Disable other game inputs during reaction phase
    }

    // Deactivates the reaction phase, hiding the UI.
    private void EndReactionPhase()
    {
        if (reactionWindowPanel != null)
        {
            reactionWindowPanel.SetActive(false); // Hide the UI panel
        }
        
        isReactionActive = false;
        
        // Stop the timer if it's running
        if (reactionTimerCoroutine != null)
        {
            StopCoroutine(reactionTimerCoroutine);
        }
        
        // Disable reactive inputs
        EnableReactiveInputs(false);
    }

    // Public method to trigger reaction phase from other systems
    public void TriggerReactionPhase()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameManager.GameState.ReactionPhase);
        }
    }

    // Getter methods
    public bool IsReactionActive() => isReactionActive;
    public float GetReactionTimeLimit() => reactionTimeLimit;
} 