/* Written by Cameron Williams
 * 
 * This script manages all of the variables related to the chracters current
 * status. This includes the player's hull integrity and the constructed/active
 * technologies. This used to include the crew allocation variables as well before
 * that system was cut.
 * 
 * Last updated 8/21/2019
 */

using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; } // As inhereited by IGameManager

    // Ship Status Variables
    private bool movement; // keeps track of whether the player is in movement mode
    private bool mining; // keeps track of whether the player is in mining mode
    public float hull; // keeps track of the amount of health the player has
    public float hullMax = 500;
    [Header("Player Start Settings")]
    [SerializeField] [Tooltip("Amount of health the player starts with")] public int initialHull = 500;
    [SerializeField] [Tooltip("Amount of energy the player starts with")] public int initialEnergy = 100;

    [Header("Tech Variables and Objects")]
    // These are mostly boolean variables representing whether the corresponding tech has been acquired
    [SerializeField] public bool armorI;
    [SerializeField] [Range(1, 100)] public float armorIPercentDamageReduction = 10; // defines the effectiveness of armorI upgrade
    [SerializeField] public bool armorII;
    [SerializeField] [Range(1, 100)] public float armorIIPercentDamageReduction = 20; // defines the effectiveness of armorII upgrade
    [SerializeField] [Tooltip("The boost tanks on the ship; also functions as a bool by checking if the object is active or not")] public GameObject boost;
    [SerializeField] public bool empProtection;
    [SerializeField] public bool energyStorage;
    [SerializeField] public bool engineI;
    [SerializeField] public bool engineII;
    [SerializeField] public bool filtrationI;
    [SerializeField] public bool filtrationII;
    [SerializeField] public bool collectionI;
    [SerializeField] public bool collectionII;
    [SerializeField] public bool navigationI;
    [SerializeField] public bool navigationII;
    [SerializeField] [Tooltip("The shiled on the ship model; also fucnions as a bool by checking if the object is active or not")] public GameObject shield;
    [SerializeField] public bool shieldResearched;
    [SerializeField] public bool torpedoesEMP;
    [SerializeField] public bool torpedoesHE;
    [SerializeField] public bool torpedoesSeeking;
    [SerializeField] [Tooltip("GameObjects attached to the player ship, in order front, front left, front right, back left, back right")]public GameObject[] turrets;
    [SerializeField] [Tooltip("Determines which turrets are being powered")] public bool[] turretsEnable = { true, false, false, false, false };
    [NonSerialized] public bool researchInProgress = false;
    
    [Header("Energy Costs/Variables")]
    public float energy = 100;
    public float maxEnergy = 100;
    public float passiveEnergyGainPerSecond = 0;
    public float lightsEnergyCostPerSecond = 1;
    public float turretsEnergyCostPerSecond = 1;
    public float boostEnergyCost = 10;

    [Header("Player Damage Materials")]
    [SerializeField] public Material player_sub_back;
    [SerializeField] public Material player_sub_engines;
    [SerializeField] public Material player_sub_main;
    [SerializeField] public Material player_sub_shield;
    [SerializeField] public Material player_sub_turret;

    [Header("Player Upgrade Textures")]
    [SerializeField] public Material submarine_Material;
    [SerializeField] public Texture submarine_noUpgrade;
    [SerializeField] public Texture submarine_armorIUpgrade;
    [SerializeField] public Texture submarine_armorIIUpgrade;
    [SerializeField] public Material engine_Material;
    [SerializeField] public Texture engines_noUpgrade;
    [SerializeField] public Texture engines_engineIIUpgrade;

    [Header("Misc")]
    [SerializeField] [Tooltip("Reference to player SpotLight for changing light levels via the slider")] public Light playerSpotLight;
    [SerializeField] [Tooltip("Reference to the player PointLight for chaning light levels via the slider")] public Light playerPointLight;
    [SerializeField] public Light playerLight = null;
    [SerializeField] [Tooltip("Player detection collider")] public SphereCollider playerDetectionZone;
    [SerializeField] [Tooltip("")] public UnityEngine.UI.Slider lightSlider;
    private float empProtectionDisableDurationReduction = 4.0f;
    private int shieldIntegrity = 5;
    private int shieldIntegrityMax = 5;

    public void Startup() // Inherited by IGameManager; set player status variables to inital values
    {
        // Debug.Log("Player manager starting...");
        // Initialize all starting variables
        hull = initialHull;
        energy = initialEnergy;
        UpdatePlayerDamageMaterials(0.0f);
        ResetUpradeTextures();
        movement = true;
        mining = false;

        StartCoroutine(CalculateEnergyPerSecond());
        MainManager.UI.UpdateTurretButtons();

        status = ManagerStatus.Started;
    }

    public void ChangeLight() // Take input from the Light Slider (0 to 1) and adjust spotlight range and intensity and point light intensity
    {
        float input = lightSlider.value;
        playerSpotLight.range = 6 + (9*input); // We found a spotlight range from 6 to 15 reasonable, so that's where the numbers come from
        playerSpotLight.intensity = 4 - (2*input); // We found a spotlight intensity from 4 to 2 resonable
        playerPointLight.intensity = 2 + (2*input); // We found a pointlight intensity from 2 to 4 reasonable
        playerDetectionZone.radius = 5 + (5*input); // Increase player detection area with light level; probably need a seperate function in the future to compound light level with time spent mining
        lightsEnergyCostPerSecond = 0 + (2 * input);
        for (int i = 0; i < turrets.Length; i++)
        {
            PlayerTurret turretScript = turrets[i].GetComponent<PlayerTurret>();
            turretScript.fireBubbleDistance = 2.5f + (2 * input);
            turretScript.fireBubbleRadius = 3 + (2 * input);
        }
    }

    public void ChangeTurretEnable(int turretNumber) // toggle turrets on/off depending on UI button state
    {
        if (turrets[turretNumber].activeInHierarchy == true && turretsEnable[turretNumber] == true || energy == 0) // turret has been built and is enabled or out of energy
        {
            turretsEnable[turretNumber] = false;
            MainManager.UI.turretActiveIndicator[turretNumber].SetActive(false);
        }
        else if (turrets[turretNumber].activeInHierarchy == true && turretsEnable[turretNumber] == false) // turret has been built and is disabled
        {
            turretsEnable[turretNumber] = true;
            MainManager.UI.turretActiveIndicator[turretNumber].SetActive(true);
        }
        else // turret has not been constructed yet
        {
            // Debug.Log("You may not enable this turret because you haven't built it yet or you don't have enough available crew.");
        }
    }

    public void ChangeTurretSeeking(int turretNumber) // if firtype and turret unlocked/built, switch the turret to the firetype
    {
        if (turrets[turretNumber].activeInHierarchy == true && MainManager.Player.torpedoesSeeking == true && turrets[turretNumber].GetComponent<PlayerTurret>().seeking)
        {
            turrets[turretNumber].GetComponent<PlayerTurret>().seeking = false;
            MainManager.UI.UpdateTurretButtons();
        }
        else if (turrets[turretNumber].activeInHierarchy == true && MainManager.Player.torpedoesSeeking == true && !turrets[turretNumber].GetComponent<PlayerTurret>().seeking)
        {
            turrets[turretNumber].GetComponent<PlayerTurret>().seeking = true;
            MainManager.UI.UpdateTurretButtons();
        }
    }

    public void ChangeEnergy(float value) // change amount of energy by the value input
    {
        energy = energy + value;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        if (energy < maxEnergy)
        {
            MainManager.Inventory.DecrementMagnesium();
        }
        if (energy <= 0) // shut down turrets and turn lights to minimum if out of energy
        {
            energy = 0;
            for (int i = 0; i < turrets.Length; i++)
            {
                ChangeTurretEnable(i);
            }
            MainManager.Player.lightSlider.value = 0;
        }
        MainManager.UI.UpdateEnergy();
    }

    private IEnumerator CalculateEnergyPerSecond() // Determines energy cost per second depending on the light slider and the number of active turrets; boost uses energy instantly and thus isn't in this calculation
    {
        int turretsEnergyCostPerSecond = 0;
        yield return new WaitForSeconds(1);
        for (int i = 0; i < turretsEnable.Length; i++)
        {
            if (turretsEnable[i] == true)
            {
                turretsEnergyCostPerSecond++;
            }
        }
        float energyCost = passiveEnergyGainPerSecond - turretsEnergyCostPerSecond - lightsEnergyCostPerSecond;
        // Debug.Log("Energy gen/cost is " + energyCost.ToString());
        ChangeEnergy(energyCost);
        StartCoroutine(CalculateEnergyPerSecond());
    }


    // Upgrades
    public void BuildTurret(int turretNumber) // 0-4 represent turrets in the following order: forward, front left, front right, back left, back right
    {
        // Debug.Log("Starting to build tech...");
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.turretCost.cost);
        turrets[turretNumber].SetActive(true);
        MainManager.UI.UpdateTechButtons();
        ChangeTurretEnable(turretNumber);
        MainManager.UI.UpdateTurretButtons();
    }

    public void BuildEngineI() // unlock the tier 1 engine upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.engineICost.cost);
        engineI = true;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildEngineII() // unlock the tier 2 engine upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.engineIICost.cost);
        engineII = true;
        engine_Material.mainTexture = engines_engineIIUpgrade;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildMiningI() // unlock the tier 1 mining upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.miningICost.cost);
        collectionI = true;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildMiningII() // unlock the tier 2 mining upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.miningIICost.cost);
        collectionII = true;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildFiltrationI() // unlock the tier 1 filtration upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.filtrationICost.cost);
        filtrationI = true;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildFiltrationII() // unlock the tier 2 filtration upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.filtrationIICost.cost);
        filtrationII = true;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildShield() // Unlock the shield upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.shieldCost.cost);
        shield.SetActive(true);
        MainManager.UI.shieldBar.gameObject.SetActive(true);
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildArmorI() // unlock the tier 1 armor upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.armorICost.cost);
        armorI = true;
        submarine_Material.mainTexture = submarine_armorIUpgrade;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildArmorII() // unlock the tier 2 armor upgared
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.armorIICost.cost);
        armorII = true;
        submarine_Material.mainTexture = submarine_armorIIUpgrade;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildEnergyStorage() // unlock the energy storage upgarde
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.energyStorageCost.cost);
        energyStorage = true;
        maxEnergy = 200; // increase maximum energy to upgraded limit
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildNavigationI() // unlock the tier 1 navigtation upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.navigationICost.cost);
        navigationI = true;
        MainManager.UI.UpdateTechButtons();
    }

    public void BuildNavigationII() // unlock the tier 2 navigtation upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.navigationIICost.cost);
        navigationII = true;
        MainManager.UI.UpdateTechButtons();
    }

    private void ResetUpradeTextures()
    {
        submarine_Material.mainTexture = submarine_noUpgrade;
        engine_Material.mainTexture = engines_noUpgrade;
    }

    // Research
    public void ResearchHETorpedoes() // Unlock the HE Torpedo tech
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.torpedoesHECost.cost);
        MainManager.UI.currentResearch.text = "Explosive Torpedo";
        MainManager.UI.techImage.sprite = MainManager.UI.hETorpedoSprite;
        StartCoroutine(ResearchTech(MainManager.Inventory.torpedoesHECost.researchRequired, (result) => { torpedoesHE = result; }));
        MainManager.UI.UpdateTechButtons();
        MainManager.UI.UpdateTurretButtons();
    }

    public void ResearchBoost() // unlock the boost upgrade/ability
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.boostCost.cost);
        MainManager.UI.currentResearch.text = "Boost";
        MainManager.UI.techImage.sprite = MainManager.UI.boostSprite;
        StartCoroutine(ResearchTech(boost, MainManager.Inventory.boostCost.researchRequired));
        MainManager.UI.UpdateTechButtons();
    }

    public void ResearchSeekingTorpedoes() // Unlock the Seeking Torpedo tech
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.torpedoesSeekingCost.cost);
        MainManager.UI.currentResearch.text = "Seeking Mechanic";
        MainManager.UI.techImage.sprite = MainManager.UI.seekingSprite;
        StartCoroutine(ResearchTech(MainManager.Inventory.torpedoesSeekingCost.researchRequired, (result) => { torpedoesSeeking = result; }));
        MainManager.UI.UpdateTechButtons();
        MainManager.UI.UpdateTurretButtons();
    }

    public void ResearchShield() // Start research on the shield upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.shieldResearchCost.cost);
        MainManager.UI.currentResearch.text = "Forward Shield";
        MainManager.UI.techImage.sprite = MainManager.UI.forwardShieldSprite;
        StartCoroutine(ResearchTech(MainManager.Inventory.shieldResearchCost.researchRequired, (result) => { shieldResearched = result; }));
        MainManager.UI.UpdateTechButtons();
    }

    public void ResearchEMPProtection() // unlock the emp protection upgrade
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.empProtectionCost.cost);
        MainManager.UI.currentResearch.text = "Counter EMP";
        MainManager.UI.techImage.sprite = MainManager.UI.counterEMPSprite;
        StartCoroutine(ResearchTech(MainManager.Inventory.empProtectionCost.researchRequired, (result) => { empProtection = result; }));
        MainManager.UI.UpdateTechButtons();
    }

    public void ResearchEMPTorpedoes() // Unlock the EMP Torpedo tech
    {
        MainManager.Inventory.RemoveSupplies(MainManager.Inventory.torpedoesEMPCost.cost);
        MainManager.UI.currentResearch.text = "EMP Torpedo";
        MainManager.UI.techImage.sprite = MainManager.UI.eMPSprite;
        StartCoroutine(ResearchTech(MainManager.Inventory.torpedoesEMPCost.researchRequired, (result) => { torpedoesEMP = result; }));
        MainManager.UI.UpdateTechButtons();
        MainManager.UI.UpdateTurretButtons();
    }

    public void ChangeTurretProjectileToEMP(int turretNumber) // Set respective turret to fire EMP Torpedoes
    {
        turrets[turretNumber].GetComponent<PlayerTurret>().SetProjectile(2);
        // Debug.Log("Turret set to use EMP Torpedoes");
    }

    public void ChangeTurretProjectileToHE(int turretNumber) // Set respective turret to fire HE Torpedoes
    {
        turrets[turretNumber].GetComponent<PlayerTurret>().SetProjectile(1);
        // Debug.Log("Turret set to use HE Torpedoes");
    }

    public void ChangeTurretProjectileToPiercing(int turretNumber) // Set respective turret to fire Piercing Torpedoes
    {
        turrets[turretNumber].GetComponent<PlayerTurret>().SetProjectile(0);
        // Debug.Log("Turret set to use piercing torpedoes");
    }

    private int researchComplete;
    public IEnumerator ResearchTech(GameObject techToEnable, int researchRequired) // Build tech that requires enabling a GameObject; takes amount of time specified by researchRequired
    {
        SetResearch();
        while (researchComplete < researchRequired)
        {
            researchComplete++;
            // MainManager.UI.researchBar.value = (float)researchComplete / (float)researchRequired;
            MainManager.UI.techResearchTime.value = (float)researchComplete / (float)researchRequired;
            //MainManager.UI.researchPercentage.text = (Math.Round((float)researchComplete / (float)researchRequired * 100)).ToString() + "%";
            // Debug.Log("Research completed is " + researchComplete + " out of " + researchRequired);
            yield return new WaitForSeconds(1.0f);
        }
        techToEnable.SetActive(true);
        // Debug.Log("Tech completed!");
        ResetResearch();
    }

    public IEnumerator ResearchTech(int researchRequired, System.Action<bool> callback) // for techs not related to a specific game object, but a boolean variable
    {
        // Debug.Log("Tech enable coroutine started. Research required is " + researchRequired);
        SetResearch();
        while (researchComplete < researchRequired)
        {
            researchComplete++;
            //MainManager.UI.researchBar.value = (float)researchComplete / (float)researchRequired;
            MainManager.UI.techResearchTime.value = (float)researchComplete / (float)researchRequired;
            //MainManager.UI.researchPercentage.text = (Math.Round((float)researchComplete / (float)researchRequired * 100)).ToString() + "%";
            // Debug.Log("Research completed is " + researchComplete + " out of " + researchRequired);
            yield return new WaitForSeconds(1.0f);

        }
        // Debug.Log("Tech completed!");
        callback(true); // Coroutines can't assign the value of reference variables, so I had to use a callback
        ResetResearch();
    }

    private void SetResearch() // Sets research in progress boolean and displays the image of the tech being researched
    {
        researchInProgress = true;
        MainManager.UI.techImage.enabled = true; // image sprite set in individual button functions
        researchComplete = 0;
    }

    private void ResetResearch() // Executed when research is completed; resets the researching tech image and research status
    {
        researchInProgress = false;
        MainManager.UI.techImage.enabled = false;
        MainManager.UI.currentResearch.text = "Nothing"; // current research set originally in individual button functions
    }

    public void DamageHull(int damage) // damage the player an amount that depends on armor upgrades
    {
        // Debug.Log("Initial damage to player is " + damage);

        if (armorII == true)
        {
            damage = damage - (int)Math.Ceiling(armorIIPercentDamageReduction * 0.01f * damage);
            // Debug.Log("Player has Armor 2. Damage after armor is " + damage);
        }
        else if (armorII == false && armorI == true)
        {
            damage = damage - (int)Math.Ceiling(armorIPercentDamageReduction * 0.01f * damage);
            // Debug.Log("Player has Armor 1. Damage after armor is " + damage);
        }
        else
        {
            // Debug.Log("Player has no armor upgrades, taking full damage");
        }
        hull = hull - damage;
        UpdatePlayerDamageMaterials(1.0f - hull / hullMax);

        if (hull < 0)
        {
            hull = 0;
        }
        MainManager.UI.UpdateHull();
        if (hull == 0)
        {
            MainManager.Scene.PauseGame();
            MainManager.UI.GameOver.SetActive(true);
        }
    }

    public void DamageShield() // decrement number of hits the shield can take
    {
        shieldIntegrity--;
        MainManager.UI.shieldBar.fillAmount = (float)shieldIntegrity / (float)shieldIntegrityMax;
        if (shieldIntegrity == 0)
        {
            MainManager.UI.shieldBar.gameObject.SetActive(false);
        }
        // TODO: Make shield purchasable again if destroyed completely
    }

    private void UpdatePlayerDamageMaterials(float percentDamaged) // Set the _DamStrength float in Landon's custom shaders on all the parts of the ship
    {
        player_sub_back.SetFloat("_DamStrength", percentDamaged);
        player_sub_engines.SetFloat("_DamStrength", percentDamaged);
        player_sub_main.SetFloat("_DamStrength", percentDamaged);
        player_sub_shield.SetFloat("_DamStrength", percentDamaged);
        player_sub_turret.SetFloat("_DamStrength", percentDamaged);
    }

    public void HitByEMP(float disableDuration) // Determines how long the player should be effected by an EMP and sends the calcuated duration to the DisableElectronics Coroutine
    {
        if (MainManager.Player.empProtection)
        {
            disableDuration = disableDuration - empProtectionDisableDurationReduction;
        }
        // Debug.Log("Running DisableElectronics Coroutine");
        StartCoroutine(DisableElectronics(disableDuration));
    }

    private IEnumerator DisableElectronics(float duration) // shuts down the ships systems for the duration of the EMP effect
    {
        // Debug.Log("Disable Electronics Coroutine Running");
        movement = false;
        playerSpotLight.enabled = false;
        playerLight.enabled = false;
        Rigidbody rbody = MainManager.Scene.GetPlayer().GetComponent<Rigidbody>();
        rbody.useGravity = true;
        bool[] turretsEnableBackup = turretsEnable; // back up turret state before disabling them
        for (int i = 0; i < turretsEnable.Length; i++)
        {
            turretsEnable[i] = false;
        }
        yield return new WaitForSeconds(duration);
        movement = true;
        playerSpotLight.enabled = true;
        playerLight.enabled = true;
        rbody.useGravity = false;
        turretsEnable = turretsEnableBackup; // back up turret state before disabling them
        // Debug.Log("DisableElectronics Coroutine completed");
    }

    public bool GetMovement() // returns movement status
    {
        return movement;
    }

    public void toggleMovementOff() // Turn off movement
    {
        movement = false;
    }

    public void toggleMovementOn() // Turn on movement
    {
        movement = true;
    }

    public bool GetMining() // returns mining status
    {
        return mining;
    }

    public void toggleMiningOn() // Turn on mining
    {
        mining = true;
        StartCoroutine(MainManager.Inventory.Gather());
    }

    public void toggleMiningOff() // Turn off mining
    {
        mining = false;
    }
}