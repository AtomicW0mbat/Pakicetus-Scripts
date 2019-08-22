/* Created by Cameron Williams
 * 
 * This script is made to be attached to an object consisting of a circle of 
 * AI Attack Vectors such that the circle follows the player without rotating
 * or tilting. This will make the AI move in a much more natural way.
 * 
 * Last updated on 8/21/2019.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackVectorRing : MonoBehaviour
{
    [SerializeField] private GameObject objectToFollow = null;

    void Update()
    {
        transform.position = objectToFollow.transform.position; // make the waypoint ring stick to the object (it's only use is on the player right now)
    }
}
