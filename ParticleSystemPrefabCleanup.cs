/* Written by Cameron Williams
 * 
 * This short script simply destroys particle systems
 * after they have completed displaying their particles
 * so that the scene doesn't fill up with empty particle
 * systems at run time.
 * 
 * Last updated 8/22/2019.
 */

using UnityEngine;

public class ParticleSystemPrefabCleanup : MonoBehaviour
{
    void Start()
    {
        ParticleSystem particleSystem = this.GetComponent<ParticleSystem>();
        float totalDuration = particleSystem.main.duration;
        Destroy(this.gameObject, totalDuration);
    }
}
