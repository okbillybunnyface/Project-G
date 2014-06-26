using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class GravityConeScript : GravityScript {

	//Angle of cone in degrees.
	public float sweepAngle = 30;
	//Radius of the cone.
	public float radius = 10;
	//How fast the cone dies off.
	public float dissipationRate = 0.5f;
	//Particle System on child transform responsible for charging effect
	public ParticleSystem chargingParticle;

	// Update is called once per frame
	public override void Update () 
	{
		base.Update();

		Debug.DrawRay(transform.position, transform.up * radius, Color.cyan);

		Vector3 drawDir = Quaternion.Euler(0, 0, sweepAngle / 2) * transform.up;
		Debug.DrawRay(transform.position, drawDir * radius, Color.red);

		drawDir = Quaternion.Euler(0, 0, -sweepAngle / 2) * transform.up;
		Debug.DrawRay(transform.position, drawDir * radius, Color.red);
	}

	public override void FixedUpdate()
	{
		base.FixedUpdate();

		energy -= energy * dissipationRate * Time.deltaTime;
	}

	public override void OnTriggerStay(Collider satellite)
	{
		if(Affect(satellite.gameObject))
		{
			ApplyGravity(satellite.gameObject);
			satellite.gameObject.SendMessage("Damage", 1.5 * energy * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
		}
	}

	public override Vector3 GetDirection(Vector3 position)
	{
		Vector3 direction = this.transform.position - position;
		return direction + direction.normalized * 0.5f;
	}

	public override bool Affect (GameObject satellite)
	{
		Vector3 toTarget = satellite.transform.position - this.transform.position;
		if(Vector3.Angle(transform.up, toTarget) < sweepAngle / 2)
		{
			return base.Affect(satellite);
		}

		return false;
	}

	public override void ParticleUpdate()
	{
		particles.Emit((int)(3 * energy * energy * Time.fixedDeltaTime));
	}

	public void ChargingEffect(float energy)
	{
		if(energy == -1)
		{
			chargingParticle.Clear();
		}
		else chargingParticle.Emit((int)(energy / 5));
	}
}
