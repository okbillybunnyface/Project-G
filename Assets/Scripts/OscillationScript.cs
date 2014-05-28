using UnityEngine;
using System.Collections;

public class OscillationScript : MonoBehaviour {
	
	public Vector3 positionA;
	public Vector3 positionB;
	public float velocity;
	protected float distance;
	
	// Use this for initialization
	void Start () {
		transform.position = positionA;
		distance = (positionA - positionB).sqrMagnitude;
	}
	
	// Update is called once per frame
	public virtual void FixedUpdate () 
	{
		if((transform.position - positionA).sqrMagnitude > distance || (transform.position - positionB).sqrMagnitude > distance)
		{
			velocity *= -1;
		}
		transform.position += (positionA - positionB).normalized * velocity * Time.deltaTime;
	}
}
