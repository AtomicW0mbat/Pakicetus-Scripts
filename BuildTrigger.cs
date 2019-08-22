/* Written by Cameron Williams
 * 
 * The purpose of this script is to function as a triggering mechanism for
 * allowing the player to build structures in the final area of the game.
 * It will function similarly to the GatheringTrigger script, but instead of
 * making resources at a gathering point available, it will cost resourcs to
 * construct a building.
 * 
 * Last updated 8/22/2019
 */

using System;
using UnityEngine;


public class BuildTrigger : MonoBehaviour
{
    [SerializeField] public string structureName;
    [SerializeField] private GameObject buildIndicator = null; // the object representing that there is a buildable structure in the area
    [SerializeField] private GameObject buildableObject = null; // the object that will be built
    [SerializeField] private bool structureBuilt; // keeps track of whether the object has been construcuted
    [SerializeField] public int[] suppliesCost = new int[Enum.GetNames(typeof(Supplies)).Length];

    private void OnTriggerEnter(Collider other)
    {
        // When the player enters the buildable object area and the object isn't built, displays the build menu for the player to interact with
        if (other.tag == "Player" && !structureBuilt) // enable build button and display cost when the player enters
        {
            MainManager.UI.buildingReference = this; // Send reference to this structure to the UI
            MainManager.Inventory.SetSuppliesCost(suppliesCost);
            MainManager.UI.EnableBuildPanel();

            if (MainManager.UI.ResourceRequirementsMet(suppliesCost) && !structureBuilt) // Enable build button if the material cost of the strucutre is met and it's not built yet
            {
                MainManager.UI.EnableBuildButton();
            }
            else if (structureBuilt)
            {
                MainManager.UI.DisableBuildButton();
            }
            else
            {
                MainManager.UI.DisableBuildButton();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // when the player leaves the area, disable the build menu and reset appropriate variables
        if (other.tag == "Player") // disable build button and remove cost display on exit
        {
            MainManager.UI.DisableBuildPanel();
            MainManager.Inventory.ResetSuppliesCost();
            MainManager.UI.DisableBuildButton();
        }
    }

    public void Build() // function to build the object
    {
        buildIndicator.SetActive(false);
        buildableObject.SetActive(true);
        structureBuilt = true;
    }
}
