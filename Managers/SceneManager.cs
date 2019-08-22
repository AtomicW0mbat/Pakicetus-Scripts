/* Created by Cameron Williams
 * 
 * This class will be used to manage scene related controls.
 * In particular, this script manages the map since it's not implemented
 * like a typical menu. Instead, it mananages which camera view is being
 * displayed (Main Camera or Map Camera). This script also freezes or
 * unfreezes time in game depending on the status of the various menus,
 * manages which messages get sent to the message log and when, and the
 * countdown for the final wave spawning.
 * 
 * This description was last updated 8/21/2019.
*/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour, IGameManager
{
    public Camera mainCamera;
    public Camera mapCamera;
    [SerializeField] GameObject player = null;
    [SerializeField] GameObject waypointManager = null; // WaypointManager in the scene; to be enabled on startup

    [SerializeField] private GameObject log_MessageParent = null;
    [SerializeField] private GameObject messagePrefab = null;

    [SerializeField] private LogMessage[] logMessages = null;

    [SerializeField] private GameObject hudPanel = null;

    [Header("Final Wave Options")]
    [SerializeField] private GameObject finalWavePanel = null;
    [SerializeField] private Text finalWaveCountdown = null;
    [SerializeField] private float timer = 120;
    private bool timerStarted = false;
    [SerializeField] private GameObject[] finalWaveSpawners = null;
    [SerializeField] private Animation outOfTime = null;

    public ManagerStatus status { get; private set; }

    public void Startup()
    {
        // Debug.Log("Time manager starting...");
        waypointManager.SetActive(true);
        status = ManagerStatus.Started;
    }

    [NonSerialized] public bool mapOpen = false;
    public void ToggleMapCamera() // Toggles which camera is being displayed in order to show the map
    {
        if (mapOpen)
        {
            mapCamera.enabled = false;
            mainCamera.enabled = true;
            mapOpen = false;
            hudPanel.SetActive(true);
            UnPauseGame();
        }
        else
        {
            mainCamera.enabled = false;
            mapCamera.enabled = true;
            mapOpen = true;
            hudPanel.SetActive(false);
            PauseGame();
        }
    }

    public void PauseGame() // Method used for pausing the game when opening in game menus
    {
        Time.timeScale = 0;
    }

    public void UnPauseGame() // Method used for unpausing the game when closing in game menus
    {
        Time.timeScale = 1;
    }

    public void Quit() // Method used for exiting the application
    {
        Application.Quit();
    }

    public void RestartScene() // Method used for restarting the current scene
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        UnPauseGame();
    }

    public GameObject GetPlayer() // Returns persistent reference to the player when required
    {
        return player;
    }

    public void GenerateMessage(LogMessage messageData) // Send message specified by messageData to the player's message log in the HUD
    {
        GameObject _newMessage = Instantiate(messagePrefab, log_MessageParent.transform);
        Message currentMessage = _newMessage.GetComponent<Message>();

        currentMessage.messageName.text = messageData.crewMemberCode.ToString();
        currentMessage.messageText.text = messageData.messageText;
        currentMessage.crewMemberImage.sprite = messageData.crewMemberSprite;
    }

    public void ActivateFinalWaveCountdown() // Start the final wave coroutine
    {
        StartCoroutine(FinalWaveCountdown());
    }

    private IEnumerator FinalWaveCountdown() // display final wave countdown in the HUD and activate the appropriate spawners when it expires
    {
        finalWavePanel.SetActive(true);
        while (timer > 0.0f) // should move this loop to a coroutine
        {
            timer -= Time.deltaTime;
            float minutes = Mathf.Floor(timer / 60);
            float seconds = Mathf.Floor(timer % 60);
            double centiseconds = timer - Math.Truncate(timer);

            finalWaveCountdown.text = minutes.ToString("00") + ":" + seconds.ToString("00") + centiseconds.ToString(".00");
            yield return null;
        }
        finalWaveCountdown.text = "00:00.00";
        outOfTime.Play();
        ActivateFinalWaveSpawners();
        yield return null;
    }

    private void ActivateFinalWaveSpawners() // Activate spawners in the finalWaveSpawners array
    {
        for (int i = 0; i < finalWaveSpawners.Length; i++)
        {
            finalWaveSpawners[i].SetActive(true);
        }
    }
}
