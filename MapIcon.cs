/* Written by Cameron Williams
 * 
 * The purpose of this script is to convert Map Icon game objects
 * that are children of the objects they represent to use global
 * coordinates and rescale/rerotate them so that they don't have
 * the rotation and scale of their parent objects.
 *
 * Last updated 8/2/2019 
 */
using UnityEngine;

public class MapIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mapIconRenderer;

    private void Awake()
    {
        transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);
        transform.rotation = Quaternion.identity;
        transform.Rotate(90, 90, 0, Space.World);
        mapIconRenderer.enabled = false; // don't show map icons on map until player has gotten close enough to map them
    }

    public void EnableMapIcon()
    {
        mapIconRenderer.enabled = true;
    }
}