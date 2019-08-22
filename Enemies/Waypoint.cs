/* Written by Cameron Williams
 * 
 * This script defines a Waypoint type object that will be generated across
 * the terrain in a scene via the AIWaypoints script.
 * 
 * Last updated 8/21/2019
 */

using System.Collections;
using UnityEngine;

public class Waypoint: MonoBehaviour
{
    public bool claimed = false; // Set to true from AI pathfinder script; determines if a scout is already on it's way to this waypoint
    [SerializeField] [Tooltip("Time the waypoint will be disabled for after an enemy AI reaches it")] private int disableTimer = 10;
    [SerializeField] private bool disableOnCollision = true; // determines whether the waypoint will disappear temporarily when a scout touches it

    public void OnEnable()
    {
        claimed = false; // reset the claimed state once object is re-enabled
    }

    public void OnTriggerEnter(Collider other) // Runs the disable couroutine below if parameters met
    {
        if (other.tag == "Enemy" && disableOnCollision)
        {
            StartCoroutine(DisableWaypointTemporarily(this.gameObject));
        }
    }

    private IEnumerator DisableWaypointTemporarily(GameObject waypoint) // temporarily disable waypoint when an enemy touches it to avoid clustering of AI
    {
        waypoint.SetActive(false);
        yield return new WaitForSeconds(disableTimer);
        waypoint.SetActive(true);
    }
}
