/* Written by Cameron Williams
 * This script defines an interface that all enemy AI scripts will inherit
 * some basic methods and fields from.
 * 
 * Last updated 8/21/2019.
 */

public interface IEnemyAI
{
    int hitPoints { get; } // All enemies will have certain amount of HP
    float moveSpeed { get; } // All enemies will have a default movement speed
    bool pierceVulnerability { get; } // All enemies will have a set vulnerability to the various torpedoes that can change depending on the circumstances
    bool hEVulnerability { get; }
    bool eMPVulnerability { get; }
    bool disabled { get; } // Defines whether the enemy is disabled by an EMP

    void TakeDamage(int damage); // All enemies will take damage somehow
}
