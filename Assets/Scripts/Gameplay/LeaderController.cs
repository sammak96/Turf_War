// Scripts/Gameplay/LeaderController.cs
using UnityEngine;

public class LeaderController : MonoBehaviour
{
    [Header("Leader Properties")]
    [SerializeField] public LeaderDataSO LeaderData { get; private set; } // The data for this leader
    [SerializeField] private GameManager.PlayerID owner = GameManager.PlayerID.Player1; // Which player owns this leader

    // Visual components
    [SerializeField] private SpriteRenderer leaderIconRenderer;
    [SerializeField] private Transform leaderTransform;

    // Cooldown tracking for active skills
    private float lastActiveSkillUseTime = 0f;
    private float activeSkillCooldown = 30f; // 30 second cooldown for active skills

    // Initializes the LeaderController with its associated LeaderDataSO.
    public void Initialize(LeaderDataSO data)
    {
        LeaderData = data; // Assign the leader data
        Debug.Log($"Leader selected: {LeaderData.DisplayName}. Passive Ability: {LeaderData.PassiveDescription}");
        
        // Update visual representation
        UpdateVisuals();
        
        // Immediately apply the passive ability if it's a constant effect,
        // or subscribe to relevant game events (e.g., OnTurnStart) if it's a triggered passive.
        ApplyPassiveAbility();
    }

    // Update visual representation
    private void UpdateVisuals()
    {
        if (leaderIconRenderer != null && LeaderData != null)
        {
            leaderIconRenderer.sprite = LeaderData.Icon;
        }
    }

    // Applies the leader's passive ability.
    private void ApplyPassiveAbility()
    {
        if (LeaderData.PassiveAbility != null)
        {
            Debug.Log($"Applying passive ability: {LeaderData.PassiveAbility.DisplayName}");
            // Logic to apply the passive effect.
            // For example, if the common ability is "deploy 1 rank 1 token for free" [1],
            // this might involve modifying player's resources, adding a special action,
            // or subscribing to a deployment event to grant a free token.
            // This would likely involve calling into a central AbilityExecutor system.
            
            // Subscribe to relevant game events for triggered passive abilities
            SubscribeToGameEvents();
        }
        else
        {
            Debug.LogWarning($"Leader {LeaderData.DisplayName} has no passive ability assigned.");
        }
    }

    // Subscribe to game events for triggered abilities
    private void SubscribeToGameEvents()
    {
        if (LeaderData.PassiveAbility != null)
        {
            switch (LeaderData.PassiveAbility.Trigger)
            {
                case AbilityDataSO.AbilityTriggerType.OnTurnStart:
                    GameManager.OnGameStateChanged += OnTurnStart;
                    break;
                case AbilityDataSO.AbilityTriggerType.OnTurnEnd:
                    GameManager.OnGameStateChanged += OnTurnEnd;
                    break;
                case AbilityDataSO.AbilityTriggerType.OnDeploy:
                    // Subscribe to deployment events
                    break;
                // Add other trigger types as needed
            }
        }
    }

    // Event handlers for triggered abilities
    private void OnTurnStart(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.PlayerTurn && owner == GameManager.PlayerID.Player1)
        {
            TriggerPassiveAbility();
        }
        else if (newState == GameManager.GameState.OpponentTurn && owner == GameManager.PlayerID.Player2)
        {
            TriggerPassiveAbility();
        }
    }

    private void OnTurnEnd(GameManager.GameState newState)
    {
        if (newState == GameManager.GameState.OpponentTurn && owner == GameManager.PlayerID.Player1)
        {
            TriggerPassiveAbility();
        }
        else if (newState == GameManager.GameState.PlayerTurn && owner == GameManager.PlayerID.Player2)
        {
            TriggerPassiveAbility();
        }
    }

    // Trigger the passive ability
    private void TriggerPassiveAbility()
    {
        if (LeaderData.PassiveAbility != null)
        {
            Debug.Log($"Triggering passive ability: {LeaderData.PassiveAbility.DisplayName}");
            // Execute the ability effect
            ExecuteAbility(LeaderData.PassiveAbility);
        }
    }

    // If leaders have active skills that players can manually trigger,
    // this method would contain the logic for using that skill.
    public void UseActiveSkill()
    {
        // Check if the skill is currently available (e.g., cooldown, mana cost).
        if (Time.time - lastActiveSkillUseTime < activeSkillCooldown)
        {
            Debug.Log($"Leader {LeaderData.DisplayName} active skill is on cooldown.");
            return;
        }

        // Execute the active skill
        if (LeaderData.PassiveAbility != null)
        {
            Debug.Log($"Leader {LeaderData.DisplayName} used active skill.");
            ExecuteAbility(LeaderData.PassiveAbility);
            lastActiveSkillUseTime = Time.time;
        }
        // This would also likely interact with the AbilityExecutor system.
    }

    // Execute an ability
    private void ExecuteAbility(AbilityDataSO ability)
    {
        Debug.Log($"Executing ability: {ability.DisplayName} - {ability.Effect}");
        
        // This would typically involve a more complex 'AbilityExecutor' system
        // that interprets the AbilityDataSO and applies its effects to game objects/state.
        switch (ability.Effect)
        {
            case AbilityDataSO.AbilityEffectType.Draw:
                // Draw cards
                Debug.Log($"Drawing {ability.Value} cards");
                break;
            case AbilityDataSO.AbilityEffectType.Deploy:
                // Deploy a free token
                Debug.Log($"Deploying free token of level {ability.Value}");
                break;
            case AbilityDataSO.AbilityEffectType.Shuffle:
                // Shuffle deck
                Debug.Log("Shuffling deck");
                break;
            // Add other effect types as needed
            default:
                Debug.LogWarning($"Ability effect {ability.Effect} not yet implemented for leaders.");
                break;
        }
    }

    // Method to set the owner of this leader
    public void SetOwner(GameManager.PlayerID playerID)
    {
        owner = playerID;
    }

    // Getter methods
    public LeaderDataSO GetLeaderData() => LeaderData;
    public GameManager.PlayerID GetOwner() => owner;
    public bool CanUseActiveSkill() => Time.time - lastActiveSkillUseTime >= activeSkillCooldown;
    public float GetActiveSkillCooldownRemaining() => Mathf.Max(0, activeSkillCooldown - (Time.time - lastActiveSkillUseTime));

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        GameManager.OnGameStateChanged -= OnTurnStart;
        GameManager.OnGameStateChanged -= OnTurnEnd;
    }
} 