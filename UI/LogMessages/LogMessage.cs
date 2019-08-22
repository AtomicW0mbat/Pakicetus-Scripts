/* Written by Cameron Williams
 * 
 * This class defines the structure of all log messages that appear in game.
 * It contains an enumeration for all message authors, the particular author
 * of the message, the message to be sent, the crew member sprit to be displayed
 * with the message, and the delay it takes for the associated message's dialogue
 * to play out.
 * 
 * Much like the TechCostData class, this is scriptable object. Instances of the
 * scriptable object can be created in Unity and their respective fields may be
 * filled out to appropriately represent a single message to be displayed in
 * the log.
 *
 * Last updated 8/22/2019. 
 */


using UnityEngine;

[CreateAssetMenu]
public class LogMessage : ScriptableObject
{
    public enum crewMembers // Enumeration defining the names of all possible log message senders
    {
        Mavuto,
        Beckett,
        Rox,
        Oz,
        Ward,
        Enemy
    }
    public crewMembers crewMemberCode;
    //public string crewMemberName;
    [SerializeField]
    public string messageText;
    [SerializeField]
    public Sprite crewMemberSprite;
    [SerializeField]
    public float logMessageDelay;
}