using UnityEngine;
using System.Collections;

public class TangentialBump : MonoBehaviour {
	
	public GameObject center;
	public float force = 10;
	
	// Use this for initialization
	void Start () {
		Vector3 toCenter = center.transform.position - transform.position;
		
		Vector3 bump = new Vector3(-1 * toCenter.y, toCenter.x, 0);
		bump = bump.normalized;
		
		rigidbody.AddForce(bump * force / toCenter.magnitude, ForceMode.Impulse);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
