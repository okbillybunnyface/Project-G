using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))] 
public class GravityFieldScript : GravityScript {

	public bool ignoreGlobalGravity = false;
	private Vector3 root;
	private BoxCollider box;

	void Start()
	{
		box = (BoxCollider)this.transform.collider;
		root = transform.position - transform.up * box.size.y / 2;
	}

	// Update is called once per frame
	public override void Update () 
	{
		Debug.DrawLine(transform.position, root, Color.white);
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		box = (BoxCollider)this.transform.collider;
		root = transform.position - transform.up * box.size.y / 2 * box.transform.localScale.y;
	}

	void OnTriggerEnter(Collider collision)
	{
		collision.gameObject.rigidbody.useGravity = false;
	}

	void OnTriggerExit(Collider collision)
	{
		collision.gameObject.rigidbody.useGravity = true;
	}

	void OnTriggerStay(Collider collision)
	{
		collision.gameObject.rigidbody.useGravity = false;
		Vector3 direction = (root - collision.transform.position);
		direction = Vector3.Dot(transform.up, direction) * transform.up;
		ApplyGravity(collision.gameObject, direction + direction.normalized * 5);
	}

	public override void ParticleUpdate()
	{
		particles.startLifetime = this.transform.lossyScale.y / 20;
	}

	public override Collider[] GetSatellites()
	{
		return null;
	}
	
	public override Vector3[] GetDirections(Collider[] satellites)
	{
		return null;
	}
}
