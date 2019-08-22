/* Written by Cameron Williams
 * 
 * The purpose of this script is to be attached to a trigger volume that
 * marks the end of a map and the beginning of the next one. When the player
 * enters the trigger volume, the next map will be readied for play. In other
 * words dialgue may be activated and spawn timers may be started, among
 * other things.
 * 
 * Written on 7/24/2019
 */

using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] previousMapEnemySpawners = null; // spawners to disable from previous map
    [SerializeField] private GameObject[] nextMapEnemySpawners = null; // Associated enemy spawners for the next map area
    [SerializeField] private GameObject[] dialogueToQueue = null; // Associated dialogue
    [SerializeField] private GameObject[] gatheringSets = null;
    [SerializeField] private string areaName;

    bool finalwaveactivator = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            // deactivate old map
            for (int i = 0; i < previousMapEnemySpawners.Length; i++)
            {
                previousMapEnemySpawners[i].SetActive(false); // deactivate old spawners
            }

            // activate new map
            for (int i = 0; i < nextMapEnemySpawners.Length; i++) // activate spawners associated with this map
            {
                nextMapEnemySpawners[i].SetActive(true);
            }

            for (int i = 0; i < dialogueToQueue.Length; i++) // activate messages in log; need to queue up associated dialogues
            {
                dialogueToQueue[i].SetActive(true);
            }

            gatheringSets[Random.Range(0, gatheringSets.Length)].SetActive(true); // chooses a gathering set to enable

            if (finalwaveactivator)
            {
                MainManager.Scene.ActivateFinalWaveCountdown();
            }

            MainManager.UI.DisplayAreaName(areaName);
        }
    }
}
