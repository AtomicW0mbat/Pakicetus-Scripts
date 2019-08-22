/* Written by Cameron Williams
 * 
 * This script is the primary game manager that handles references to all other
 * managers. Any object in the game referring to a manager will access it via
 * this script.
 * 
 * Updated 8/21/2019
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(UIManager))]
[RequireComponent(typeof(SceneManager))]

public class MainManager : MonoBehaviour
{
    public static SceneManager Scene { get; private set; }
    public static PlayerManager Player { get; private set; }
    public static InventoryManager Inventory { get; private set; }
    public static UIManager UI { get; private set; }

    private List<IGameManager> _startSequence;

    private void Awake() // Before game starts, establish references to all other managers and start them up
    {
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        UI = GetComponent<UIManager>();
        Scene = GetComponent<SceneManager>();
        _startSequence = new List<IGameManager>();
        _startSequence.Add(Scene);
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);
        _startSequence.Add(UI);
        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers() // Coroutine to startup all managers in the _startSequence list
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;
        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                {
                    numReady++;
                }
            }

            if (numReady > lastReady)
            {
                // Debug.Log("Progress: " + numReady + "/" + numModules);
                
            }
            yield return null;
        }

        // Debug.Log("All managers started up");
    }
}
