/* Written by Cameron Williams
 * 
 * Inherits from abstract class Torpedo. This is teh second type of torpedo
 * to be found in the game. Uses explosionDamage and explosionRadius variables
 * to determine the area of effect of the explosion it causes on impact. Will
 * only explode on hitting the target (determined by being marked as friendly
 * or unfriedly). However, the all targets in the explosion radius will take
 * damage (as long as it's not a friendly torpedo exploding on an enemy). To
 * be effected by the explosion, enemy targets must h ave the TakeDamage(int)
 * method.
 * 
 * Last updated 8/22/2019
 */


using UnityEngine;

public class HETorpedo : Torpedo
{
    [SerializeField] [Tooltip("Area around torpedo with which enemies will take damage")] float explosionRadius = 5.0f;
    [SerializeField] [Tooltip("Damage the explosion does")] int explosionDamage = 75;
    [SerializeField] private AudioSource expHitFish;
    [SerializeField] private AudioSource expHitShip;

    public override void OnTriggerEnter(Collider other) // Torpedo explodes when it hits something, damages the player or enemies in range of the explosion depending on which entity fired it
    {
        if (friendly == true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

            int i = 0;
            while (i < hitColliders.Length)
            {
                hitColliders[i].SendMessage("TakeDamage", explosionDamage, SendMessageOptions.DontRequireReceiver);
                i++;
            }
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(this.gameObject);

            if (other.tag == "Enemy" || other.tag == "Environment")
            {
                expHitShip.Play();
            }

            if (other.tag == "Fish")
            {
                expHitFish.Play();
            }
        }
        
        else if (friendly == false)
        {
            if (other.tag == "Player" || other.tag == "Terrain" || other.tag == "Environment") // only trigger on contact with player, but still damages nearby
            {
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

                int i = 0;
                while (i < hitColliders.Length)
                {
                    if (hitColliders[i].gameObject.GetComponent<PlayerMovement>() != null)
                    {
                        MainManager.Player.DamageHull(explosionDamage);
                    }
                    i++;
                }
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(this.gameObject);
            }
        }
    }
}