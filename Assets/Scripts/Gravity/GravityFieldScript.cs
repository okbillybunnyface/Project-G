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
		root = transform.position - transform.up * box.size.y / 2 * box.transform.localScale.y;
	}

	// Update is called once per frame
	public override void Update () 
	{
		Debug.DrawLine(transform.position, root, Color.white);
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();
		/*
		 * I'm not sure why I put this code here, so I'm commenting it out for now. Uncomment if shenanigans.
		box = (BoxCollider)this.transform.collider;
		root = transform.position - transform.up * box.size.y / 2 * box.transform.localScale.y;
		*/
	}

	void OnTriggerEnter(Collider collision)
	{
		collision.gameObject.rigidbody.useGravity = false;
	}

	void OnTriggerExit(Collider collision)
	{
		collision.gameObject.rigidbody.useGravity = true;
	}

	public override void OnTriggerStay(Collider collision)
	{
		collision.gameObject.rigidbody.useGravity = false;
		base.OnTriggerStay(collision);
	}

	public override void ParticleUpdate()
	{
		particles.startLifetime = this.transform.lossyScale.y / 20;
	}

	public override Vector3 GetDirection(Collider satellite)
	{
		Vector3 direction = (root - satellite.transform.position);
		direction = Vector3.Dot(transform.up, direction) * transform.up;
		return direction;
	}
}
