/* Written by Cameron Williams
 * 
 * The purpose of this class is to exist on a single game object in each scene
 * and generate invisible waypoints for the AI to follow in a gridlike pattern
 * above the terrain.
 * 
 * Note: This script uses ints where possible to save memory used by storing
 * the transforms of the waypoint objects which are typically floats.
 * 
 * This script was changed from using the Terrain.activeTerrain function to use the
 * Terrain.activeTerrains function so that scenes with multiple terrains can still
 * receive waypoints appropriately.
 * 
 * I added a custom editor script to allow the GenerateWaypoints() function to be
 * executed in the editor. This allows for fine tuning of waypoint placement in
 * the scene and cuts down on startup time since the waypoints aren't being
 * generated at the start of the scene.
 * 
 * Last updated on 8/21/2019
 */

using UnityEngine;

public class AIWaypointGenerator: MonoBehaviour
{
    [SerializeField] [Tooltip("The waypoint prefabs that the AI will navigate towards")]
    private GameObject waypointPrefab = null;

    [SerializeField] [Tooltip("The desired X and Z distance between waypoints; waypoints could end up further apart from Y terrain height")]
    private int distanceBetweenWaypoints = 10; // Just a default value; assign in Inspector!

    [SerializeField] [Tooltip("How high along the Y axis the waypoints will be placed with respect to the sampled terrain height")]
    private int heightOfWaypoints = 10; // Just a default value; assign in Inspector!

    public void GenerateWaypoints() // This function iterates through each of the active terrains in the scene and generates a grid of waypoints above them
    {
        Terrain[] activeTerrains = Terrain.activeTerrains; // works with multiple terrains for non-square shape
        for (int k = 0; k < activeTerrains.Length; k++) // iterate through the terrains in the scene
        {
            Terrain currentTerrain = activeTerrains[k];

            Vector3 terrainSize = currentTerrain.terrainData.size; // get the terrain dimensions in the form of a Vector3
            int terrainSizeX = (int)terrainSize.x;
            int terrainSizeZ = (int)terrainSize.z;

            int waypointGridRowLength = (terrainSizeX / distanceBetweenWaypoints); // Calculate size of waypoint grid via desired distance between waypoints and terrain size
            int waypointGridColumnLength = (terrainSizeZ / distanceBetweenWaypoints);

            Vector3 origin = currentTerrain.GetPosition(); // Get the transform of the terrain; usually will be (0, 0, 0) but flexibility is good :)
            int waypointCoordinateX = (int)origin.x;
            int waypointCoordinateY; // Will be found later based via SampleHeight function at X-Z location
            int waypointCoordinateZ = (int)origin.z;
            Vector3 heightSampleCoordinates;
            Vector3Int waypointCoordinates;

            for (int i = 0; i < waypointGridRowLength; i++)
            {
                for (int j = 0; j < waypointGridColumnLength; j++)
                {
                    heightSampleCoordinates = new Vector3(waypointCoordinateX, 0, waypointCoordinateZ);
                    waypointCoordinateY = (int)currentTerrain.SampleHeight(heightSampleCoordinates);
                    waypointCoordinates = new Vector3Int(waypointCoordinateX, (waypointCoordinateY + heightOfWaypoints), waypointCoordinateZ);
                    Instantiate(waypointPrefab, waypointCoordinates, Quaternion.identity, this.transform); // Create waypoint at calculated coordinates; rotation doesn't matter and the waypoint generator is the parent
                    waypointCoordinateZ += distanceBetweenWaypoints;
                }
                waypointCoordinateZ = (int)origin.z;
                waypointCoordinateX += distanceBetweenWaypoints;
            }
        }
    }
}
