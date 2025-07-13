// Scripts/Managers/GameManager.cs
using UnityEngine;
using System.Collections; // Required for Coroutines for time-based operations
using System.Collections.Generic; // For List<T>

public class GameManager : MonoBehaviour
{
    // Singleton instance: This static property provides a global access point to the single GameManager instance.
    // 'get; private set;' ensures it can be read publicly but only set within this class.
    public static GameManager Instance { get; private set; }

    // PlayerID enum can be used to distinguish between players in a multiplayer context.
    public enum PlayerID { Player1, Player2 }

    // GameState enum defines the distinct phases of the game, crucial for turn-based logic.
    public enum GameState
    {
        GameSetup,      // Initial phase to set up the board, decks, etc.
        PlayerTurn,     // Active player's main action phase
        OpponentTurn,   // Non-active player's phase (AI or remote player)
        ReactionPhase,  // Special phase for reactive plays [1]
        GameEnd         // Game conclusion, scoring, and winner declaration
    }

    // exposes the CurrentState property in the Inspector for easy monitoring.
    public GameState CurrentState { get; private set; }

    // C# event: A powerful way to implement loose coupling. Other scripts can subscribe to this event
    // to be notified when the game state changes, without needing direct references to the GameManager.
    public static event System.Action<GameState> OnGameStateChanged;

    // References to other managers/systems (assigned in Inspector)
    private HexGridGenerator hexGridGenerator;
    private CardManager cardManager;
    private TurnManager turnManager; // Reference to the TurnManager script
    private LeaderDataSO defaultLeaderSO; // For initial leader assignment
    private TokenDataSO defaultAlphaTokenSO; // For initial alpha token assignment
    private List<CardDataSO> allAvailableCards; // All cards in the game, for deck initialization

    // Player-specific data (simplified for prototype)
    public List<CardDataSO> Player1Deck = new List<CardDataSO>();
    public List<CardDataSO> Player2Deck = new List<CardDataSO>();
    public List<Hex> player1Turfs = new List<Hex>();
    public List<Hex> player2Turfs = new List<Hex>();
    public LeaderController player1Leader; // Reference to Player 1's LeaderController
    public TokenController player1TokenAlpha; // Reference to Player 1's Alpha TokenController
    // Add similar for Player 2

    private bool isPlayer1Turn; // Tracks whose turn it is

    private void Awake()
    {
        // Singleton enforcement: Ensures that only one GameManager instance exists.
        // If another instance is found, destroy this one.
        if (Instance!= null && Instance!= this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad keeps the GameManager GameObject alive across scene loads,
            // useful if it manages global game state from a main menu to game scene.
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        // Start the game in the initial setup state.
        UpdateGameState(GameState.GameSetup);
    }

    // Central method to change the game's state.
    // It triggers specific handler methods and notifies all subscribers.
    public void UpdateGameState(GameState newState)
    {
        CurrentState = newState; // Update the current state variable

        // Use a switch statement to execute logic specific to each game state.
        switch (newState)
        {
            case GameState.GameSetup:
                StartCoroutine(HandleGameSetup()); // Start setup as a coroutine
                break;
            case GameState.PlayerTurn:
                turnManager.StartPlayerTurn(); // Delegate turn handling to TurnManager
                break;
            case GameState.OpponentTurn:
                turnManager.StartOpponentTurn(); // Delegate turn handling to TurnManager
                break;
            case GameState.ReactionPhase:
                // ReactionHandler will subscribe to OnGameStateChanged and manage this phase
                break;
            case GameState.GameEnd:
                HandleGameEnd();
                break;
            default:
                Debug.LogError("Unknown game state: " + newState);
                break;
        }

        // Invoke the event, notifying all listening scripts about the state change.
        OnGameStateChanged?.Invoke(newState);
    }

    // Placeholder methods for state-specific logic, actual implementation details will be in other managers.
    private IEnumerator HandleGameSetup()
    {
        Debug.Log("Game State: Setting Up Game...");

        // 1. Generate Hex Grid and Distribute Blocks [1]
        // Assuming hexGridGenerator is assigned in the Inspector.
        hexGridGenerator.GenerateGrid();
        hexGridGenerator.DistributeBlocksRandomly();
        yield return new WaitForSeconds(1f); // Simulate some loading time or animation

        // 2. Initialize Player Decks (simplified for prototype)
        // In a full game, this would load from player's collection data.
        Player1Deck = new List<CardDataSO>(allAvailableCards); // Example: populate with all available cards
        Player2Deck = new List<CardDataSO>(allAvailableCards);
        ShuffleDeck(Player1Deck);
        ShuffleDeck(Player2Deck);
        Debug.Log("Decks shuffled.");
        yield return new WaitForSeconds(0.5f);

        // 3. Draw Starting Hand [1]
        for (int i = 0; i < 4; i++)
        {
            cardManager.DrawCard(DrawCardFromDeck(Player1Deck)); // Assuming DrawCardFromDeck exists in GameManager
            cardManager.DrawCard(DrawCardFromDeck(Player2Deck)); // Draw for Player 2 as well
        }
        Debug.Log("Starting hands drawn.");
        yield return new WaitForSeconds(0.5f);

        // 4. Random Coin Flip for First Player [1]
        isPlayer1Turn = (Random.Range(0, 2) == 0);
        Debug.Log($"Coin flip: Player {(isPlayer1Turn? "1" : "2")} goes first.");
        yield return new WaitForSeconds(0.5f);

        // 5. Leader and Alpha Selection (Simplified: assume pre-selected or basic UI interaction) [1]
        // For prototype, you might just assign default leaders/alphas.
        // Ensure player1Leader and player1TokenAlpha GameObjects exist in scene and are referenced.
        player1Leader.Initialize(defaultLeaderSO);
        // Place alpha on a starting hex (e.g., center hex or a specific spawn point).
        // This Hex(0,0) is an example, you'd need a proper spawn point.
        player1TokenAlpha.Initialize(defaultAlphaTokenSO, new Hex(0, 0));
        //... similar initialization for Player 2
        Debug.Log("Leaders and Alphas selected.");
        yield return new WaitForSeconds(0.5f);

        // 6. Secret Turf Assignment (Simplified: assign random hexes as turfs) [1]
        AssignRandomTurfs(player1Turfs, 3);
        AssignRandomTurfs(player2Turfs, 3);
        Debug.Log("Turfs assigned secretly.");
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Game Setup Complete. Starting First Turn.");
        // Transition to the first player's turn.
        UpdateGameState(isPlayer1Turn? GameState.PlayerTurn : GameState.OpponentTurn);
    }

    // Helper method to shuffle a list (Fisher-Yates shuffle algorithm)
    private void ShuffleDeck(List<CardDataSO> deck)
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            CardDataSO temp = deck[i];
            deck[i] = deck[rnd];
            deck[rnd] = temp;
        }
    }

    // Helper method to draw a card from the top of the deck
    public CardDataSO DrawCardFromDeck(List<CardDataSO> deck)
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("Deck is empty, cannot draw card.");
            return null; // Handle empty deck scenario
        }
        CardDataSO card = deck[0]; // Get the top card
        deck.RemoveAt(0); // Remove it from the deck
        return card;
    }

    // Helper method to assign random turfs to a player
    private void AssignRandomTurfs(List<Hex> turfList, int count)
    {
        // This is a very basic random assignment. In a real game,
        // ensure turfs are valid (e.g., not occupied by starting tokens) and distinct.
        List<Hex> allHexes = new List<Hex>(hexGridGenerator.hexTiles.Keys); // Get all available hex coordinates
        for (int i = 0; i < count; i++)
        {
            if (allHexes.Count == 0)
            {
                Debug.LogWarning("Not enough unique hexes to assign turfs.");
                break;
            }
            int randomIndex = Random.Range(0, allHexes.Count);
            turfList.Add(allHexes[randomIndex]); // Add the randomly selected hex as a turf
            allHexes.RemoveAt(randomIndex); // Remove it from the pool to prevent duplicate assignments
        }
    }

    private void HandleGameEnd()
    {
        Debug.Log("Game State: Game Over!");
        // Calculate scores, determine winner [1]
        // This logic would be more detailed, potentially in a separate ScoreManager.
        CalculateAndDeclareWinner();
    }

    private void CalculateAndDeclareWinner()
    {
        // Placeholder: You'd need a way to track which player owns which hex tile throughout the game.
        // For example, each HexTile could have a 'PlayerID owner' property that is updated when a token is deployed.

        int player1AssignedTurfsCaptured = CalculateCapturedAssignedTurfs(player1Turfs, PlayerID.Player1);
        int player2AssignedTurfsCaptured = CalculateCapturedAssignedTurfs(player2Turfs, PlayerID.Player2);

        int player1TotalTurfs = CalculateTotalTurfs(PlayerID.Player1);
        int player2TotalTurfs = CalculateTotalTurfs(PlayerID.Player2);

        string winnerMessage = "It's a Draw!";

        // First, compare assigned turfs [1]
        if (player1AssignedTurfsCaptured > player2AssignedTurfsCaptured)
        {
            winnerMessage = "Player 1 Wins!";
        }
        else if (player2AssignedTurfsCaptured > player1AssignedTurfsCaptured)
        {
            winnerMessage = "Player 2 Wins!";
        }
        else // Assigned turfs are tied, then compare total turfs [1]
        {
            if (player1TotalTurfs > player2TotalTurfs)
            {
                winnerMessage = "Player 1 Wins (more total turfs)!";
            }
            else if (player2TotalTurfs > player1TotalTurfs)
            {
                winnerMessage = "Player 2 Wins (more total turfs)!";
            }
        }

        Debug.Log(winnerMessage);
        // Trigger end game UI display (Section 6.1)
    }

    // Methods for calculating turf ownership.
    // These iterate through all HexTiles and check their 'owner' property.
    private int CalculateCapturedAssignedTurfs(List<Hex> assignedTurfs, PlayerID player)
    {
        int count = 0;
        foreach (Hex turfHex in assignedTurfs)
        {
            HexTile tile = hexGridGenerator.GetHexTile(turfHex);
            if (tile != null && tile.Owner == player)
            {
                count++;
            }
        }
        return count;
    }

    private int CalculateTotalTurfs(PlayerID player)
    {
        int count = 0;
        foreach (HexTile tile in hexGridGenerator.GetAllHexTiles())
        {
            if (tile.Owner == player)
            {
                count++;
            }
        }
        return count;
    }
}
