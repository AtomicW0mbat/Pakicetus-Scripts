/* Written by Cameron Williams
 * 
 * This script defines the behavior of both the spiky fish and the smooth fish.
 * Both of them swim in a cirular pattern until they detect the player
 * in which they will approach and attack, which plays an animation.
 * 
 * Last updated 8/21/2019.
 */

using UnityEngine;

public class SpikyFish : MonoBehaviour, IEnemyAI
{
    // Interface variables and functions
    public int hitPoints { get; private set; } = 25;
    public float moveSpeed { get; private set; } = 1;
    public bool pierceVulnerability { get; private set; } = true;
    public bool hEVulnerability { get; private set; } = true;
    public bool eMPVulnerability { get; private set; } = false;
    public bool disabled { get; private set; } = false;

    public void TakeDamage(int damage) // Subtract damage from hitPoints; destroy object if hitPoints 0 or below
    {
        hitPoints = hitPoints - damage;
        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    // Other variables and functions
    private float timeCounter = 0;
    [SerializeField] private float patrolRadius = 0.5f;
    private Rigidbody _rbody;
    private bool playerDetected = false;
    private GameObject player = null;
    private Animator fishAnimtor;
    [SerializeField] private int attackDamage = 1;

    private void Start()
    {
        // initialize variables
        _rbody = GetComponent<Rigidbody>();
        fishAnimtor = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (playerDetected)
        {
            ApproachPlayer(); // execute active behavior when player is detected
        }
        else
        {
            Patrol(); // execute passive behavior when the player hasn't been detected
        }
    }

    void OnTriggerEnter(Collider other) // Use trigger volume to detect player
    {
        if (other.tag == "Player")
        {
            playerDetected = true;
            player = other.gameObject;
        }
    }

    private void OnCollisionEnter(Collision other) // Use collision with player to activate attack animation and deal damage to the player
    {
        if (other.gameObject.tag == "Player")
        {
            fishAnimtor.SetTrigger("Attack");
            MainManager.Player.DamageHull(attackDamage);
        }
    }
    
    private void Patrol() // Makes the fish move in a circular "patrol" pattern
    {
        timeCounter += Time.deltaTime * moveSpeed;
        float x = Mathf.Cos(timeCounter * (1/patrolRadius));
        float y = 0;
        float z = Mathf.Sin(timeCounter * (1/patrolRadius));
        _rbody.AddForce(new Vector3(x, y, z));
        transform.rotation = Quaternion.LookRotation(_rbody.velocity.normalized, Vector3.up);
    }

    private void ApproachPlayer() // Makes the fish move towards the player
    {
        _rbody.AddForce((player.transform.position - transform.position).normalized * moveSpeed);
        transform.rotation = Quaternion.LookRotation(_rbody.velocity.normalized, Vector3.up);
    }
}
