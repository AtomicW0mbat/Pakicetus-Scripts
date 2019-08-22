/* Created by Cameron Williams
 * 
 * This script defines the minimum and maximum values of resources minable from
 * the Exposed Serpentinite prefabs with which it is attached. These values are
 * used in the Start method to roll values at startup based on this range.
 * 
 * Last updated 6/24/2019
 */

using UnityEngine;

public class ExposedSerpentinite : GatheringTrigger
{
    // Note that the following serialized fields should be set in the Exposed Serpentinite prefab
    // From there, all instances of the Exposed Serpentinite gathering points in the game will
    // then be given ranges for provided materials.

    protected override GatheringPoints gatheringPoint => GatheringPoints.Serpentinite;
    [SerializeField] [Tooltip("Minimum amount of chromium that can be provided by Exposed Serpentinite gathering points")] public int minChromium;
    [SerializeField] [Tooltip("Maximum amount of chromium that can be provided by Exposed Serpentinite gathering points")] public int maxChromium;

    void Start()
    {
        suppliesProvided[(int)Supplies.chromium] = Random.Range(minChromium, maxChromium);
    }
}
