/* Created by Cameron Williams
 * 
 * This script defines the minimum and maximum values of resources minable from
 * the Serpentinite Vent prefabs with which it is attached. These values are
 * used in the Start method to roll values at startup based on this range.
 * 
 * Last updated 6/24/2019
 */

using UnityEngine;

public class SerpentiniteHostedVent : GatheringTrigger
{
    // Note that the following serialized fields should be set in the Serpentinite Hosted Vent prefab
    // From there, all instances in the game will then be given ranges for provided materials.

    protected override GatheringPoints gatheringPoint => GatheringPoints.SerpentiniteHostedVent;
    [SerializeField] [Tooltip("Minimum amount of hydrogen that can be provided by Serpentinite Hosted Vent gathering points")] public int minHydrogen;
    [SerializeField] [Tooltip("Maximum amount of hydrogen that can be provided by Serpentinite Hosted Vent gathering points")] public int maxHydrogen;
    [SerializeField] [Tooltip("Minimum amount of methane that can be provided by Serpentinite Hosted Vent gathering points")] public int minMethane;
    [SerializeField] [Tooltip("Maximum amount of methane that can be provided by Serpentinite Hosted Vent gathering points")] public int maxMethane;

    void Start()
    {
        suppliesProvided[(int)Supplies.hydrodgen] = Random.Range(minHydrogen, maxHydrogen);
        suppliesProvided[(int)Supplies.methane] = Random.Range(minMethane, maxMethane);
    }
}
