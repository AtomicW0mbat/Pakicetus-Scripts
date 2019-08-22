/* Written by Cameron Williams
 * 
 * This is a base class inherited by other turret classes that 
 * defines some basic variables that all other turrets use.
 * 
 * Last updated 8/22/2019
 */


using UnityEngine;

public abstract class Turret : MonoBehaviour
{
    [SerializeField] protected float baseFireTime;

    protected GameObject _projectile; // temporarily contains information of GameObject to be fired so it can be manipulated on instantiation
    protected RaycastHit hitInfo; // contains GameObject first contacted by SphereCast
    // Could modify this later to contain an array 
    protected float timer = 0.0f;
}