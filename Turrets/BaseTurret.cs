/* Written by Cameron Williams
 * 
 * This script is made to be a sort of variaton on the player turret script for
 * the turrets that can be constructed on top of towers in the final area of the game.
 * It will have similar attributes, except will fire only one torpedo type and
 * won't be affected by player UI.
 * 
 * Last updated 8/22/2019.
 */

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BaseTurret : Turret
{
    private float fireTime; // fire time gets calculated in Update. Should probably move this call from this classes Update() to player manager's change crew on turrets methods
    [SerializeField] AudioSource fireSound = null;
    [SerializeField] [Tooltip("The torpedo this turret will fire")] public GameObject baseTurretTorpedo;
    [SerializeField] private LayerMask enemyLayer = 1;

    [SerializeField] private GameObject turretHead = null;
    [SerializeField] private GameObject turretBarrel = null;
    [SerializeField] public float fireBubbleRadius = 4.5f;
    [SerializeField] public float fireBubbleDistance = 4.5f;

    void OnDrawGizmosSelected() // Visualizaiton of turrets target detection/firing area
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - transform.up * fireBubbleDistance, fireBubbleRadius);
    }

    void FixedUpdate()
    {
        // counts to keep track of how quickly it can fire; scans for targets using OverlapSphere and selects one to fire upon
        fireTime = baseFireTime;
        timer += Time.deltaTime;

        //Debug.Log("Value of timer is " + timer);

        if (timer >= fireTime)
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
        _projectile = Instantiate(baseTurretTorpedo) as GameObject;
        _projectile.transform.position = transform.TransformPoint(0, 0, 0);
        _projectile.transform.LookAt(target);
        _projectile.SendMessage("SetToSeeking", target.gameObject);
        fireSound.Play();
    }
}
