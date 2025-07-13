// Scripts/ScriptableObjects/CardDataSO.cs
using UnityEngine;
using TMPro; // Required for TextMeshProUGUI if used in UI

[CreateAssetMenu(fileName = "NewCardData", menuName = "TurfWar/Card Data", order = 1)]
public class CardDataSO : BaseGameDataSO
{
    // Enums provide a way to define a set of named integral constants.
    // Here, they categorize card types and factions, improving readability and maintainability.
    public enum CardType { Deploy, Event } // Distinguish between card types [1]
    public enum Faction { Cats, Dogs, Neutral } // Factions from lore [1]

    // Header attributes organize fields in the Inspector, making complex ScriptableObjects easier to manage.
    [Header("Card Properties")]
    [SerializeField] private CardType cardType; // The type of this card (Deploy or Event)
    [SerializeField] private Faction faction; // The faction this card belongs to
    [SerializeField] private Sprite cardArtwork; // The visual sprite/image for the card's artwork
    [SerializeField] private string cardDescription; // A multi-line text area for the card's description or effect

   
    [SerializeField] private int manaCost; // The cost to play this card, potentially for a mana-based system [1]

    // Fields specific to Deploy cards:
    // A direct reference to a TokenDataSO asset, linking a Deploy card to the type of token it creates.
    // This demonstrates how ScriptableObjects can reference each other, building a data graph.
    [SerializeField] private TokenDataSO tokenToDeploy;

    // Fields specific to Event cards:
    // An array of AbilityDataSO objects, allowing an Event card to trigger multiple effects.
    [SerializeField] private AbilityDataSO abilities;

    // Public properties (read-only) to access the private serialized fields.
    // This is good practice for encapsulation.
    public CardType Type => cardType;
    public Faction CardFaction => faction;
    public Sprite Artwork => cardArtwork;
    public string Description => cardDescription;
    public int ManaCost => manaCost;
    public TokenDataSO TokenToDeploy => tokenToDeploy;
    public AbilityDataSO Abilities => abilities;
}