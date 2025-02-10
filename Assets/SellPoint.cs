using UnityEngine;
using TMPro;

public class SellPoint : MonoBehaviour
{
    [Header("Selling Settings")]
    public int tomatoSellValue = 10;      // Points per fully grown tomato
    public int carrotSellValue = 15;      // Points per fully grown carrot

    [Header("UI Elements")]
    public TMP_Text pointsText;           // UI Text for displaying points

    private int totalPoints = 0;          // Total points earned by the player

    [Header("Player Inventory Reference")]
    public TileInteractable playerInventory; // Reference to the player's inventory

    private bool isPlayerInRange = false; // Track if the player is near the sell point
    private string interactionMessage = "Press 'E' to sell your fully grown plants!";

    void Start()
    {
        // Initialize the points UI when the game starts
        UpdatePointsUI();
    }

    void Update()
    {
        // Check for player interaction
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SellFullyGrownPlants();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void SellFullyGrownPlants()
    {
        if (playerInventory == null)
        {
            Debug.LogError("Player inventory is not assigned.");
            return;
        }

        // Calculate points for fully grown tomatoes and carrots
        int tomatoesSold = playerInventory.tomatoFullyGrownCount;
        int carrotsSold = playerInventory.carrotFullyGrownCount;

        // Update the total points
        totalPoints += tomatoesSold * tomatoSellValue;
        totalPoints += carrotsSold * carrotSellValue;

        // Reset the fully grown counts in the player's inventory
        playerInventory.tomatoFullyGrownCount = 0;
        playerInventory.carrotFullyGrownCount = 0;

        // Update the points UI
        UpdatePointsUI();

        // Update the player's inventory UI
        playerInventory.UpdateSeedCountUI();

        // Log the sold amount
        Debug.Log($"Sold {tomatoesSold} tomatoes and {carrotsSold} carrots for {totalPoints} points.");
    }

    private void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "Points: " + totalPoints;
        }
        else
        {
            Debug.LogError("PointsText UI is not assigned.");
        }
    }

    void OnGUI()
    {
        if (isPlayerInRange)
        {
            // Display the interaction message near the center of the screen
            GUI.Label(new Rect(10, 10, 300, 40), interactionMessage);
        }
    }
}

