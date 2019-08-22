/* Written by Cameron Williams
 * 
 * This script will be attached the the whale fall/dead fish
 * game object and will randomly select points around the model
 * to instantiate food fish prefabs at.
 * 
 * Last updated 8/22/2019.
 */

using UnityEngine;

public class WhaleFall : MonoBehaviour
{
    [SerializeField] private GameObject foodFishPrefab;
    // The following two variables define a range of food fish to be spawned around the whale fall
    [SerializeField] private int minFoodFish;
    [SerializeField] private int maxFoodFish;
    // The follwing three variables define a space around the whale fall with which food fish may be spawned
    [SerializeField] private float fishSpawn_x_max = 5;
    [SerializeField] private float fishSpawn_y_max = 5;
    [SerializeField] private float fishSpawn_z_max = 5;

    private void OnDrawGizmosSelected() // In-editor visualization of the are where food fish may be spawned
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - Vector3.right * fishSpawn_x_max, transform.position + Vector3.right * fishSpawn_x_max);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * fishSpawn_y_max);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position - Vector3.forward * fishSpawn_z_max, transform.position + Vector3.forward * fishSpawn_z_max);
    }

    void Start()
    {
        // Semi-randomly instantiates food fish in the area around the whale fall
        int foodFishToSpawn = Random.Range(minFoodFish, maxFoodFish);
        for (int i = 0; i < foodFishToSpawn; i++)
        {
            float _x = Random.Range(-fishSpawn_x_max, fishSpawn_x_max);
            float _y = Random.Range(0, fishSpawn_y_max);
            float _z = Random.Range(-fishSpawn_z_max, fishSpawn_z_max);
            Vector3 foodFishPostition = new Vector3(_x, _y, _z) + transform.position;
            Instantiate(foodFishPrefab, foodFishPostition, Quaternion.identity);
        }
    }
}
