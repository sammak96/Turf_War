// Scripts/ScriptableObjects/LeaderDataSO.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewLeaderData", menuName = "TurfWar/Leader Data", order = 3)]
public class LeaderDataSO : BaseGameDataSO
{
    [Header("Leader Properties")]
    [SerializeField] private Sprite leaderIcon; // The visual icon representing the leader
    [SerializeField] private string passiveAbilityDescription; // Description of the leader's passive ability
    [SerializeField] private AbilityDataSO passiveAbility; // Reference to the AbilityDataSO defining the passive ability

    // Public properties for read-only access.
    public Sprite Icon => leaderIcon;
    public string PassiveDescription => passiveAbilityDescription;
    public AbilityDataSO PassiveAbility => passiveAbility;
}