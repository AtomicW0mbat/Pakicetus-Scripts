/* Written by Cameron Williams
 * The purpose of this script is to manage the resources that the player will
 * collect throughout the game as well as handle references to all of the costs
 * of producing tech research and upgrades.
 * 
 * This description last updated 8/21/2019.
 */

using System;
using System.Collections;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; } // as inherited from IGameManager
    
    [NonSerialized] public int[] suppliesInInventory = new int[Enum.GetNames(typeof(Supplies)).Length]; // Array of supplies the player currently has
    [SerializeField] private int[] startingSupplies = new int[Enum.GetNames(typeof(Supplies)).Length];
    [NonSerialized] private int[] suppliesGatherable = new int[Enum.GetNames(typeof(Supplies)).Length]; // Array of gatherables supplies from current gathering point occupied, if any
    [NonSerialized] private GatheringTrigger.gatheringType currentGatheringPoint;
    [NonSerialized] private int[] _suppliesCost = new int[Enum.GetNames(typeof(Supplies)).Length];
    [SerializeField] private AudioSource gatherSound = null;
    // The size of these arrays will always in the order with which they are listed in the referenced enumeration, Supplies

    [Header("Food Consumption")]
    [SerializeField] private float timeBetweenMeals = 5.0f;
    [SerializeField] private int foodConsumedByMeals = 3;

    [Header("Tech Cost Data")]
    // All TechCostData files may be found in the \Scripts\TechCosts subdirectory of the Assets folder
    [SerializeField] public TechCostData armorICost;
    [SerializeField] public TechCostData armorIICost;
    [SerializeField] public TechCostData boostCost;
    [SerializeField] public TechCostData empProtectionCost;
    [SerializeField] public TechCostData energyStorageCost;
    [SerializeField] public TechCostData engineICost;
    [SerializeField] public TechCostData engineIICost;
    [SerializeField] public TechCostData filtrationICost;
    [SerializeField] public TechCostData filtrationIICost;
    [SerializeField] public TechCostData miningICost;
    [SerializeField] public TechCostData miningIICost;
    [SerializeField] public TechCostData navigationICost;
    [SerializeField] public TechCostData navigationIICost;
    [SerializeField] public TechCostData shieldCost;
    [SerializeField] public TechCostData shieldResearchCost;
    [SerializeField] public TechCostData torpedoesEMPCost;
    [SerializeField] public TechCostData torpedoesHECost;
    [SerializeField] public TechCostData torpedoesSeekingCost;
    [SerializeField] public TechCostData turretCost;
    private float timeToGather = 2.0f;

    public void Startup() // As inherited by IGameManager
    {
        // // Debug.Log("Inventory manager starting...");
        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            suppliesInInventory[i] += startingSupplies[i];
        }

        StartCoroutine(FoodConsumption());

        status = ManagerStatus.Started;
    }

    private IEnumerator FoodConsumption() // Implements timeBetweenMeals and foodConsumedByMeals to represent the consumption of food along the journey
    {
        yield return new WaitForSeconds(timeBetweenMeals);
        RemoveFood(foodConsumedByMeals);
        StartCoroutine(FoodConsumption());
    }

    public void SetSuppliesGatherable(int[] suppliesProvided) // Change supplies gatherable to match that of the gathering point the player has entered
    {
        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            suppliesGatherable[i] = suppliesProvided[i];
        }
    }

    public GatheringTrigger.gatheringType GetGatheringPointType() // returns the type of gathering point the player is currently visiting
    {
        return currentGatheringPoint;
    }

    public void SetGatheringPointType(GatheringTrigger.gatheringType inputGatheringType) // set the gathering point that the player is currently at for later referencing
    {
        currentGatheringPoint = inputGatheringType;
        Debug.Log("Current gathering point type set to " + inputGatheringType);
    }

    public void ResetSuppliesGatherable() // Change supplies gatherable back to zero when the player exits a gathering point
    {
        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            suppliesGatherable[i] = 0;
        }
    }

    public void SetSuppliesCost(int[] suppliesCost) // Used to display cost of buildable structures when the player enters their trigger volume
    {
        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            _suppliesCost[i] = suppliesCost[i];
        }
    }

    public int[] GetSuppliesCost() // Used to retrive the suppliesCost if necessary
    {
        return _suppliesCost;
    }

    public void ResetSuppliesCost() // Reset supplies cost to avoid potential conflicts when entering new buildable structure triggers
    {
        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            _suppliesCost[i] = 0;
        }
    }

    public int[] GetSupplies() // Gives the amount of suppplies the player currently has
    {
        return suppliesInInventory;
    }

    public void RemoveSupplies(int[] suppliesCost) // Remove set supply cost from inventory
    {
        for (int i = 0; i < suppliesInInventory.Length; i++)
        {
            suppliesInInventory[i] -= suppliesCost[i];
        }
        MainManager.UI.UpdateSupplyLabels();
    }

    public IEnumerator Gather() // Coroutine for adding supplies to the players inventory depending on the amount currently gatherable; updates UI accordingly
    {
        float gatheringMultiplier = SetGatheringMultiplier();
        float gatheringTimer = 0.0f;
        while (MainManager.Player.GetMining()) // stop gathering when the player switches back to movement mode
        {
            gatheringTimer += Time.deltaTime;
            MainManager.UI.gatheringProgressFill.fillAmount = gatheringTimer / timeToGather;
            yield return new WaitForSeconds(timeToGather);
            if (!gatherSound.isPlaying)
            {
                gatherSound.Play();
            }
            
            for (int i = 0; i < suppliesInInventory.Length; i++)
            {
                suppliesInInventory[i] += suppliesGatherable[i];
                if (suppliesInInventory[i] > 999)
                {
                    suppliesInInventory[i] = 999;
                }
            }
            MainManager.UI.UpdateSupplyLabels();
        }
    }

    public void GatherFood(int foodAmount) // Function used to add food to the player's inventory when colliding with food fish
    {
        suppliesInInventory[(int)Supplies.food] += foodAmount;
        MainManager.UI.UpdateSupplyLabels();
    }

    public void RemoveFood(int foodAmount) // Function used to remove a specified amount of food from the supplies array
    {
        suppliesInInventory[(int)Supplies.food] -= foodAmount;
        MainManager.UI.UpdateSupplyLabels();

        if (suppliesInInventory[(int)Supplies.food] < 0)
        {
            suppliesInInventory[(int)Supplies.food] = 0;
        }
        MainManager.UI.UpdateHull();
        if (suppliesInInventory[(int)Supplies.food] == 0)
        {
            MainManager.Scene.PauseGame();
            MainManager.UI.GameOver.SetActive(true);
        }
    }

    private float SetGatheringMultiplier() // Increases rate of gathering depending on gathering/filtration upgrades' status
    {
        if (currentGatheringPoint == GatheringTrigger.gatheringType.Collection)
        {
            if (MainManager.Player.collectionII)
            {
                return 2.0f;
            }
            else if (!MainManager.Player.collectionII && MainManager.Player.collectionI)
            {
                return 1.5f;
            }
            else
            {
                return 1.0f;
            }
        }

        else // must be GatheringTrigger.gatheringType.Filtration
        {
            if (MainManager.Player.filtrationII)
            {
                return 2.0f;
            }
            else if (!MainManager.Player.filtrationII && MainManager.Player.filtrationI)
            {
                return 1.5f;
            }
            else
            {
                return 1.0f;
            }
        }
    }

    public void DecrementMagnesium() // Function used to remove magnesium from the player's inventory to simulate use of magnesium in generating power
    {
        suppliesInInventory[(int)Supplies.magnesium]--;
        if (suppliesInInventory[(int)Supplies.magnesium] < 0)
        {
            suppliesInInventory[(int)Supplies.magnesium] = 0;
        }
        MainManager.UI.UpdateSupplyLabels();
        if (suppliesInInventory[(int)Supplies.magnesium] == 0)
        {
            MainManager.Scene.PauseGame();
            MainManager.UI.GameOver.SetActive(true);
        }
    }
}