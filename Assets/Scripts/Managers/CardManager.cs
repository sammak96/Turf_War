// Scripts/Managers/CardManager.cs
using UnityEngine;
using System.Collections.Generic;
using TMPro; // Required for TextMeshProUGUI if used in UI

public class CardManager : MonoBehaviour
{
    [Header("Card Management")]
    [SerializeField] private GameObject cardPrefab; // Assign your generic Card visual prefab (e.g., a UI Panel or 3D Plane with CardDisplay script)
    [SerializeField] private Transform handTransform; // The UI parent transform where cards in hand will be placed (e.g., a Horizontal Layout Group)

    // A list to keep track of the visual CardDisplay instances currently in the player's hand.
    public List<CardDisplay> playerHand = new List<CardDisplay>();

    // Example method to simulate drawing a card from a deck.
    // It takes a CardDataSO (the data for the card) and creates its visual representation.
    public void DrawCard(CardDataSO cardData)
    {
        if (cardData == null)
        {
            Debug.LogWarning("Attempted to draw a null card data.");
            return;
        }

        // Instantiate the visual card prefab as a child of the handTransform.
        GameObject newCardGO = Instantiate(cardPrefab, handTransform);
        // Get the CardDisplay component from the newly created GameObject.
        CardDisplay cardDisplay = newCardGO.GetComponent<CardDisplay>();

        if (cardDisplay != null)
        {
            // Initialize the CardDisplay with the ScriptableObject data, which will update its visuals.
            cardDisplay.Initialize(cardData);
            playerHand.Add(cardDisplay); // Add the visual card to the player's hand list.
            // Additional logic here would arrange cards visually within the hand (e.g., using a Horizontal Layout Group or custom spacing).
            Debug.Log($"Player drew: {cardData.DisplayName}");
        }
        else
        {
            Debug.LogError("CardPrefab is missing a CardDisplay component.");
        }
    }

    // Example method to deploy a token onto the game board.
    // It takes a TokenDataSO (the data for the token) and the target board position.
    public void DeployToken(TokenDataSO tokenData, Vector3 boardPosition)
    {
        if (tokenData == null || tokenData.Prefab == null)
        {
            Debug.LogWarning("Attempted to deploy a null token data or token prefab.");
            return;
        }

        // Instantiate the token's visual prefab at the specified board position.
        GameObject newTokenGO = Instantiate(tokenData.Prefab, boardPosition, Quaternion.identity);
        // If tokens have their own controller script (e.g., TokenController), initialize it here.
        TokenController tokenController = newTokenGO.GetComponent<TokenController>();
        if (tokenController != null)
        {
            // Assuming the token is deployed to a specific Hex, you'd pass its Hex coordinates here.
            // For this example, we'll use a placeholder Hex.
            tokenController.Initialize(tokenData, new Hex((int)boardPosition.x, (int)boardPosition.z));
        }
        Debug.Log($"Deployed token: {tokenData.DisplayName} at {boardPosition}");
    }

    // Method to remove a card from the player's hand
    public void RemoveCardFromHand(CardDisplay card)
    {
        if (playerHand.Contains(card))
        {
            playerHand.Remove(card);
            Destroy(card.gameObject);
        }
    }

    // Method to check if hand size exceeds limit (5 cards as per game design)
    public bool IsHandFull()
    {
        return playerHand.Count >= 5;
    }

    // Method to discard excess cards if hand is full
    public void DiscardExcessCards()
    {
        while (playerHand.Count > 5)
        {
            CardDisplay cardToDiscard = playerHand[playerHand.Count - 1]; // Remove from end of hand
            RemoveCardFromHand(cardToDiscard);
            Debug.Log($"Discarded excess card: {cardToDiscard.CardData.DisplayName}");
        }
    }
} 