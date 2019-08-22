/* Created by Cameron Williams
 * 
 * This script defines the minimum and maximum values of resources minable from
 * the Basalt Vent prefabs with which it is attached. These values are
 * used in the Start method to roll values at startup based on this range.
 * 
 * If a nearby event plume is activated, output of the vent is multiplied by
 * the inputted event plume multiplier. (This has not yet been tested)
 * 
 * Last updated 8/21/2019.
 */

using UnityEngine;

public class BasaltHostedVent : GatheringTrigger
{
    // Note that the following serialized fields should be set in the Basalt Hosted Vent prefab
    // From there, all instances in the game will then be given ranges for provided materials.

    protected override GatheringPoints gatheringPoint => GatheringPoints.BasaltHostedVent;
    [SerializeField] [Tooltip("Minimum amount of hydrogen that can be provided by Basalt Hosted Vent gathering points")] public int minHydrogen;
    [SerializeField] [Tooltip("Maximum amount of hydrogen that can be provided by Basalt Hosted Vent gathering points")] public int maxHydrogen;
    [SerializeField] [Tooltip("Minimum amount of sulfur that can be provided by Basalt Hosted Vent gathering points")] public int minSulfur;
    [SerializeField] [Tooltip("Maximum amount of sulfur that can be provided by Basalt Hosted Vent gathering points")] public int maxSulfur;
    [SerializeField] [Tooltip("Minimum amount of rare earth elements that can be provided by Basalt Hosted Vent gathering points")] public int minRareEarthElements;
    [SerializeField] [Tooltip("Maximum amount of rare earth elements that can be provided by Basalt Hosted Vent gathering points")] public int maxRareEarthElements;

    void Start()
    {
        suppliesProvided[(int)Supplies.hydrodgen] = Random.Range(minHydrogen, maxHydrogen);
        suppliesProvided[(int)Supplies.sulfur] = Random.Range(minSulfur, maxSulfur);
        suppliesProvided[(int)Supplies.rareEarthMetals] = Random.Range(minRareEarthElements, maxRareEarthElements);
    }

    public void EventPlume(int eventPlumeMultiplier, int _eventPlumeDamage) // this method gets executed by an event plume gameObject, multiplying the output of the supplies provided and slowly causing damage
    {
        eventPlumeActive = true;
        eventPlumeDamage = _eventPlumeDamage;
        for (int i = 0; i < suppliesProvided.Length; i++)
        {
            suppliesProvided[i] = suppliesProvided[i] * eventPlumeMultiplier;
        }
    }
}
