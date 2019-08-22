/* Written by Cameron Williams
 * 
 * This script defines the stats and behaviour of the most common enemy found
 * in the game. As per our discussion on enemy behaviour, the default passive
 * behaviour scout will patrol the area. When the scout detects the player, it
 * will alert other nearby enemies of the players location. The scouts then
 * approach the player and find a path to attack from via the "Attack Vecotr Ring".
 * 
 * Scouts scan for waypoints by sending a small SphereCast forwards. If this fails,
 * they fall back to a larger OverlapSphere for scanning for waypoints.
 * 
 * Last updated on 8/21/2019.
 */

using UnityEngine;
using System.Collections;

public class Scout : MonoBehaviour, IEnemyAI
{
    // Variables from EnemyAI interface
    public int hitPoints { get; private set; }
    public float moveSpeed { get; private set; }
    [SerializeField]
    public bool pierceVulnerability { get; private set; } = true;
    public bool hEVulnerability { get; private set; } = true;
    public bool eMPVulnerability { get; private set; } = true;
    public bool disabled { get; private set; } = false;
    [Header("EnemeyAI Variables Initialization")]
    [SerializeField] [Tooltip("Amount of HP this enemy prefab will start with")] private int hP = 25;

    // Scout Specific variables
    [Header("Navigation")]
    private Rigidbody rbody;
    private Vector3 destination; // need to change usage in fixed update or make this default to last velocity heading if no other valid destinations are found
    bool playerDetected = false;
    [SerializeField] private float thrust = 5f;
    [SerializeField] private LayerMask waypointsLayer = 1;
    [SerializeField] private float waypointDetectionRadius = 5f;
    [SerializeField] private int forwardCheckRadius = 5;
    [SerializeField] private int forwardCheckDistance = 30;
    
    [Header("Attacking")]
    private GameObject player;
    [SerializeField] private LayerMask attackVectorLayer = 1;
    [SerializeField] private float attackVectorDetectionRadius = 20f;
    [SerializeField] [Tooltip("Amount of damage the enemy prefab deals to the player on contact")] private int collisionDamage = 10;
    [SerializeField] private GameObject explosion = null;
    [SerializeField] private float dashAttackForce = 10f;
    

    [Header("Enemy Communications")]
    [SerializeField] private LayerMask enemyLayer = 1;
    [SerializeField] private float enemyCommsRadius = 75f;

    [Header("Other")]
    [SerializeField] private Light scoutLight = null;

    void OnDrawGizmosSelected() // Visualize obstacle and waypoint detection areas, destination
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, waypointDetectionRadius); // Red sphere indicating waypoint detection area
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, destination); // Blue line indicating the scouts intended destination
    }

    // EnemyAI interface function
    public void TakeDamage(int damage) // Subtract damage from hitPoints; destroy object if hitPoints 0 or below
    {
        hitPoints = hitPoints - damage;
        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void HitByEMP(int disableTime) // Executes the DisabledByEMP Coroutine
    {
        StartCoroutine(DisabledByEMP(disableTime));
    }

    private IEnumerator DisabledByEMP(int disableTime) // Coroutine to be called by HitByEMP; mark enemy as disabled and make it use gravity for the duration to make it slowly drift to the seafloor
    {
        disabled = true;
        scoutLight.enabled = false;
        rbody.useGravity = true; // temporarily enable low gravity to make the scout slowly drift to the sea floor
        rbody.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(disableTime);
        disabled = false;
        scoutLight.enabled = true;
        rbody.useGravity = false;
        FindNewWaypoint();
    }

    // MonoBehaviour functions
    void Start()
    {
        // initialize variables
        disabled = false;
        FindNewWaypoint();
        hitPoints = hP;
        rbody = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.LookRotation(transform.forward);
    }

    void FixedUpdate() // Move towards target destination and turn to face it if necessary
    {
        if (!disabled)
        {
            rbody.AddForce((destination - transform.position).normalized * thrust);
            if (rbody.velocity.normalized != Vector3.zero) // look in direction of movement if velocity not 0
            {
                transform.rotation = Quaternion.LookRotation(rbody.velocity.normalized, Vector3.up);
            }
        }
    }

    public void OnCollisionEnter(Collision other) // Rigidbody collision logic
    {
        if (other.gameObject.tag == "Enemy") // On collision with another AI, find new waypoint
        {
            // Debug.Log("Some enemies just collided. This shouldn't happen very often...");
            FindNewWaypoint();
        }

        if (other.gameObject.tag == "Player") // On collision with the player, explode and deal damage
        {
            // Debug.Log("Collided with player. Exploding to deal damage");
            Instantiate(explosion);
            MainManager.Player.DamageHull(collisionDamage);
            Destroy(this.gameObject);
        }
    }

    // Navigation functions
    public void FindAttackVector() // Use OverlapSphere to find nearest attack vector; set it as destination
    {
        Collider[] validAttackVectors = Physics.OverlapSphere(transform.position, attackVectorDetectionRadius, attackVectorLayer);
        if (validAttackVectors != null)
        {
            float tempDistance = attackVectorDetectionRadius;
            GameObject tempAttackVector = null;
            for (int i = 0; i < validAttackVectors.Length; i++) // find closest attack vector
            {
                float scoutToVector = Vector3.Distance(transform.position, validAttackVectors[i].transform.position);
                if (scoutToVector < tempDistance)
                {
                    tempAttackVector = validAttackVectors[i].gameObject;
                    tempDistance = scoutToVector;
                }
            }

            // Debug.Log("Found the closest attack vector!");
            destination = tempAttackVector.transform.position;
        }
        else
        {
            // Debug.Log("Scout couldn't find any attack vectors");
        }
    }

    public void OnTriggerEnter(Collider other) // Find a next destination when target waypoint reached
    {
        if (other.tag == "AttackVector") // If attack vector reached, dash attack at player
        {
            AttackPlayer();
        }

        if (other.tag == "Shield") // if the enemy collides with the front shield, they are destroyed at no damage to the player
        {
            Destroy(gameObject);
            MainManager.Player.DamageShield();
        }

        if (playerDetected == false) // On collision with regular waypoint, find new one if the player hasn't been detected
        {
            FindNewWaypoint();
        }

        if (other.tag == "PlayerDetectionArea")
        {
            // Debug.Log("Scout detected the player!");
            player = other.GetComponentInParent<PlayerMovement>().gameObject;
            FindAttackVector();
            AlertNearbyAI();
        }
    }

    public void FindNewWaypoint() // Sets the destination to a waypoint; first checks forward via SphereCast, but resorts to OverlapSphere if nothing is straight ahead
    {
        Ray forwardCheck = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(forwardCheck, forwardCheckRadius, forwardCheckDistance, waypointsLayer))
        {
            RaycastHit[] hit = Physics.SphereCastAll(transform.position, 5f, transform.forward, 3 * waypointDetectionRadius, waypointsLayer, QueryTriggerInteraction.Collide);
            int newWaypoint = UnityEngine.Random.Range(0, hit.Length);
            destination = hit[newWaypoint].transform.position;
            // Debug.Log("New destination found via SphereCast");
        }
        else
        {
            SphericalWaypointSelect();
        }
    }

    public void SphericalWaypointSelect() // Sets the destination to a waypoint found via OverlapSphere method
    {
        Collider[] viableWaypoints = Physics.OverlapSphere(transform.position, waypointDetectionRadius, waypointsLayer);
        int newWaypoint = UnityEngine.Random.Range(0, viableWaypoints.Length);
        Waypoint claimedTargetWaypointData = viableWaypoints[newWaypoint].gameObject.GetComponent<Waypoint>();
        claimedTargetWaypointData.claimed = true;
        destination = viableWaypoints[newWaypoint].transform.position;
        // Debug.Log("New destination found via OverlapSphere");
    }


    // Player Detected functions
    public void AlertNearbyAI() // Use OverlapSphere to contact other enemies and inform them of the player's position
    {
        Collider[] enemiesToAlert = Physics.OverlapSphere(transform.position, enemyCommsRadius, enemyLayer);
        int i = 0;
        while (i < enemiesToAlert.Length)
        {
            // Debug.Log("Scout is alerting nearby AI.");
            enemiesToAlert[i].SendMessage("Alert", player.transform.position);
            i++;
        }
    }

    public void Alert(Vector3 playerPosition) // function called byt AlertNearbyAI function; sets player as the destination
    {
        // Debug.Log("Scout has been alerted of player position by another scout.");
        playerDetected = true;
        destination = playerPosition;
    }

    public void AttackPlayer() // stop, turn to face the player, wait then dash at the player
    {
        destination = player.transform.position;
        thrust = 0;
        transform.rotation = Quaternion.LookRotation(player.transform.position);
        rbody.AddForce((destination - transform.position).normalized * dashAttackForce, ForceMode.Impulse);
    }
}