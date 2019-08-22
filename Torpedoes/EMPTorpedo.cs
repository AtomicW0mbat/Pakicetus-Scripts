/* Written by Cameron Williams
 * 
 * Much like the HETorpedo, this torpedo effects enemies within an area.
 * However, instead of dealing damage to them via the TakeDamage method,
 * targets will be sent the HitByEMP(int) message that will disable them
 * for set amount of time or, if appropriate, disable them permanently.
 * This will be determined on a per-target basis via the targets stats.
 * 
 * Last updated 8/22/2019
 */

using UnityEngine;

public class EMPTorpedo : Torpedo
{
    [SerializeField] [Tooltip("Defines the area the radius of the area that the EMP will effect enemies")] public int empRadius = 5;
    [SerializeField] [Tooltip("Defines the amount of time that the enemies will be disabled for")] public int empTime= 5;
    [SerializeField] private ParticleSystem eMPEffect = null;
    [SerializeField] AudioSource empHitFish;
    [SerializeField] AudioSource empHitShip;

    public override void OnTriggerEnter(Collider other) // This method sends the HitByEMP message to enemies or the player depending on which entity fired the torpedo
    {
        if (friendly == true)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, empRadius);

            int i = 0;
            while (i < hitColliders.Length)
            {
                hitColliders[i].SendMessage("HitByEMP", empTime, SendMessageOptions.DontRequireReceiver);
                i++;
            }
            Instantiate(eMPEffect, other.transform);
            Destroy(this.gameObject);

            if (other.tag == "Enemy" || other.tag == "Environment")
            {
                empHitShip.Play();
            }

            if (other.tag == "Fish")
            {
                empHitFish.Play();
            }
        }
        
        else if (friendly == false)
        {
            if (other.tag == "Player")
            {
                MainManager.Player.HitByEMP(empTime);
                Instantiate(eMPEffect, other.transform);
                Destroy(this.gameObject);
            }
        }
    }
}
