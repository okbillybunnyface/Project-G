using UnityEngine;
using System.Collections;

public class ConveyorScript : OscillationScript {
	
		
	// Update is called once per frame
	public override void FixedUpdate () 
	{
		//If it gets too far from positionA, it resets to positionA
		if((transform.position - positionA).sqrMagnitude > distance)
		{
			transform.position = positionA;
		}
		//Moves toward positionB
		transform.position += (positionB - positionA).normalized * velocity * Time.deltaTime;
	}
}
