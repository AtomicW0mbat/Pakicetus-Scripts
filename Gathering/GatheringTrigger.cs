/* Created by Cameron Williams
 *
 * This script will be attached to invisible trigger volumes that will in turn
 * be attached to gathering point prefabs such that when the player enters the 
 * area, the Inventory Manager is informed of the amount of resources the 
 * gathering point provides.
 * 
 * Last updated 8/21/2019.
*/

using UnityEngine;
using System;

public abstract class GatheringTrigger : MonoBehaviour
{
    protected int[] suppliesProvided = new int[Enum.GetNames(typeof(Supplies)).Length]; // create new array of potential supplies that will be gatherable
    protected abstract GatheringPoints gatheringPoint { get; }
    public enum gatheringType { Collection, Filtration }
    [SerializeField] public gatheringType gatheringTypes;
    [NonSerialized] public bool eventPlumeActive = false;
    [NonSerialized] protected int eventPlumeDamage = 2;
    private float eventPlumeDamageTimer;
    private float eventPlumeDamageTimerThreshold;

    // Children of this class should all have a Startup() method that rolls numbers for their respective gatherable resources

    private void OnTriggerEnter(Collider other)  // make supplies provided by this resource location gatherable and enable mining button
    {
        if (other.tag == "Player")
        {
            MainManager.Inventory.SetSuppliesGatherable(suppliesProvided);
            MainManager.UI.EnableGatheringPanel();
            SetGatheringPointTitleAndElements();
            if (GetComponentInChildren<MapIcon>() != null)
            {
                MapIcon gathering_point_icon = GetComponentInChildren<MapIcon>();
                gathering_point_icon.EnableMapIcon();
            }
        }
    }

    private void SetGatheringPointTitleAndElements() // displays the appropriate resources on the HUD indicating the elements provided by the gathering point
    {
        // if resourcesgatherable index > 0, enable associated tag
        for (int i = 0; i < Enum.GetNames(typeof(Supplies)).Length; i++)
        {
            if (suppliesProvided[i] > 0)
            {
                MainManager.UI.gathering_elementBoxes[i].SetActive(true);
            }
            else
            {
                MainManager.UI.gathering_elementBoxes[i].SetActive(false);
            }
        }
        // set title of gatherable resource depending on gathering point type
        MainManager.UI.gatheringPanelTitle.text = this.GetType().Name;
    }

    private void OnTriggerExit(Collider other) // clear the gatherables when exiting area and disable mining button
    {
        if (other.tag == "Player")
        {
            if (MainManager.Player.GetMovement() == false) // disables gathering if player knocked out of gathering zone while gathering
            {
                MainManager.UI.ToggleGathering();
            }
            MainManager.Inventory.ResetSuppliesGatherable();
            MainManager.UI.DisableGatheringPanel();
        }
    }

    private void OnTriggerStay(Collider other) // slowly damages the player while they are inside of a gathering point effected by an event plume
    {
        if (other.tag == "Player" && Input.GetKeyDown(KeyCode.G))
        {
            MainManager.UI.ToggleGathering();

            if (eventPlumeActive)
            {
                eventPlumeDamageTimer += Time.deltaTime;
                if (eventPlumeDamageTimer >= eventPlumeDamageTimerThreshold)
                {
                    MainManager.Player.DamageHull(eventPlumeDamage);
                    eventPlumeDamageTimer = 0.0f; // reset timer
                }
            }
        }
    }
}