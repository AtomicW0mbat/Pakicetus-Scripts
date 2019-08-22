/* Written by Cameron Williams
 * 
 * This script is meant to be attached to a turret GameObject.
 * When an enemy is detected in the OverlapSphere, the turret rotates the head
 * and barrel to align with the targets location then instantiates a
 * torpedo of a set type that goes straight out towards the detected target.
 * 
 * This script could maybe use some additional target selection logic.
 * 
 * Last updated 8/22/2019
 */

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerTurret : Turret
{
    private float fireTime; // fire time gets calculated in Update. Should probably move this call from this classes Update() to player manager's change crew on turrets methods
    // The three ammo types are assigned below and may be toggled between via the player's HUD
    [SerializeField] [Tooltip("The normal player torpedo")] public GameObject piercingTorpedoPrefab;
    [SerializeField] [Tooltip("High Explosive player torpedo")] public GameObject heTorpedoPrefab;
    [SerializeField] [Tooltip("EMP torpedo prefab")] public GameObject empTorpedoPrefab;
    int fireType = 0; // keeps track of the torpedo to fire
    [SerializeField] private LayerMask enemyLayer = 1; // Layer in which targets will be searched for; default, must be changed in inspector

    // The following two variables are used for rotating the turret and pivoting the barrel such that torpedoes appear to fire from it
    [SerializeField] private GameObject turretHead = null;
    [SerializeField] private GameObject turretBarrel = null;
    // The following two variables are used to determine the area in which targets will be found and fired upon via the OverlapSphere method
    [SerializeField] public float fireBubbleRadius = 4.5f;
    [SerializeField] public float fireBubbleDistance = 4.5f;
    [SerializeField] private int turretNumber = 0; // turret numbers must be assigned in the inspector, 0-4 depending on the locaiton (See PlayerTurretNumbering.cs)
    [SerializeField] public bool seeking = false; // determines if torpedoes fired from this turret will seek; toggled in HUD on a per-turret basis

    void OnDrawGizmosSelected() // Visualizaiton of turrets target detection/firing area
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.up * fireBubbleDistance, fireBubbleRadius);
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        // counts up to determine when to fire, then selects a target and fires a the selected ammo type at the target
        fireTime = baseFireTime;
        timer += Time.deltaTime;

        //Debug.Log("Value of timer is " + timer);

        if (timer >= fireTime && MainManager.Player.turretsEnable[turretNumber] == true)
        {
            Collider[] targetsInFireArea = Physics.OverlapSphere(transform.position - transform.up * fireBubbleDistance, fireBubbleRadius, enemyLayer, QueryTriggerInteraction.Collide);
            // Debug.Log("Found " + targetsInFireArea.Length + " targets in fire area");
            if (targetsInFireArea.Length > 0)
            {
                int targetNumber = Random.Range(0, targetsInFireArea.Length); // select a random target that is in the area
                GameObject target = targetsInFireArea[targetNumber].gameObject;
                turretHead.transform.LookAt(target.transform);
                turretBarrel.transform.LookAt(hitInfo.transform);
                FireTorpedo(target.transform);

                timer = 0.0f; // reset timer for firing again
            }
        }
        timer = (timer >= fireTime) ? fireTime : timer; // Ternary operator just for fun
    }

    private void FireTorpedo(Transform target) // Instantiates the appropriate torpedo and sends it toward the target
    {
        if (fireType == 2)
        {
            _projectile = Instantiate(empTorpedoPrefab) as GameObject;
        }
        if (fireType == 1)
        {
            _projectile = Instantiate(heTorpedoPrefab) as GameObject;
        }
        if (fireType == 0)
        {
            _projectile = Instantiate(piercingTorpedoPrefab) as GameObject;
        }

        _projectile.transform.position = transform.TransformPoint(0, 0, 0);
        _projectile.transform.LookAt(target);
        if (seeking)
        {
            _projectile.SendMessage("SetToSeeking", target.gameObject);
        }
    }

    public void SetProjectile(int type) // Determines the type of projectile the turret will fire
    {
        if (type == 2)
        {
            fireType = 2;
            // Debug.Log(this.gameObject.name + " set to fire emp torpedoes");
        }
        else if (type == 1)
        {
            fireType = 1;
            // Debug.Log(this.gameObject.name + " set to fire he torpedoes");
        }
        else if (type == 0)
        {
            fireType = 0;
            // Debug.Log(this.gameObject.name + " set to fire pierce torpedoes");
        }
    }

    public int GetProjectileType() // Used to determine the interactability of the ammo selection buttons in the turret section of the HUD
    {
        return fireType;
    }
}
