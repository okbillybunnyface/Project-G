using UnityEngine;
using System.Collections;

public class TeleportMe : MonoBehaviour {

	public Transform destination;
	private GameObject player;

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject == player)
		{
			player.transform.position = destination.position;
		}
	}
}
