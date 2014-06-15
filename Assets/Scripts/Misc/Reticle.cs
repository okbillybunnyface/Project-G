using UnityEngine;
using System.Collections;

public class Reticle : MonoBehaviour {
	
	public GameObject player;
	public float distance = 10;
	public float smoothiness = 10;
	private Vector3 offset;
	private Vector3 direction = new Vector3(0, 0, 0);
	
	// Use this for initialization
	void Start () 
	{
		offset = new Vector3(distance, 0, 0);
		transform.position = player.transform.position + offset;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = Vector3.Lerp(transform.position, player.transform.position + new Vector3(direction.x * distance, direction.y * distance, 0), Time.deltaTime * smoothiness);
		transform.up = transform.position - player.transform.position;

		//Makes the reticle invisible if it's right on top of the player.
		this.renderer.enabled = ((transform.position - player.transform.position).magnitude >= 0.02);

	}
	
	void OnEnable()
	{
		ControlScript.controlCourier += GetInput;
	}
	
	void OnDisable()
	{
		ControlScript.controlCourier -= GetInput;
	}
	
	void GetInput(Vector3 directionL, Vector3 directionR, float triggers)
	{
		this.direction = directionR;
	}
}
