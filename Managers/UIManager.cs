/* Written by Cameron Williams
 * 
 * Any logic concerning Game Objects located on the UI Canvas is located in this class
 * with the exception of the Main Menu since it's in a different scene.
 * 
 * Last updated on 8/21/2019.
 */

using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, IGameManager
{
    [Header("Other UI")]
    [SerializeField] [Tooltip("Game Object containing all game over menu UI elements")] public GameObject GameOver;
    [SerializeField] [Tooltip("Game Object containing all pause menu UI elements")] public GameObject PausePanel;

    [Header("HUD")]
    [SerializeField] [Tooltip("Array of resource text labels in order of Supplies enum")] public Text[] supplyLabels = new Text[Enum.GetNames(typeof(Supplies)).Length];
    [SerializeField] [Tooltip("Text label indicating integrity of the ship's hull")] public Text hullText;
    [SerializeField] [Tooltip("Image on HUD indicating the amount of HP the player has left")] public Image hullBar;
    [SerializeField] [Tooltip("Bar under the healthbar indicating the integrity of the front shield")] public Image shieldBar;
    [SerializeField] [Tooltip("Displays percentage of energy currently available")] public Text energyText;
    [SerializeField] [Tooltip("Reference to entire panel that the compass is on")] public GameObject compassPanel;
    [SerializeField] [Tooltip("Slider bar indicating the amount of research complete")] public Slider researchBar;
    [SerializeField] [Tooltip("Text that shows research percentage next to the research bar")] public Text researchPercentage;

    [Header("Tech Panel")]
    [SerializeField] [Tooltip("Game Object containing all tech menu UI elements")] public GameObject TechPanel;
    [SerializeField] public Button armorIButton;
    [SerializeField] public Button armorIIButton;
    [SerializeField] public Button boostButton;
    [SerializeField] public Button empProtectionButton;
    [SerializeField] public Button energyStorageButton;
    [SerializeField] public Button engineIButton;
    [SerializeField] public Button engineIIButton;
    [SerializeField] public Button filtrationIButton;
    [SerializeField] public Button filtrationIIButton;
    [SerializeField] public Button miningIButton;
    [SerializeField] public Button miningIIButton;
    [SerializeField] public Button navigationIButton;
    [SerializeField] public Button navigationIIButton;
    [SerializeField] public Button shieldResearchButton;
    [SerializeField] public Button shieldButton;
    [SerializeField] public Button torpedoesEMPButton;
    [SerializeField] public Button torpedoesHEButton;
    [SerializeField] public Button torpedoesSeekingButton;
    [SerializeField] public Button turret0Button;
    [SerializeField] public Button turret1Button;
    [SerializeField] public Button turret2Button;
    [SerializeField] public Button turret3Button;
    [SerializeField] public Button turret4Button;

    [Header("Turret Control")]
    [SerializeField] public GameObject[] turretActiveIndicator = new GameObject[Enum.GetNames(typeof(PlayerTurretNumberings)).Length];
    [SerializeField] public Image[] torpedoesPiercingToggle = new Image[Enum.GetNames(typeof(PlayerTurretNumberings)).Length];
    [SerializeField] public Image[] torpedoesHEToggle = new Image[Enum.GetNames(typeof(PlayerTurretNumberings)).Length];
    [SerializeField] public Image[] torpedoesEMPToggle = new Image[Enum.GetNames(typeof(PlayerTurretNumberings)).Length];
    [SerializeField] public Image[] torpedoesSeekingIndicator = new Image[Enum.GetNames(typeof(PlayerTurretNumberings)).Length];

    [Header("Torpedo Button Sprites")]
    [SerializeField] public Sprite pierceTorpedoNonInteractable;
    [SerializeField] public Sprite pierceTorpedoInteractable;
    [SerializeField] public Sprite pierceTorpedoMouseover;
    [SerializeField] public Sprite pierceTorpedoSelected;
    [SerializeField] public Sprite hETorpedoNonInteractable;
    [SerializeField] public Sprite hETorpedoInteractable;
    [SerializeField] public Sprite hETorpedoMouseover;
    [SerializeField] public Sprite hETorpedoSelected;
    [SerializeField] public Sprite eMPTorpedoNonInteractable;
    [SerializeField] public Sprite eMPTorpedoInteractable;
    [SerializeField] public Sprite eMPTorpedoMouseover;
    [SerializeField] public Sprite eMPTorpedoSelected;
    [SerializeField] public Sprite seekingTorpedoNonInteractable;
    [SerializeField] public Sprite seekingTorpedoInteractable;
    [SerializeField] public Sprite seekingTorpedoMouseover;
    [SerializeField] public Sprite seekingTorpedoSelected;

    [NonSerialized] public BuildTrigger buildingReference = null; // Contains reference to buildable structure when player enters it's vicinity
    [Header("Building Panel")]
    [SerializeField] [Tooltip("Building Panel containing structure name, cost, and build button")] public GameObject buildPanel;
    [SerializeField] [Tooltip("Structure label")] private Text structureLabel = null;
    [SerializeField] [Tooltip("Array of resource cost labels in order of Supplies enum")] public Text[] buildSupplyCosts = new Text[Enum.GetNames(typeof(Supplies)).Length];
    [SerializeField] [Tooltip("Array of GameObjects that function as labes for how much of what resources is required for building")] public GameObject[] buildSuppplyLabels = new GameObject[Enum.GetNames(typeof(Supplies)).Length];
    [SerializeField] [Tooltip("Button that allows the player to construct structures they are near")] public Button buildButton;


    [Header("Tech Cost Panel")]
    [SerializeField] [Tooltip("Array of tech cost supplies")] public Text[] techSupplyCosts = new Text[Enum.GetNames(typeof(Supplies)).Length];
    [SerializeField] [Tooltip("GameObjects containing supply labels and and associated costs for tech")] public GameObject[] techSupplyCostLabels;
    [SerializeField] [Tooltip("Text box that displays the name of the moused over tech")] public Text techName;
    [SerializeField] [Tooltip("Reference to the text box in the tech cost panel that shows amount of time required to research")] public Slider techResearchTime;
    [SerializeField] [Tooltip("Display name of currently researched tech")] public Text currentResearch;
    [SerializeField] public Sprite hETorpedoSprite;
    [SerializeField] public Sprite boostSprite;
    [SerializeField] public Sprite seekingSprite;
    [SerializeField] public Sprite forwardShieldSprite;
    [SerializeField] public Sprite counterEMPSprite;
    [SerializeField] public Sprite eMPSprite;
    [SerializeField] [Tooltip("Reference to the text box in the tech cost panel that shows the description")] public Text techDescription;
    [SerializeField] [Tooltip("Reference to ths image object on the tech cost panel")] public Image techImage;

    [Header("Gathering Panel")]
    [SerializeField] [Tooltip("The GameObject containing/composing the entirety of the gathering panel")] public GameObject gatheringPanel;
    [SerializeField] [Tooltip("Name of gatherable resource at the top of the panel")] public Text gatheringPanelTitle;
    [SerializeField] [Tooltip("Image that has fill value for representing gathering progress")] public Image gatheringProgressFill;
    [SerializeField] [Tooltip("Array of supplies in order of the supply enum")] public GameObject[] gathering_elementBoxes;

    [Header("Database Panel")]
    [SerializeField] private GameObject encyclopediaNotifciation = null;
    [SerializeField] private GameObject encyclopediaPanel = null;
    [SerializeField] private Button geologySectionButton = null;
    [SerializeField] private GameObject[] geologyEntryButtons = null;
    [SerializeField] private Button crewSectionButton = null;
    [SerializeField] private GameObject[] crewEntryButtons = null;
    [SerializeField] private Button techSectionButton = null;
    [SerializeField] private GameObject[] techEntryButtons = null;
    [SerializeField] private Button storySectionButton = null;
    [SerializeField] private GameObject[] storyEntryButtons = null;
    [SerializeField] private Button enemiesSectionButton = null;
    [SerializeField] private GameObject[] enemiesEntryButtons = null;
    [SerializeField] private Button otherSectionButton = null;
    [SerializeField] private GameObject[] otherEntryButtons = null;
    private GameObject activeObject = null;

    [Header("Map Panel")]
    [SerializeField] private GameObject mapPanel = null;

    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel = null;

    [Header("Area Popup")]
    [SerializeField] private Text areaName = null;
    [SerializeField] private Animation areaName_fadeIn_fadeOut = null;

    [Header("Dialogue")]
    [SerializeField] public AudioSource dialogueSource;


    public ManagerStatus status { get; private set; } // As inherited by IGameManager
    public void Startup() // Inherited by IGameManager
    {
        // Debug.Log("UI manager starting...");

        int[] tempSupplies = MainManager.Inventory.GetSupplies();
        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++) // Set supply labels to initial values
        {
            supplyLabels[i].text = tempSupplies[i].ToString();
        }

        status = ManagerStatus.Started;
    }

    public void Update()
    {
        // Handles keyboard input for opening and closing menus
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTechPanel();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMap();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscape();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleDatabasePanel();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ToggleOptionsPanel();
        }
    }

    public void ToggleDatabasePanel() // Toggles the database panel depending on it's active state
    {
        if (encyclopediaPanel.activeInHierarchy)
        {
            encyclopediaPanel.gameObject.SetActive(false);
            MainManager.Scene.UnPauseGame();
        }
        else
        {
            encyclopediaPanel.gameObject.SetActive(true);
            MainManager.Scene.PauseGame();
        }
    }

    public void ToggleEscape() // Toggles the escape menu panel depending on it's active state
    {
        if (MenuOpen())
        {
            PausePanel.SetActive(false);
            encyclopediaPanel.SetActive(false);
            TechPanel.SetActive(false);
            optionsPanel.SetActive(false);
            if (MainManager.Scene.mapOpen)
            {
                MainManager.Scene.ToggleMapCamera();
            }
            MainManager.Scene.UnPauseGame();
        }
        else
        {
            PausePanel.gameObject.SetActive(true);
            MainManager.Scene.PauseGame();
        }
    }

    public void ToggleTechPanel() // Toggles the tech menu panel depending on it's active state
    {
        if (TechPanel.gameObject.activeInHierarchy)
        {
            TechPanel.gameObject.SetActive(false);
            MainManager.Scene.UnPauseGame();
        }
        else
        {
            TechPanel.gameObject.SetActive(true);
            UpdateTechButtons();
            MainManager.Scene.PauseGame();
        }
    }

    public void ToggleMap() // Toggles the map menu panel depending on it's active state
    {
        MainManager.Scene.ToggleMapCamera();
        if (mapPanel.activeInHierarchy)
        {
            mapPanel.gameObject.SetActive(false);
            MainManager.Scene.UnPauseGame();
        }
        else
        {
            mapPanel.gameObject.SetActive(true);
            MainManager.Scene.PauseGame();
        }
    }

    public void ToggleOptionsPanel() // Toggles the options menu panel depending on it's active state
    {
        if (optionsPanel.activeInHierarchy)
        {
            optionsPanel.gameObject.SetActive(false);
            MainManager.Scene.UnPauseGame();
        }
        else
        {
            optionsPanel.gameObject.SetActive(true);
            MainManager.Scene.PauseGame();
        }
    }

    private bool MenuOpen() // determines if any menu panels are active; used to determine whether Esc should close menus or open the pause menu
    {
        if (PausePanel.activeInHierarchy || encyclopediaPanel.activeInHierarchy || TechPanel.activeInHierarchy || MainManager.Scene.mapOpen || optionsPanel.activeInHierarchy)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ResourceRequirementsMet(TechCostData techData) // helper function for UpdateTechButtons; compare tech cost to player inventory
    {
        int[] suppliesInInventory = MainManager.Inventory.GetSupplies();

        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            if (suppliesInInventory[i] < techData.cost[i])
            {
                return false; // returns false if even a single resource requirment is not met
            }
        }

        return true; // returns true if all resource requirements are met
    }

    public bool ResourceRequirementsMet(int[] suppliesCost) // Overload for the above function that uses supplies array directly
    {
        int[] suppliesInInventory = MainManager.Inventory.GetSupplies();

        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            if (suppliesInInventory[i] < suppliesCost[i])
            {
                return false;
            }
        }
        return true;
    }

    public void TechButtonMouseEnter(TechCostData costToCheck) // Display the required resources for constructing the tech button the mouse entered
    {
        for (int i = 0; i < costToCheck.cost.Length; i++) // assign cost values
        {
            techSupplyCosts[i].text = costToCheck.cost[i].ToString();
            if (costToCheck.cost[i] == 0)
            {
                techSupplyCostLabels[i].SetActive(false); // disable tech cost label if cost is 0
            }
        }
        techName.text = "Upgrade Name: " + costToCheck.techName;
        techDescription.text = "Description: " + costToCheck.description;
    }

    public void UpdateTechButtons() // Update interactability of tech buttons depending on whether they are constructed and whether the their costs are met
    {
        // armorI button logic
        if (ResourceRequirementsMet(MainManager.Inventory.armorICost) && MainManager.Player.armorI == false)
        {
            armorIButton.interactable = true;
        }
        else
        {
            armorIButton.interactable = false;
        }

        // armorII button logic
        if (ResourceRequirementsMet(MainManager.Inventory.armorIICost) && MainManager.Player.armorI == true && MainManager.Player.armorII == false)
        {
            armorIIButton.interactable = true;
        }
        else
        {
            armorIIButton.interactable = false;
        }

        // boost button logic
        if (ResourceRequirementsMet(MainManager.Inventory.boostCost) && MainManager.Player.boost.activeInHierarchy == false && MainManager.Player.researchInProgress == false)
        {
            boostButton.interactable = true;
        }
        else
        {
            boostButton.interactable = false;
        }

        // empProtection button logic
        if (ResourceRequirementsMet(MainManager.Inventory.empProtectionCost) && MainManager.Player.empProtection == false && MainManager.Player.researchInProgress == false)
        {
            empProtectionButton.interactable = true;
        }
        else
        {
            empProtectionButton.interactable = false;
        }

        // energyStorage button logic
        if (ResourceRequirementsMet(MainManager.Inventory.energyStorageCost) && MainManager.Player.energyStorage == false)
        {
            energyStorageButton.interactable = true;
        }
        else
        {
            energyStorageButton.interactable = false;
        }

        // engineI button logic
        if (ResourceRequirementsMet(MainManager.Inventory.engineICost) && MainManager.Player.engineI == false)
        {
            engineIButton.interactable = true;
        }
        else
        {
            engineIButton.interactable = false;
        }

        // engineII button logic
        if (ResourceRequirementsMet(MainManager.Inventory.engineIICost) && MainManager.Player.engineI == true && MainManager.Player.engineII == false)
        {
            engineIIButton.interactable = true;
        }
        else
        {
            engineIIButton.interactable = false;
        }

        // filtrationI button logic
        if (ResourceRequirementsMet(MainManager.Inventory.filtrationICost) && MainManager.Player.filtrationI == false)
        {
            filtrationIButton.interactable = true;
        }
        else
        {
            filtrationIButton.interactable = false;
        }

        // filtrationII button logic
        if (ResourceRequirementsMet(MainManager.Inventory.filtrationIICost) && MainManager.Player.filtrationII == false && MainManager.Player.filtrationI == true)
        {
            filtrationIIButton.interactable = true;
        }
        else
        {
            filtrationIIButton.interactable = false;
        }

        // miningI button logic
        if (ResourceRequirementsMet(MainManager.Inventory.miningICost) && MainManager.Player.collectionI == false)
        {
            miningIButton.interactable = true;
        }
        else
        {
            miningIButton.interactable = false;
        }

        // miningII button logic
        if (ResourceRequirementsMet(MainManager.Inventory.miningIICost) && MainManager.Player.collectionII == false && MainManager.Player.collectionI == true)
        {
            miningIIButton.interactable = true;
        }
        else
        {
            miningIIButton.interactable = false;
        }

        // navigationI button logic
        if (ResourceRequirementsMet(MainManager.Inventory.navigationICost) && MainManager.Player.navigationI == false)
        {
            navigationIButton.interactable = true;
        }
        else
        {
            navigationIButton.interactable = false;
        }

        // navigationII button logic
        if (ResourceRequirementsMet(MainManager.Inventory.navigationIICost) && MainManager.Player.navigationII == false && MainManager.Player.navigationI == true)
        {
            navigationIIButton.interactable = true;
        }
        else
        {
            navigationIIButton.interactable = false;
        }

        // shield button logic
        if (ResourceRequirementsMet(MainManager.Inventory.shieldCost) && MainManager.Player.shield.activeInHierarchy == false)
        {
            shieldButton.interactable = true;
        }
        else
        {
            shieldButton.interactable = false;
        }

        // shield research button logic
        if (ResourceRequirementsMet(MainManager.Inventory.shieldResearchCost) && MainManager.Player.shieldResearched == false && MainManager.Player.researchInProgress == false)
        {
            shieldResearchButton.interactable = true;
        }
        else
        {
            shieldResearchButton.interactable = false;
        }

        // torpedoesEMP button logic
        if (ResourceRequirementsMet(MainManager.Inventory.torpedoesEMPCost) && MainManager.Player.torpedoesEMP == false && MainManager.Player.researchInProgress == false)
        {
            torpedoesEMPButton.interactable = true;
        }
        else
        {
            torpedoesEMPButton.interactable = false;
        }

        // torpedoesHE button logic
        if (ResourceRequirementsMet(MainManager.Inventory.torpedoesHECost) && MainManager.Player.torpedoesHE == false && MainManager.Player.researchInProgress == false)
        {
            torpedoesHEButton.interactable = true;
        }
        else
        {
            torpedoesHEButton.interactable = false;
        }

        // torpedoesSeeking button logic
        if (ResourceRequirementsMet(MainManager.Inventory.torpedoesSeekingCost) && MainManager.Player.torpedoesSeeking == false && MainManager.Player.researchInProgress == false)
        {
            torpedoesSeekingButton.interactable = true;
        }
        else
        {
            torpedoesSeekingButton.interactable = false;
        }

        // turret 0 Button Logic
        if (ResourceRequirementsMet(MainManager.Inventory.turretCost) && MainManager.Player.turrets[0].activeInHierarchy == false)
        {
            turret0Button.interactable = true;
        }
        else
        {
            turret1Button.interactable = false;
        }

        // turret 1 Button Logic
        if (ResourceRequirementsMet(MainManager.Inventory.turretCost) && MainManager.Player.turrets[1].activeInHierarchy == false)
        {
            turret1Button.interactable = true;
        }
        else
        {
            turret1Button.interactable = false;
        }

        // turret 2 Button Logic
        if (ResourceRequirementsMet(MainManager.Inventory.turretCost) && MainManager.Player.turrets[2].activeInHierarchy == false)
        {
            turret2Button.interactable = true;
        }
        else
        {
            turret2Button.interactable = false;
        }

        // turret 3 Button Logic
        if (ResourceRequirementsMet(MainManager.Inventory.turretCost) && MainManager.Player.turrets[3].activeInHierarchy == false)
        {
            turret3Button.interactable = true;
        }
        else
        {
            turret3Button.interactable = false;
        }

        // turret 4 Button Logic
        if (ResourceRequirementsMet(MainManager.Inventory.turretCost) && MainManager.Player.turrets[4].activeInHierarchy == false)
        {
            turret4Button.interactable = true;
        }
        else
        {
            turret4Button.interactable = false;
        }
    }


    // Turret Controls Logic
    public void MouseOverPierceTorpedo(int turretNumber) // Display the pierceTorpedoMouseOver sprite when the pierce torpedo button is moused over
    {
        if (MainManager.Player.turrets[turretNumber].activeInHierarchy)
        {
            torpedoesPiercingToggle[turretNumber].sprite = pierceTorpedoMouseover;
        }
    }

    public void MouseOverHETorpedo(int turretNumber) // Display the hETorpedoMouseOver sprite when the pierce torpedo button is moused over
    {
        if (MainManager.Player.turrets[turretNumber].activeInHierarchy)
        {
            torpedoesHEToggle[turretNumber].sprite = hETorpedoMouseover;
        }
    }

    public void MouseOverEMPTorpedo(int turretNumber) // Display the eMPTorpedoMouseOver sprite when the pierce torpedo button is moused over
    {
        if (MainManager.Player.turrets[turretNumber].activeInHierarchy)
        {
            torpedoesEMPToggle[turretNumber].sprite = eMPTorpedoMouseover;
        }
    }

    public void MouseOverSeekingTorpedo(int turretNumber) // Display the seekingTorpedoMouseOver sprite when the pierce torpedo button is moused over
    {
        if (MainManager.Player.turrets[turretNumber].activeInHierarchy)
        {
            torpedoesSeekingIndicator[turretNumber].sprite = seekingTorpedoMouseover;
        }
    }

    public void MouseExitTorpedoToggles() // revert torpedo control buttons to default sprite when mouse exits the button
    {
        UpdateTurretButtons();
    }

    public void SwitchTurretToPierceTorpedoes(int turretNumber) // if firtype and turret unlocked/built, switch the turret to the firetype
    {
        if (MainManager.Player.turrets[turretNumber].activeInHierarchy == true)
        {
            MainManager.Player.ChangeTurretProjectileToPiercing(turretNumber);
            UpdateTurretButtons();
        }
        else
        {
            // Debug.Log("Can't switch to this torpedo type because you haven't constructed this turret");
        }
    }

    public void SwitchTurretToHETorpedoes(int turretNumber) // if firtype and turret unlocked/built, switch the turret to the firetype
    {
        if (MainManager.Player.turrets[turretNumber].activeInHierarchy == true && MainManager.Player.torpedoesHE == true) 
        {
            MainManager.Player.ChangeTurretProjectileToHE(turretNumber);
            UpdateTurretButtons();
        }
        else
        {
            // Debug.Log("Can't switch to this torpedo type because you haven't constructed this turret and/or you haven't researched this torepdo type");
        }
    }

    public void SwitchTurretToEMPTorpedoes(int turretNumber) // if firtype and turret unlocked/built, switch the turret to the firetype
    {
        if (MainManager.Player.turrets[turretNumber].activeInHierarchy == true && MainManager.Player.torpedoesEMP == true)
        {
            MainManager.Player.ChangeTurretProjectileToEMP(turretNumber);
            UpdateTurretButtons();
        }
        else
        {
            // Debug.Log("Can't switch to this torpedo type because you haven't constructed this turret and/or you haven't researched this torepdo type");
        }
    }

    public void UpdateTurretButtons() // Update the fire selection buttons based on recently completed research/upgrades and UI interactions
    {
        for (int i = 0; i < MainManager.Player.turrets.Length; i++)
        {
            if (MainManager.Player.turrets[i].activeInHierarchy == true)
            {
                // determine selected firemode of each turret; make sprite active
                if (MainManager.Player.turrets[i].GetComponent<PlayerTurret>().GetProjectileType() == 0) // if turret i set to pierce
                {
                    torpedoesPiercingToggle[i].sprite = pierceTorpedoSelected;
                    if (MainManager.Player.torpedoesHE) // make HE Torpedoe toggle interactable or not depending on research status
                    {
                        torpedoesHEToggle[i].sprite = hETorpedoInteractable;
                    }
                    else
                    {
                        torpedoesHEToggle[i].sprite = hETorpedoNonInteractable;
                    }
                    if (MainManager.Player.torpedoesEMP)
                    {
                        torpedoesEMPToggle[i].sprite = eMPTorpedoInteractable;
                    }
                    else
                    {
                        torpedoesEMPToggle[i].sprite = eMPTorpedoNonInteractable;
                    }
                }
                else if (MainManager.Player.turrets[i].GetComponent<PlayerTurret>().GetProjectileType() == 1) // if turret i set to HE
                {
                    torpedoesPiercingToggle[i].sprite = pierceTorpedoInteractable;
                    torpedoesHEToggle[i].sprite = hETorpedoSelected;
                    if (MainManager.Player.torpedoesEMP)
                    {
                        torpedoesEMPToggle[i].sprite = eMPTorpedoInteractable;
                    }
                    else
                    {
                        torpedoesEMPToggle[i].sprite = eMPTorpedoNonInteractable;
                    }
                }
                else if (MainManager.Player.turrets[i].GetComponent<PlayerTurret>().GetProjectileType() == 2) // if turret i set to EMP
                {
                    torpedoesPiercingToggle[i].sprite = pierceTorpedoInteractable;
                    if (MainManager.Player.torpedoesHE) // make HE Torpedoe toggle interactable or not depending on research status
                    {
                        torpedoesHEToggle[i].sprite = hETorpedoInteractable;
                    }
                    else
                    {
                        torpedoesHEToggle[i].sprite = hETorpedoNonInteractable;
                    }
                    torpedoesEMPToggle[i].sprite = eMPTorpedoSelected;
                }

                // if seeking unlocked && if turret set to seek, torpedoesSeekToggle[i] = torpedoesSeekActive; else torpedoesSeekToggle[i] = torpedoesSeekInactive; else torpedoesSeekToggle disabled
                if (MainManager.Player.torpedoesSeeking && MainManager.Player.turrets[i].GetComponent<PlayerTurret>().seeking)
                {
                    torpedoesSeekingIndicator[i].sprite = seekingTorpedoSelected;
                }
                else if (MainManager.Player.torpedoesSeeking && !MainManager.Player.turrets[i].GetComponent<PlayerTurret>().seeking)
                {
                    torpedoesSeekingIndicator[i].sprite = seekingTorpedoInteractable;
                }
                else
                {
                    torpedoesSeekingIndicator[i].sprite = seekingTorpedoNonInteractable;
                }
            }
            else // Turret i is not active in the hierarchy and thus all related buttons should not have the interactable apperance
            {
                torpedoesPiercingToggle[i].sprite = pierceTorpedoNonInteractable;
                torpedoesHEToggle[i].sprite = hETorpedoNonInteractable;
                torpedoesEMPToggle[i].sprite = eMPTorpedoNonInteractable;
                torpedoesSeekingIndicator[i].sprite = seekingTorpedoNonInteractable;
            }
        }
    }

    public void EnableBuildButton() // Make build button clickable; display build cost?
    {
        buildButton.interactable = true;
    }

    public void DisableBuildButton() // Disable build button; reset build cost indicator?
    {
        buildButton.interactable = false;
    }

    public void EnableBuildPanel() // Displays the build panel in the HUD when the player enters the appropriate trigger volume
    {
        buildPanel.SetActive(true);
        structureLabel.text = buildingReference.structureName + " Structure";

        // set the supply cost labels
        for (int i = 0; i < buildSupplyCosts.Length; i++)
        {
            buildSupplyCosts[i].text = buildingReference.suppliesCost[i].ToString();
            if (buildingReference.suppliesCost[i] > 0)
            {
                MainManager.UI.buildSuppplyLabels[i].SetActive(true);
            }
            else
            {
                MainManager.UI.buildSuppplyLabels[i].SetActive(false);
            }
        }
    }

    public void DisableBuildPanel() // Disables the build panel when the player exits the build trigger volume
    {
        buildPanel.SetActive(false);
    }

    public void EnableGatheringPanel() // Enables the gathering panel in the HUD when the player is in a gathering trigger
    {
        gatheringPanel.SetActive(true);
    }

    public void DisableGatheringPanel() // Disables the gathering panel in the HUD when the player exits a gathering trigger
    {
        gatheringPanel.SetActive(false);
    }

    public void BuildStructure() // Build the structure that the player is near and remove cost
    {
        buildingReference.Build();
        MainManager.Inventory.RemoveSupplies(buildingReference.suppliesCost);
        UpdateSupplyLabels();
        buildingReference = null;
        DisableBuildButton();
    }
    
    public void ToggleGathering() // Toggles mining state depending on it's current state
    {
        if (MainManager.Player.GetMining())
        {
            MainManager.Player.toggleMiningOff();
            MainManager.Player.toggleMovementOn();
        }
        else
        {
            MainManager.Player.toggleMovementOff();
            MainManager.Player.toggleMiningOn();
        }
    }

    public void UpdateSupplyLabels() // Update the supply lables on the UI
    {
        int[] suppliesInInventory = MainManager.Inventory.GetSupplies();

        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            supplyLabels[i].text = suppliesInInventory[i].ToString();
        }
    }

    public void UpdateHull() // Update the status of the Hull on the UI
    {
        hullText.text = Math.Truncate(((float)(MainManager.Player.hull / MainManager.Player.hullMax)) * 100).ToString() + "%";
        hullBar.fillAmount = (float)MainManager.Player.hull / (float)MainManager.Player.hullMax;
    }

    public void UpdateShieldBar() // funciton will be used to display the number of hits the shield has left before it breaks and must be rebuilt
    {

    }

    public void UpdateEnergy() // Update the energy percentage located on the Resources UI bar
    {
        energyText.text = Math.Round(((MainManager.Player.energy / MainManager.Player.maxEnergy) * 100)).ToString() + "%";
    }


    // Encyclopedia related entries
    public void CloseEncyclopedia() // Closes the encyclopedia panel
    {
        encyclopediaPanel.gameObject.SetActive(false);
        MainManager.Scene.UnPauseGame();
    }

    public void ResetEncyclopediaEntries() // Sets entries in the display group on the left of the encyclopedia panel to inactive when changing tabs to display other entry buttons
    {
        for (int i = 0; i < geologyEntryButtons.Length; i++)
        {
            geologyEntryButtons[i].SetActive(false);
        }

        for (int i = 0; i < crewEntryButtons.Length; i++)
        {
            crewEntryButtons[i].SetActive(false);
        }

        for (int i = 0; i < techEntryButtons.Length; i++)
        {
            techEntryButtons[i].SetActive(false);
        }

        for (int i = 0; i < storyEntryButtons.Length; i++)
        {
            storyEntryButtons[i].SetActive(false);
        }

        for (int i = 0; i < enemiesEntryButtons.Length; i++)
        {
            enemiesEntryButtons[i].SetActive(false);
        }

        for (int i = 0; i < otherEntryButtons.Length; i++)
        {
            otherEntryButtons[i].SetActive(false);
        }
    }

    public void ShowGeologyEncylcopediaEntries() // Display entries in the encyclopedia's left group related to the Geology tab
    {
        ResetEncyclopediaEntries();
        for (int i = 0; i < geologyEntryButtons.Length; i++)
        {
            geologyEntryButtons[i].SetActive(true);
        }
    }

    public void ShowCrewEncycolopediaEntries() // Display entries in the encyclopedia's left group related to the Crew tab
    {
        ResetEncyclopediaEntries();
        for (int i = 0; i < crewEntryButtons.Length; i++)
        {
            crewEntryButtons[i].SetActive(true);
        }
    }

    public void ShowTechEncyclopediaEntries() // Display entries in the encyclopedia's left group related to the Tech tab
    {
        ResetEncyclopediaEntries();
        for (int i = 0; i < techEntryButtons.Length; i++)
        {
            techEntryButtons[i].SetActive(true);
        }
    }

    public void ShowStoryEncyclopediaEntries() // Display entries in the encyclopedia's left group related to the Story tab
    {
        ResetEncyclopediaEntries();
        for (int i = 0; i < storyEntryButtons.Length; i++)
        {
            storyEntryButtons[i].SetActive(true);
        }
    }

    public void ShowEnemyEncyclopediaEntries() // Display entries in the encyclopedia's left group related to the Enemy tab
    {
        ResetEncyclopediaEntries();
        for (int i = 0; i < enemiesEntryButtons.Length; i++)
        {
            enemiesEntryButtons[i].SetActive(true);
        }
    }

    public void ShowOtherEncyclopediaEntries() // Display entries in the encyclopedia's left group related to the Other tab
    {
        ResetEncyclopediaEntries();
        for (int i = 0; i < otherEntryButtons.Length; i++)
        {
            otherEntryButtons[i].SetActive(true);
        }
    }

    public void SetEncyclopediaEntry(GameObject entryObject) // When a button on the left portion of the Encyclopedia panel is clicked, display it's corresponding entry contents on the right side of the panel
    {
        if (activeObject != null)
        {
            activeObject.SetActive(false); // disable previous entry
        }
        entryObject.SetActive(enabled); // enable new entry
        activeObject = entryObject; // set current entry as active
    }

    public void DisplayAreaName(string _areaName) // Method executed when entering an area trigger in game; displays the name of the area being entered on the HUD for a few seconds
    {
        areaName.text = _areaName;
        areaName_fadeIn_fadeOut.Play();
    }
}