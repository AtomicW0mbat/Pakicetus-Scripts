/* Written by Cameron Williams
 * 
 * This script manages all of the logic in the Main Menu scene.
 * 
 * Last updated 8/21/2019.
 */

using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] [Tooltip("The audio source containing the title music clip")] private AudioSource titleMusic;
    [SerializeField] [Tooltip("The UI Panel containing the credits")] private GameObject creditsPanel;
    [SerializeField] [Tooltip("The UI Panel containing the settings")] private GameObject settingsPanel;

    public void Credits() // method executed when the Credits button is pressed
    {
        creditsPanel.SetActive(true);
    }

    public void NewGame() // method executed when the New Game button is pressed
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void LoadGame() // method executed when the Load Game button is pressed
    {
        // TODO: Implement save/load mechanic and make this button load the save file
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void Settings() // method executed when the Settings button is pressed
    {
        settingsPanel.SetActive(true);
    }

    public void Back() // method executed when the Back button is pressed
    {
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void QuitToDesktop() // method executed when the Quit button is pressed
    {
        Application.Quit();
    }
}
