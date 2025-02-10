using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileInteractable : MonoBehaviour
{
    public float interactDistance = 3f; // Distance at which the player can interact with the tile
    private GameObject player;          // Reference to the player
    private bool isPlayerInRange = false;

    // UI References
    public GameObject seedSelectionUI;
    public Button tomatoSeedButton;
    public Button carrotSeedButton;
    public Button closeUIButton;
    public TMP_Text tomatoSeedCountText;    // UI Text for tomato seed count
    public TMP_Text carrotSeedCountText;    // UI Text for carrot seed count

    // Inventory UI
    public GameObject inventoryPanel;       // Inventory panel UI
    public TMP_Text inventoryTomatoCount;   // UI Text for tomato seeds in inventory
    public TMP_Text inventoryCarrotCount;   // UI Text for carrot seeds in inventory

    // Inventory
    public int tomatoSeedCount = 5;     // Initial tomato seed count
    public int carrotSeedCount = 5;     // Initial carrot seed count
    public int tomatoFullyGrownCount = 0;     // Fully grown tomato count
    public int carrotFullyGrownCount = 0;     // Fully grown carrot count

    // Seed selection logic
    private string selectedSeed = "";
    private bool isPlanted = false;    // Track if the tile has been planted
    private bool isGrowing = false;   // Track if the plant is growing
    private bool isHarvestable = false; // Track if the plant is harvestable
    private float growthTimer = 0f;   // Timer to track growth progress

    // Adjustable Growth Settings in Inspector
    [Header("Plant Growth Settings")]
    public GameObject seedStage;      // The seed stage model
    public GameObject saplingStage;   // The sapling stage model
    public GameObject tomatoFullStage;   // Tomato full-grown model
    public GameObject carrotFullStage;   // Carrot full-grown model

    private GameObject currentFullStage;  // Holds the current full-grown model for the selected seed

    [Header("UI Interaction Settings")]
    public string interactionMessage = "Press 'E' to plant seed"; // Default message when player is nearby
    public string plantedMessage = "Seed planted, starting to grow..."; // Message after planting
    public string growingMessage = "Growing..."; // Message while growing
    public string harvestMessage = "Press 'E' to harvest the plant"; // Message when the plant is ready for harvest

    // Define growth durations for different seeds
    [Header("Seed Growth Durations")]
    public float tomatoGrowthDuration = 10f; // Duration for tomato growth
    public float carrotGrowthDuration = 15f; // Duration for carrot growth

    private float growthDuration;  // This will be set based on the selected seed

    private string currentMessage = ""; // Current message displayed to the player

    void Start()
    {
        player = GameObject.FindWithTag("Player");

        // Ensure the seed selection UI is hidden initially
        seedSelectionUI.SetActive(false);

        // Ensure the inventory panel is hidden initially
        inventoryPanel.SetActive(false);

        // Add button listeners
        tomatoSeedButton.onClick.AddListener(() => SelectSeed("Tomato"));
        carrotSeedButton.onClick.AddListener(() => SelectSeed("Carrot"));
        closeUIButton.onClick.AddListener(CloseSeedSelection);

        // Ensure all models are inactive at the start, except the seed model
        if (seedStage != null) seedStage.SetActive(false);
        if (saplingStage != null) saplingStage.SetActive(false);
        if (tomatoFullStage != null) tomatoFullStage.SetActive(false);
        if (carrotFullStage != null) carrotFullStage.SetActive(false);

        // Update UI seed counts
        UpdateSeedCountUI();
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player object not found!");
            return;
        }

        if (Vector3.Distance(player.transform.position, transform.position) < interactDistance)
        {
            isPlayerInRange = true;

            // Update the interaction message based on the tile state
            if (!isPlanted)
            {
                currentMessage = interactionMessage; // "Press 'E' to plant seed"
                if (Input.GetKeyDown(KeyCode.E))
                {
                    OpenSeedSelectionUI();
                }
            }
            else if (isHarvestable)
            {
                currentMessage = harvestMessage; // "Press 'E' to harvest the plant"
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Harvest();
                }
            }
            else if (isGrowing)
            {
                currentMessage = growingMessage; // "Growing..."
            }
        }
        else
        {
            isPlayerInRange = false;
            currentMessage = ""; // Clear message when out of range
        }

        if (isGrowing)
        {
            growthTimer += Time.deltaTime;

            if (growthTimer >= growthDuration)
            {
                growthTimer = growthDuration;
                isGrowing = false;
                isHarvestable = true; // Mark the plant as ready for harvest
                Debug.Log(selectedSeed + " is ready for harvest!");
            }

            UpdateGrowthStage();
        }

        // Toggle inventory on "I" key press
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void OpenSeedSelectionUI()
    {
        if (isPlayerInRange && !isPlanted)
        {
            seedSelectionUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Disable buttons if out of seeds
            tomatoSeedButton.interactable = tomatoSeedCount > 0;
            carrotSeedButton.interactable = carrotSeedCount > 0;
        }
    }

    void SelectSeed(string seed)
    {
        if (seed == "Tomato" && tomatoSeedCount > 0)
        {
            selectedSeed = seed;
            growthDuration = tomatoGrowthDuration;
            currentFullStage = tomatoFullStage;

            // Reduce seed count
            tomatoSeedCount--;
        }
        else if (seed == "Carrot" && carrotSeedCount > 0)
        {
            selectedSeed = seed;
            growthDuration = carrotGrowthDuration;
            currentFullStage = carrotFullStage;

            // Reduce seed count
            carrotSeedCount--;
        }

        PlantSeed();
    }

    void PlantSeed()
    {
        if (!string.IsNullOrEmpty(selectedSeed))
        {
            isPlanted = true;
            isGrowing = true;
            growthTimer = 0f;

            Debug.Log("Planted " + selectedSeed + "!");

            CloseSeedSelection();

            if (seedStage != null)
                seedStage.SetActive(true);

            // Update UI seed counts
            UpdateSeedCountUI();
        }
    }

    void CloseSeedSelection()
    {
        seedSelectionUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UpdateGrowthStage()
    {
        float growthPercentage = growthTimer / growthDuration;

        if (growthPercentage < 0.5f)
        {
            SetActiveStage(seedStage, true);
            SetActiveStage(saplingStage, false);
            SetActiveStage(currentFullStage, false);
        }
        else if (growthPercentage >= 0.5f && growthPercentage < 1f)
        {
            SetActiveStage(seedStage, false);
            SetActiveStage(saplingStage, true);
            SetActiveStage(currentFullStage, false);
        }
        else if (growthPercentage >= 1f)
        {
            SetActiveStage(seedStage, false);
            SetActiveStage(saplingStage, false);
            SetActiveStage(currentFullStage, true);
        }
    }

    void SetActiveStage(GameObject stage, bool isActive)
    {
        if (stage != null)
        {
            stage.SetActive(isActive);
        }
    }

    void Harvest()
    {
        Debug.Log("Harvested " + selectedSeed + "!");

        // Increment the fully grown count for the respective seed type
        if (selectedSeed == "Tomato")
        {
            tomatoFullyGrownCount++;
        }
        else if (selectedSeed == "Carrot")
        {
            carrotFullyGrownCount++;
        }

        // Reset plant state
        isPlanted = false;
        isGrowing = false;
        isHarvestable = false;
        growthTimer = 0f;

        SetActiveStage(seedStage, false);
        SetActiveStage(saplingStage, false);
        SetActiveStage(currentFullStage, false);

        selectedSeed = "";

        // Update the UI with the latest seed and fully grown counts
        UpdateSeedCountUI();
    }

    public void UpdateSeedCountUI()
    {
        // Update both the first and second tomato seed count text
        if (tomatoSeedCountText != null)
        {
            tomatoSeedCountText.text = "Tomato Seeds: " + tomatoSeedCount;
        }

        if (carrotSeedCountText != null)
        {
            carrotSeedCountText.text = "Carrot Seeds: " + carrotSeedCount;
        }


        // Update inventory UI with counts
        if (inventoryTomatoCount != null)
        {
            inventoryTomatoCount.text = $"Tomato Seeds: {tomatoSeedCount}\nFully Grown: {tomatoFullyGrownCount}";
        }

        if (inventoryCarrotCount != null)
        {
            inventoryCarrotCount.text = $"Carrot Seeds: {carrotSeedCount}\nFully Grown: {carrotFullyGrownCount}";
        }
    }

    void ToggleInventory()
    {
        // Toggle the inventory panel visibility
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    void OnGUI()
    {
        if (!string.IsNullOrEmpty(currentMessage) && isPlayerInRange)
        {
            GUI.Label(new Rect(10, 10, 200, 30), currentMessage);
        }
    }
}

