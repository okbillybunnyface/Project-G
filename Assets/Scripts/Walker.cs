using UnityEngine;
using System.Collections;

public abstract class Walker/*, Texas Ranger*/ : Enemy {

	public float jumpThreshold = 15f;
	public float range = 10;
	protected Vector3 toTarget;

	// Use this for initialization
	public override void Start () 
	{
		base.Start();
	}

	public override void OnEnable()
	{
		base.OnEnable();
	}

	// Update is called once per frame
	public override void Update () 
	{
		base.Update();

	}

	public override void FixedUpdate ()
	{
		base.FixedUpdate ();
	}

	protected override void Seek(Vector3 target, float delay)
	{
		float angle = Vector3.Angle(transform.up, target);
		LayerMask platformLayer = 1 << 13;
		if(angle < 20)
		{
			Walk(target * 0f, delay);
			if(Physics.Raycast(transform.position, transform.up, jumpThreshold, platformLayer))
			{
				Jump(target);
			}
			//print("A");
		}
		else if(angle > 160 && Physics.Raycast(probeEnd.position, -transform.up, jumpThreshold))
		{
			Drop();
			//print("B");
		}
		else if(!DetectLedge(target))
		{
			Walk(target.normalized, delay);
			//print("C");
		}
		else if(target.sqrMagnitude < jumpThreshold * jumpThreshold)
		{
			Jump(target);
			//print("D");
		}
		else if(angle > 100 && Physics.Raycast(probeEnd.position, -transform.up, jumpThreshold))
		{
			Walk(target.normalized, delay);
			//print("E");
		}
		else
		{
			Walk(target * 0f, delay);
			//print("F");
		}
	}

	//Detects a ledge between you and your target.
	private bool DetectLedge(Vector3 target)
	{
		float dir = Vector3.Dot(target, transform.right);
		dir /= Mathf.Abs(dir);
		if(Physics.Raycast(this.transform.position + 2 * dir * this.transform.right, -transform.up, probeLength))
		{
			Debug.DrawRay(this.transform.position + 2 * dir * this.transform.right, -transform.up * probeLength, Color.green);
			return false;
		}
		else
		{
			Debug.DrawRay(this.transform.position + 2 * dir * this.transform.right, -transform.up * probeLength, Color.red);
			return true;
		}
	}

	IEnumerator Activated()
	{
		do
		{
			toTarget = player.transform.position - this.transform.position;
			Behaviours(toTarget);

			yield return null;
		}
		while(activeRoom);

		DeactivationBehaviours();
	}

	IEnumerator ImStuck()
	{
		yield return new WaitForSeconds(0f);
		transform.position = startPos;
	}

	protected override void ActivationBehaviours(Vector3 toPlayer)
	{
		StartCoroutine(Activated());
	}
}
