/* Written by Cameron Williams
 * 
 * The most common type of torpedo that the player starts with. Inherits from
 * Torpedo abstract class. Deals damage to player or enemies depending on
 * whether or not it is tagged as friendly. Enemies should have the 
 * TakeDamage(int) method in order to recieve damage from this projectile.
 * 
 * This was last updated 8/22/2019.
 */

using UnityEngine;

public class PierceTorpedo : Torpedo
{
    [SerializeField] [Tooltip("Amount of damage this torpedo deals to the target it hits")] public int damage;
    [SerializeField] private AudioSource pierceHitFish;
    [SerializeField] private AudioSource pierceHitShip;

    public override void OnTriggerEnter(Collider other) // On conctact, damages the either player or enemies depending on which entity fired it
    {
        if (friendly == true)
        {
            if (other.tag == "Enemy") // only trigger on contact with enemy
            {
                other.SendMessage("TakeDamage", damage); // target must have a TakeDamage method to work
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(this.gameObject);

                if (other.tag == "Enemy" || other.tag == "Environment")
                {
                    pierceHitShip.Play();
                }

                if (other.tag == "Fish")
                {
                    pierceHitFish.Play();
                }
            }
        }
        
        else if (friendly == false)
        {
            if (other.tag == "Player") // only triggers when hitting player
            {
                MainManager.Player.DamageHull(damage);
                Instantiate(explosion, transform.position, transform.rotation);
                Destroy(this.gameObject);
            }
        }
    }
}
