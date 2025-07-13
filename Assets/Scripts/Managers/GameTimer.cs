// Scripts/Managers/GameTimer.cs
using UnityEngine;
using System.Collections; // Required for Coroutines
using TMPro; // For TextMeshProUGUI for UI text display

public class GameTimer : MonoBehaviour
{
    [Header("Game Timer Settings")]
    [SerializeField] private float gameTimeLimitMinutes = 15f; // The total duration of the game in minutes [1]
    [SerializeField] private TextMeshProUGUI gameTimerText; // The UI Text element to display the remaining game time

    private float timeRemaining; // Tracks the time left in the game
    private bool timerRunning = false; // Flag to control if the timer is actively counting down
    private Coroutine gameCountdownCoroutine; // Reference to the running game countdown coroutine

    // Events for timer updates
    public static event System.Action<float> OnGameTimerChanged; // Event for UI updates
    public static event System.Action OnGameTimeExpired; // Event when game time runs out

    void Start()
    {
        timeRemaining = gameTimeLimitMinutes * 60f; // Convert minutes to seconds
        // Subscribe to game state changes to start/stop the timer appropriately.
        GameManager.OnGameStateChanged += OnGameStateChanged;
        UpdateTimerDisplay(timeRemaining); // Initial display of the timer
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks.
        GameManager.OnGameStateChanged -= OnGameStateChanged;
    }

    // Callback method for GameManager.OnGameStateChanged event.
    private void OnGameStateChanged(GameManager.GameState newState)
    {
        // The game timer should only run during active player turns.
        if (newState == GameManager.GameState.PlayerTurn || newState == GameManager.GameState.OpponentTurn)
        {
            if (!timerRunning) // Start timer only if it's not already running
            {
                timerRunning = true;
                if (gameTimerText != null) gameTimerText.gameObject.SetActive(true); // Show timer UI
                gameCountdownCoroutine = StartCoroutine(CountdownGameTimer()); // Start the countdown coroutine
            }
        }
        else
        {
            // Stop the timer if the game transitions out of an active turn state.
            timerRunning = false;
            if (gameCountdownCoroutine != null)
            {
                StopCoroutine(gameCountdownCoroutine);
            }
            if (gameTimerText != null) gameTimerText.gameObject.SetActive(false); // Hide timer UI
        }
    }

    // Coroutine to handle the main game countdown.
    private IEnumerator CountdownGameTimer()
    {
        while (timeRemaining > 0 && timerRunning)
        {
            timeRemaining -= Time.deltaTime; // Decrement time by the time passed since last frame
            UpdateTimerDisplay(timeRemaining); // Update the UI display
            OnGameTimerChanged?.Invoke(timeRemaining); // Notify listeners of timer change
            yield return null; // Wait for the next frame
        }

        // If the timer runs out and it was actively running, trigger the game end.
        if (timeRemaining <= 0 && timerRunning)
        {
            timeRemaining = 0; // Ensure timer doesn't go negative on display
            UpdateTimerDisplay(timeRemaining); // Final update to show 00:00
            Debug.Log("Game time limit reached!");
            OnGameTimeExpired?.Invoke(); // Notify listeners that time expired
            GameManager.Instance.UpdateGameState(GameManager.GameState.GameEnd); // Transition to GameEnd state
        }
    }

    // Formats the time into a "MM:SS" string and updates the UI text.
    private void UpdateTimerDisplay(float timeToDisplay)
    {
        // Ensure time doesn't go negative for display purposes.
        if (timeToDisplay < 0) timeToDisplay = 0;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60); // Calculate whole minutes
        float seconds = Mathf.FloorToInt(timeToDisplay % 60); // Calculate remaining seconds
        // Format the string to ensure two digits for both minutes and seconds (e.g., 05:30).[40, 41, 44, 45]
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (gameTimerText != null)
        {
            gameTimerText.text = timeString;
        }
    }

    // Public methods for external access
    public float GetTimeRemaining() => timeRemaining;
    public float GetTimeLimit() => gameTimeLimitMinutes * 60f;
    public bool IsTimerRunning() => timerRunning;
    
    // Method to pause/resume the timer
    public void SetTimerRunning(bool running)
    {
        timerRunning = running;
        if (running && gameCountdownCoroutine == null)
        {
            gameCountdownCoroutine = StartCoroutine(CountdownGameTimer());
        }
        else if (!running && gameCountdownCoroutine != null)
        {
            StopCoroutine(gameCountdownCoroutine);
            gameCountdownCoroutine = null;
        }
    }
    
    // Method to add time to the timer (for power-ups or special effects)
    public void AddTime(float secondsToAdd)
    {
        timeRemaining += secondsToAdd;
        UpdateTimerDisplay(timeRemaining);
    }
    
    // Method to set the timer to a specific value
    public void SetTime(float newTime)
    {
        timeRemaining = Mathf.Clamp(newTime, 0, gameTimeLimitMinutes * 60f);
        UpdateTimerDisplay(timeRemaining);
    }
} 