using UnityEngine;
using System.Collections;

public class GravitySphereScript : GravityScript 
{	
	//Events
	
	//Instance variables
	public float radiusFactor = 1;
	public float dissipationRate = 0.5f;
	public float effectFactor = 0.2f;
	public bool isSticky = false;

	private float radius;

	//Called once per frame
	public override void Update()
	{
		Debug.DrawRay(transform.position, transform.right * radius, Color.red);
		Debug.DrawRay(transform.position, -transform.right * radius, Color.red);
		Debug.DrawRay(transform.position, transform.up * radius, Color.red);
		Debug.DrawRay(transform.position, -transform.up * radius, Color.red);

		ParticleUpdate();
	}

	//Physics updates
	public override void FixedUpdate () 
	{
		base.FixedUpdate();

		//Depletes the objects energy if it is not environmental or being charged
		if(!isEnvironment && !isCharging)
		{
			energy -= energy * dissipationRate * Time.deltaTime;
			//Makes the energy recharge if it's an environmental gravity source
		} else if(energy * anti < maxEnergy)
		{
			energy += anti * maxEnergy * dissipationRate * Time.deltaTime;
		}

		radius = radiusFactor * energy * anti;

		Collider[] satellites = GetSatellites();
		Vector3[] directions = GetDirections(satellites);
		for(int s = 0; s < satellites.Length; s++)
		{
			ApplyGravity(satellites[s].gameObject, directions[s]);
		}
	}
	
	//Called upon collision.
	void OnCollisionEnter(Collision collision)
	{
		//If the gravity ball has been fired
		if(!isCharging)
		{
			//If it's a sticky gravity ball, it disables its physics
			if(isSticky)
			{
				rigidbody.isKinematic = true;
			} else //otehrwise it bounces and loses half its energy
			{
				energy /= 2;
			}
		}
	}

	public override Vector3[] GetDirections(Collider[] satellites)
	{
		Vector3[] output = new Vector3[satellites.Length];
		for(int s = 0; s < satellites.Length; s++)
		{
			output[s] = this.transform.position - satellites[s].transform.position;
		}

		return output;
	}

	public override Collider[] GetSatellites()
	{
		return Physics.OverlapSphere(transform.position, radius);
	}

	void SetIsSticky(bool truthiness)
	{
		isSticky = truthiness;
	}

	public override void ParticleUpdate()
	{
		float scale = energy / maxEnergy * effectFactor * anti;
		particles.startSpeed = 2 * scale * anti;
		particles.emissionRate = anti *  energy * effectFactor * effectFactor / 10;

		if(energy >= 0)
		{
			transform.localScale = new Vector3(-scale, -scale, 1f);
		}
		else
		{
			transform.localScale = new Vector3(0.1f, 0.1f, 1f);
		}
	}
}
