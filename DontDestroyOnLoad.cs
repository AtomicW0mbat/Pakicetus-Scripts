/* Written by Cameron Williams
 * 
 * This short script simply marks objects in the game such
 * that they won't be destroyed when another scene is loading.
 * For example, this is used on the some audio on the main menu
 * scene so that it doesn't immediately cut out when the 
 * main game scene begins to load.
 * 
 * Last updated 8/22/2019
 */


using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
    
