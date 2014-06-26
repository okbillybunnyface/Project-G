using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour {
	
	
	public GameObject door;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider)
	{
		if(collider.tag == "Player")
		{
			door.layer = 16;
		}
	}
}
