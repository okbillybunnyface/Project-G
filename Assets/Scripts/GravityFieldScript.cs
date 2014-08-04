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

	public override void OnTriggerEnter(Collider satellite)
	{
        base.OnTriggerEnter(satellite);
		satellite.gameObject.rigidbody.useGravity = false;
	}

	public override void OnTriggerExit(Collider satellite)
	{
        base.OnTriggerExit(satellite);
		satellite.gameObject.rigidbody.useGravity = true;
	}

	public void OnTriggerStay(Collider collision)
	{
		collision.gameObject.rigidbody.useGravity = false;
	}

	public override Vector3 GetAcceleration (Vector3 position, float time)
	{
		return base.GetAcceleration (position, time);
	}

	public override void ParticleUpdate()
	{
		gravParticles.startLifetime = this.transform.lossyScale.y / 20;
	}

	public override Vector3 GetDirection(Vector3 position)
	{
		Vector3 direction = (root - position);
		direction = Vector3.Dot(transform.up, direction) * transform.up;
		return direction + direction.normalized * 2;
	}
}
