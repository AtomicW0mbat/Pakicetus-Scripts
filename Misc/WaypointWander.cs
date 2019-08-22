using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class WaypointWander : MonoBehaviour {
    private Rigidbody character;
	private float giveUpTime = 15.0f;
	private float time = 0f;
	private float speed = 0.5f;
    private float rotationDamping = 0.01f;
    private float closeEnough = 0.3f;
    private float distanceFromTarget;
    private int waypointIndex = 0;
    private Vector3 waypoint;
    public Transform[] waypoints;

    void Awake() {
        character = GetComponent<Rigidbody>();
		waypointIndex = Random.Range(0, waypoints.Length);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		character.velocity = transform.forward * speed;
		Quaternion r = Quaternion.LookRotation(waypoint - transform.position, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, r, rotationDamping);

		float d = Vector3.Distance(transform.position, waypoint);
		if (d < closeEnough || Time.time > time) {
			time += giveUpTime;
			//waypoint = waypoints[Random.Range(0, waypoints.Length)].position;
			if (waypointIndex + 1 < waypoints.Length)
				waypointIndex++;
			else
				waypointIndex = 0;
			waypoint = waypoints[waypointIndex].position;
		}
		//Debug.Log(waypointIndex);
		//Debug.DrawLine(transform.position, waypoint, Color.blue, 0.1f);
	}
}