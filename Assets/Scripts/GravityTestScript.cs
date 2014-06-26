using UnityEngine;
using System.Collections;

public class GravityTestScript : MonoBehaviour {
	
	public float moveSpeed = 50;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody.velocity = new Vector3(moveSpeed * Input.GetAxis("Horizontal"), moveSpeed * Input.GetAxis("Vertical"), 0);
	}
}
