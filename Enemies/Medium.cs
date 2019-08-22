/* Written by Cameron Williams
 * 
 * This is the second enemy the player will encounter. This
 * enemy waits at hydrothermal vents rich in resources until the player
 * comes along or moves to attack the player if infomred of player's position.
 * When in range, the medium enemy will fire explosive torpedoes at the player.
 * 
 * Written by Cameron Williams on 8/21/2019
 */

using UnityEngine;
using System.Collections;

public class Medium : MonoBehaviour, IEnemyAI
{
    // Variables from EnemyAI interface
    public int hitPoints { get; private set; }
    public float moveSpeed { get; private set; }
    [SerializeField]
    public bool pierceVulnerability { get; private set; } = false;
    public bool hEVulnerability { get; private set; } = true;
    public bool eMPVulnerability { get; private set; } = true;
    public bool disabled { get; private set; } = false;
    [Header("EnemeyAI Variables Initialization")]
    [SerializeField] [Tooltip("Amount of HP this enemy prefab will start with")] private int hP = 25;

    // Medium Specific variables
    [Header("Navigation")]
    private Rigidbody rbody;
    private Vector3 destination;
    bool playerDetected = false;
    [SerializeField] private float thrust = 3f;
    private GameObject player;
    [SerializeField] private float playerSpacing = 2.0f;

    void OnDrawGizmosSelected() // Visualize obstacle and waypoint detection areas, destination
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, destination); // Blue line indicating the scouts intended destination
    }

    // EnemyAI interface function
    public void TakeDamage(int damage) // Subtract damage from hitPoints; destroy object if hitPoints 0 or below
    {
        float newDamage = damage * 0.9f; // the armor on the medium enemy reduces damage taken by 10%
        hitPoints = (int) Mathf.Floor((float)hitPoints - newDamage);
        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void HitByEMP(int disableTime) // Executes the DisabledByEMP couroutine
    {
        StartCoroutine(DisabledByEMP(disableTime));
    }

    private IEnumerator DisabledByEMP(int disableTime) // Coroutine to be called by HitByEMP; mark enemy as disabled and make it use gravity for the duration to make it slowly drift to the seafloor
    {
        disabled = true;
        rbody.useGravity = true; // temporarily enable low gravity to make the scout slowly drift to the sea floor
        rbody.constraints = RigidbodyConstraints.None;
        yield return new WaitForSeconds(disableTime);
        disabled = false;
        rbody.constraints = RigidbodyConstraints.FreezeRotationX;
        rbody.useGravity = false;
    }

    // MonoBehaviour functions
    void Start()
    {
        // initialize variables
        disabled = false;
        hitPoints = hP;
        rbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate() // Move towards target destination and avoid obstacles; always look at destination
    {
        if (!disabled && playerDetected)
        {
            destination = player.transform.position;
            if (Vector3.Distance(transform.position, player.transform.position) > playerSpacing)
            {
                rbody.AddForce((destination - transform.position).normalized * thrust);
                if (rbody.velocity.normalized != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(rbody.velocity.normalized, Vector3.up);
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other) // Find a next destination when target waypoint reached
    {
        if (other.tag == "PlayerDetectionArea")
        {
            // Debug.Log("Scout detected the player!");
            player = other.GetComponentInParent<PlayerMovement>().gameObject;
            playerDetected = true;
            //AlertNearbyAI();
        }
    }
}