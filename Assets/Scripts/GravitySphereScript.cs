using UnityEngine;
using System.Collections;

/*The maximum size of any gravity sphere is the size you make its collider in the scene (not its transform scale).
 It will always be a fraction of this size.*/
[RequireComponent(typeof(SphereCollider))]
public class GravitySphereScript : GravityScript 
{	
	//Instance variables
	public float dissipationRate = 0.5f;
	public float effectFactor = 0.2f;
	public bool isSticky = false;

	//Called once per frame
	public override void Update()
	{
		Debug.DrawRay(transform.position, transform.right * transform.localScale.x, Color.red);
		Debug.DrawRay(transform.position, -transform.right * transform.localScale.x, Color.red);
		Debug.DrawRay(transform.position, transform.up * transform.localScale.y, Color.red);
		Debug.DrawRay(transform.position, -transform.up * transform.localScale.y, Color.red);

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

		//Alters the gravity sphere's size based upon available energy.
		float scale = energy * anti / maxEnergy;
		this.transform.localScale = new Vector3(scale, scale, 1f);
	}

	void SetIsSticky(bool truthiness)
	{
		isSticky = truthiness;
	}

	public override Vector3 GetDirection(Collider satellite)
	{
		return this.transform.position - satellite.transform.position;
	}

	public override void ParticleUpdate()
	{
		float scale = energy / maxEnergy * anti;
		particles.startSpeed = 2 * scale * anti;
		particles.emissionRate = anti *  energy * effectFactor * effectFactor / 10;

		/*
		 * Using triggers for gravity affect broke this.
		if(energy >= 0)
		{
			transform.localScale = new Vector3(-scale, -scale, 1f);
		}
		else
		{
			transform.localScale = new Vector3(0.1f, 0.1f, 1f);
		}
		*/
	}
}
