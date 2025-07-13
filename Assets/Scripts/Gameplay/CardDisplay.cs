// Scripts/Gameplay/CardDisplay.cs
using UnityEngine;
using UnityEngine.UI; // For UI elements like Image
using TMPro; // For TextMeshProUGUI, recommended for better text rendering in UI

public class CardDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image cardFrameImage; // Used to change color based on card type
    [SerializeField] private Button cardButton; // Button component for click handling

    // The CardDataSO instance that this visual card represents.
    public CardDataSO CardData { get; private set; }

    // C# event: Notifies other systems when this card is clicked to be played.
    public static event System.Action<CardDisplay> OnCardPlayed;

    void Start()
    {
        // Set up the button click listener
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnClickPlayCard);
        }
    }

    void OnDestroy()
    {
        // Clean up the button listener to prevent memory leaks
        if (cardButton != null)
        {
            cardButton.onClick.RemoveListener(OnClickPlayCard);
        }
    }

    // Initializes the card's visual display with data from a CardDataSO.
    public void Initialize(CardDataSO data)
    {
        CardData = data; // Assign the data
        UpdateUI(); // Update the visual elements based on this data
    }

    // Updates the UI elements of the card based on its assigned CardDataSO.
    private void UpdateUI()
    {
        if (CardData == null) return; // Ensure data is assigned

        if (artworkImage != null)
            artworkImage.sprite = CardData.Artwork; // Set the card's artwork
        if (nameText != null)
            nameText.text = CardData.DisplayName; // Set the card's name
        if (costText != null)
            costText.text = CardData.ManaCost.ToString(); // Set the card's mana cost
        if (descriptionText != null)
            descriptionText.text = CardData.Description; // Set the card's description

        // Apply visual style based on card type.[1]
        // Deploy cards use pastel colors, Event cards use vibrant colors.
        if (cardFrameImage != null)
        {
            if (CardData.Type == CardDataSO.CardType.Deploy)
            {
                cardFrameImage.color = new Color(0.8f, 0.9f, 1.0f); // Example: Pastel blue
            }
            else // Event card
            {
                cardFrameImage.color = new Color(1.0f, 0.6f, 0.2f); // Example: Vibrant orange
            }
        }
    }

    // This method is called when the player clicks on the card.
    // It would be hooked up to a Unity UI Button component's OnClick() event.
    public void OnClickPlayCard()
    {
        // Basic check: Only allow playing if it's currently the player's turn.
        if (GameManager.Instance.CurrentState == GameManager.GameState.PlayerTurn)
        {
            OnCardPlayed?.Invoke(this); // Invoke the event, notifying listeners that this card was played.
        }
        else
        {
            Debug.Log("It's not your turn to play cards!");
        }
    }

    // Method to enable/disable the card's interactivity
    public void SetInteractable(bool interactable)
    {
        if (cardButton != null)
        {
            cardButton.interactable = interactable;
        }
    }

    // Method to highlight the card (e.g., when it's selected)
    public void Highlight(bool highlight)
    {
        if (cardFrameImage != null)
        {
            Color currentColor = cardFrameImage.color;
            if (highlight)
            {
                cardFrameImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.8f); // Slightly transparent
            }
            else
            {
                cardFrameImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f); // Fully opaque
            }
        }
    }
} 