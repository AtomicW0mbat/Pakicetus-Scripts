/* Written by Cameron Williams
 * 
 * This class defines the behavior of the fish that provide
 * food for the player when the player runs into them to pick
 * them up. Currently, their only pattern of movement is to 
 * "patrol" in a circle, but I added the ability to extend this
 * in the future, if desried.
 * 
 * Last updated 8/22/2019.
 */

using UnityEngine;

public class FoodFish : MonoBehaviour
{
    [SerializeField] int minFood = 5;
    [SerializeField] int maxFood = 25;
    private int foodAmount; // food amount is randomized in start function
    [SerializeField] private ParticleSystem fishParticles = null; // particle system that displays the fish models

    private int movementPatterns = 2; // # of movement patterns including 0 (stationary); can be expanded on
    private int selectedMovementPattern;

    Rigidbody _rbody;
    private float moveRadius; // randomized between the set interval below
    [SerializeField] private float moveRadiusMin = 0.2f;
    [SerializeField] private float moveRadiusMax = 4.0f;
    private float moveSpeed; // randomized between the set interval below
    [SerializeField] private float moveSpeedMin = 0.1f;
    [SerializeField] private float moveSpeedMax = 1.5f;
    private float timeCounter = 0; // for movement calculations

    void Start() // initialize variables for the food fish including movement pattern, speed, radius, and food amount
    {
        foodAmount = Random.Range(minFood, maxFood);
        selectedMovementPattern = Random.Range(0, movementPatterns);
        moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
        moveRadius = Random.Range(moveRadiusMin, moveRadiusMax);
        _rbody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other) // Gives food to the player when they run into the food fish swarm in game
    {
        if (other.tag == "Player")
        {
            fishParticles.Stop();
            MainManager.Inventory.GatherFood(foodAmount);
        }
    }

    private void FixedUpdate() // makes the fish move in the selected pattern
    {
        if (selectedMovementPattern == 1) 
        {
            CircularMotion();
        }
    }

    private void CircularMotion() // defines the circular "patrol" movement pattern; makes the rigidbody move in a circle
    {
        timeCounter += Time.deltaTime * moveSpeed;
        float x = Mathf.Cos(timeCounter * (1 / moveRadius));
        float y = 0;
        float z = Mathf.Sin(timeCounter * (1 / moveRadius));
        _rbody.AddForce(new Vector3(x, y, z));
        transform.rotation = Quaternion.LookRotation(_rbody.velocity.normalized, Vector3.up);
    }
}