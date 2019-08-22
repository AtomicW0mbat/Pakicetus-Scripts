/* Written by Cameron Williams
 * 
 * This subclass is super similar to the PlayerTurret subclass,
 * with the key difference being that it will be attached to turrets
 * on enemy ships and fire at the player.
 * 
 * Last updated 8/22/2019.
 */

using UnityEngine;

public class EnemyTurret : Turret
{
    [SerializeField] [Tooltip("Prefab for enemy piercing torpedo")] public GameObject torpedoPrefab;
    [SerializeField] [Tooltip("The layer in which the turret will look for the player to fire at")] private LayerMask playerLayer = 1; // default value; assign in inspector
    private float fireTime = 5.0f; // delay between shots fired from the turret
    [SerializeField] public bool seeking = false; // by default, no enemy turrets will have seeking capabilty so that the player can avoid enemy torpedoes
    [SerializeField] public float fireBubbleRadius = 4.5f; // default; assign in inspector if necesary
    [SerializeField] public float fireBubbleDistance = 4.5f; // default; assign in inspector if necessary
    [SerializeField] AudioSource fireSound = null;

    void FixedUpdate()
    {
        // Similar to other turrets, count to keep track of when the turret may fire, then select a target and fire a torpedo at it
        fireTime = baseFireTime;
        timer += Time.deltaTime;

        //Debug.Log("Value of timer is " + timer);

        if (timer >= fireTime)
        {
            Collider[] targetsInFireArea = Physics.OverlapSphere(transform.position + transform.forward * fireBubbleDistance, fireBubbleRadius, playerLayer, QueryTriggerInteraction.Collide);
            // Debug.Log("Found " + targetsInFireArea.Length + " targets in fire area");
            if (targetsInFireArea.Length > 0)
            {
               
                GameObject target = targetsInFireArea[0].gameObject;
                //turretHead.transform.LookAt(target.transform);
                //turretBarrel.transform.LookAt(hitInfo.transform);
                FireTorpedo(target.transform);

                timer = 0.0f; // reset timer for firing again
            }
        }
        timer = (timer >= fireTime) ? fireTime : timer; // Ternary operator just for fun
    }

    private void FireTorpedo(Transform target) // Instantiates the appropriate torpedo and sends it toward the target
    {
        _projectile = Instantiate(torpedoPrefab) as GameObject;
 
        _projectile.transform.position = transform.TransformPoint(0, 0, 0);
        _projectile.transform.LookAt(target);
        if (seeking)
        {
            _projectile.SendMessage("SetToSeeking", target.gameObject);
        }
        fireSound.Play();
    }

    void OnDrawGizmosSelected() // Visualize obstacle and waypoint detection areas, destination
    {
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, transform.forward);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * fireBubbleDistance, fireBubbleRadius);
    }
}
