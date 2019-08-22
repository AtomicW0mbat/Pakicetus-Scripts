/* Written Cameron Williams
 * 
 * Defines a ScriptableObject of type TechCostData that contains the resource
 * cost and time taken to develop a specific tech. New TechCostData scriptable
 * objects may be created in the project via the regular create asset menu in
 * the Unity Editor and their values may then be defined in the Inspctor.
 * References to these TechCostData objects will then need to be creatd in 
 * the InventoryManager. Logic for making the techs buildable would then be
 * added to the UIManager script and gameplay effects can be added where
 * deemed appropriate.
 *
 * Last updated 8/22/2019
 */

using UnityEngine;
using System;

[CreateAssetMenu]
public class TechCostData: ScriptableObject
{
    [SerializeField] [Tooltip("The name of the technology to be constructed or researched")]
    public string techName;
    [SerializeField] [Tooltip("Array of integers defining cost to purchase upgrade; order in Supplies enum")]
    public int[] cost = new int[Enum.GetNames(typeof(Supplies)).Length];
    [SerializeField] [Tooltip("Intger value defining number of seconds to comoplete upgrade")]
    public int researchRequired;
    [SerializeField] [Tooltip("Render image of the tech to be displayed in the tech cost panel on mouseover")]
    public Texture techImage;
    [SerializeField] [Tooltip("Description of the item that will appear in the cost panel on mouseover")]
    public string description = "";
}