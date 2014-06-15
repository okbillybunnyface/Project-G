using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour {
	
	private Transform spawnPoint;

	void Start()
	{
		//player = GameObject.FindWithTag("Player");
		spawnPoint = GameObject.FindWithTag("SpawnPoint").transform;
	}
	void OnTriggerEnter(Collider collider) {
		if (collider.tag == "Player")
		{
			spawnPoint.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
			Debug.Log("Checkpoint Updated");
		}
	}
}
