/* Written by Cameron Williams
 * 
 * This short script simply is attached to the 
 * map camera in the game scene and makes it follow
 * the player's x-z location so that screen properly
 * displays centered around the player.
 * 
 * Last updated 8/22/2019.
 */

using UnityEngine;

public class MapCamToPlayer : MonoBehaviour
{
    [SerializeField] private GameObject player = null;

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }
}