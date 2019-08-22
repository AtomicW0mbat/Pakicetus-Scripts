/* Written by Cameron Williams
 * This script defines an interface that will form the structure of all of the other
 * management related scripts.
 * 
 * This description was last updated 6/24/2019.
 */

public interface IGameManager
{
    ManagerStatus status { get; } // all managers must have a "getter" for their respective statuses

    void Startup(); // all managers must have a Startup method
}
