/* Written by Cameron Williams
 * 
 * Defines the properties that all projectiles should inherit. This includes a rate of fire, 
 * speed of the torpedo, ability to seek targets, and whether the torpedo is friendly.
 * 
 * Last updated 8/22/2019.
 */

using UnityEngine;

public abstract class Torpedo : MonoBehaviour
{
    [SerializeField] [Tooltip("How fast the projectile moves")] protected int speed = 5;
    [SerializeField] [Tooltip("Turret fire speed when using this projectile")] public int fireSpeed = 2;
    public bool seeker = false; // Ability to track targets; public so that the turret can tell the torpedo to seek
    public bool friendly; // Defines whether the torpedo will target enemies or the player
    [SerializeField] protected GameObject explosion;
    private GameObject target;
    [SerializeField] private float turnSpeed = 0.5f;

    void Update()
    {
        // propel torpedo forward at the defined speed and seek targets if seeking is enabled
        if (seeker)
        {
            if (target != null)
            {
                Vector3 dir = target.transform.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, dir, turnSpeed * Time.deltaTime, 0f);
                Debug.DrawRay(transform.position, dir, Color.red);
                transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
        transform.Translate(0, 0, speed * Time.deltaTime); // all projectiles move forward after instantiation
    }

    public void SetToSeeking(GameObject targetToSeek) // receive message from turret to make the torpedo seek
    {
        Debug.Log("Torpedo will now seek enemies");
        seeker = true;
        target = targetToSeek;
    }

    public abstract void OnTriggerEnter(Collider other); // all projectiles should respond to contacting targets in some way
}
