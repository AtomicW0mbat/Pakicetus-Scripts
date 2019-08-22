/* Written by Cameron Williams
 * 
 * This class manages the triggers for dialogue by
 * playing the appropriate audio and queuing up it's
 * matching chain of log messages to appear at the right times
 * 
 * Last updated 8/22/2019
 */

using System.Collections;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private bool activated = false; // used to ensure the trigger is only activated once
    [SerializeField] public Collider _collider;
    [SerializeField] private AudioClip _dialogueAudio; // to be changed in the inspector; specific to each trigger
    [SerializeField] public LogMessage[] dialogueChain; // array of the log messages that corresponds to the dialogue audio in the clip above

    void OnTriggerEnter(Collider other)
    {
        // when the player enters the trigger volume, start the dialogue chain if it hasn't already played out
        if (other.tag == "Player" && !activated)
        {
            activated = true;
            MainManager.UI.dialogueSource.clip = _dialogueAudio;
            MainManager.UI.dialogueSource.Play();
            StartCoroutine(SendLogMessages(dialogueChain));
        }
    }

    private IEnumerator SendLogMessages(LogMessage[] _dialogueChain)
    {
        // This coroutine iterates through the dialogueChain array, playing each message and waiting for the specified time to keep the log messages in sync with the audio
        for (int i = 0; i < dialogueChain.Length; i++)
        {
            MainManager.Scene.GenerateMessage(dialogueChain[i]);
            yield return new WaitForSeconds(dialogueChain[i].logMessageDelay);
        }
        yield return null;
    }
}