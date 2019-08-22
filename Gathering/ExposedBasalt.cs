/* Created by Cameron Williams
 * 
 * This script defines the minimum and maximum values of resources minable from
 * the Exposed Basalt prefabs with which it is attached. These values are
 * used in the Start method to roll values at startup based on this range.
 * 
 * Last updated 6/24/2019
 */

using UnityEngine;

public class ExposedBasalt : GatheringTrigger
{
    // Note that the following serialized fields should be set in the Exposed Basalt prefab
    // From there, all instances of the Exposed Basalt gathering points in the game will
    // then be given ranges for provided materials.

    protected override GatheringPoints gatheringPoint => GatheringPoints.Basalt;
    [SerializeField] [Tooltip("Minimum amount of iron that can be provided by Exposed Basalt gathering points")] public int minIron;
    [SerializeField] [Tooltip("Maximum amount of iron that can be provided by Exposed Basalt gathering points")] public int maxIron;
    [SerializeField] [Tooltip("Minimum amount of magnesium that can be provided by Exposed Basalt gathering points")] public int minMagnesium;
    [SerializeField] [Tooltip("Maximum amount of magnesium that can be provided by Exposed Basalt gathering points")] public int maxMagnesium;
    [SerializeField] [Tooltip("Minimum amount of silicon that can be provided by Exposed Basalt gathering points")] public int minSilicon;
    [SerializeField] [Tooltip("Maximum amount of silicon that can be provided by Exposed Basalt gathering points")] public int maxSilicon;

    void Start()
    {
        suppliesProvided[(int)Supplies.iron] = Random.Range(minIron, maxIron);
        suppliesProvided[(int)Supplies.magnesium] = Random.Range(minMagnesium, maxMagnesium);
        suppliesProvided[(int)Supplies.silicon] = Random.Range(minSilicon, maxSilicon);
    }
}
