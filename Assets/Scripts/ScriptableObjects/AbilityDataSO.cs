// Scripts/ScriptableObjects/AbilityDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbilityData", menuName = "TurfWar/Ability Data", order = 2)]
public class AbilityDataSO : BaseGameDataSO
{
    // Enums to categorize when an ability triggers and what type of effect it has,
    // directly mapping to the "Abilities" section of the game design.[1]
    public enum AbilityTriggerType { OnPlay, OnDeploy, OnTurnStart, OnTurnEnd, OnReaction }
    public enum AbilityEffectType { Draw, Shuffle, Deploy, Remove, Negate, Immunity, Deny, Knockback, Recycle, Discard, Peek, Copy, Decoy, UpgradeToken, RollDice }

    [Header("Ability Properties")]
    [SerializeField] private AbilityTriggerType triggerType; // When this ability activates
    [SerializeField] private AbilityEffectType effectType; // The type of effect this ability performs
    [SerializeField] private int effectValue; // A generic value (e.g., number of cards to draw, spots to knock back)
    [SerializeField] private float duration; // For effects with a time duration (e.g., Immunity, Deny)
    [SerializeField] private string targetTag; // For abilities targeting specific types of tokens or tiles

    // Public properties for read-only access.
    public AbilityTriggerType Trigger => triggerType;
    public AbilityEffectType Effect => effectType;
    public int Value => effectValue;
    public float Duration => duration;
    public string TargetTag => targetTag;
}