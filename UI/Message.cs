/* Written by Cameron Williams
 * 
 * While the LogMessage scriptable object is used to store
 * information about each log message for later use when the
 * game is running, this Message class is used to hold the
 * information of each message at run time. As such, it's
 * structure is very similar, containing the name of the 
 * message sender, the message itself, and an image for
 * the person sending the message. You can see the usage of
 * this class in the SceneManager.cs script in the
 * GenerateMessage method.
 * 
 * Last updated 8/22/2019.
 */

using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    [SerializeField]
    public Text messageName;
    [SerializeField]
    public Text messageText;
    [SerializeField]
    public Image crewMemberImage;
}
