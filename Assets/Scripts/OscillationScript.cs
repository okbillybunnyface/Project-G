using UnityEngine;
using System.Collections;

public class OscillationScript : MonoBehaviour {
	
	public Vector3 positionA;
	public Vector3 positionB;
	public float velocity;
	protected float distance;
	public bool useMyStartPos = false, usePosMarker = false;
	public GameObject PosAMarker = null;

	
	// Use this for initialization
	void Start () {
		//transform.position = positionA;
		if (useMyStartPos) 
			positionA = transform.position;
		if(usePosMarker)
			positionA = PosAMarker.transform.position;

		distance = (positionA - positionB).sqrMagnitude;
	}
	
	// Update is called once per frame
	public virtual void FixedUpdate () 
	{
		if((transform.position - positionA).sqrMagnitude > distance || (transform.position - positionB).sqrMagnitude > distance)
		{
			//velocity *= -1;
			transform.position = positionA;
		}
		transform.position += (positionA - positionB).normalized * velocity * Time.deltaTime;
	}
}
