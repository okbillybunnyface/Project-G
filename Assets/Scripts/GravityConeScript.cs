using UnityEngine;
using System.Collections;

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

		Collider[] satellites = GetSatellites();
		Vector3[] directions = GetDirections(satellites);

		energy -= energy * dissipationRate * Time.deltaTime;

		for(int s = 0; s < satellites.Length; s++)
		{
			ApplyGravity(satellites[s].gameObject, -directions[s] * anti);
			satellites[s].gameObject.SendMessage("Damage", 1.5 * energy * Time.deltaTime, SendMessageOptions.DontRequireReceiver);
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
		Collider[] tempA = Physics.OverlapSphere(transform.position, radius);
		Collider[] tempB = new Collider[tempA.Length];
		Vector3 toTarget;
		int count = 0;
		for(int s = 0; s < tempA.Length; s++)
		{
			toTarget = tempA[s].transform.position - this.transform.position;
			if(Vector3.Angle(transform.up, toTarget) < sweepAngle / 2)
			{
				tempB[count] = tempA[s];
				count++;
			}
		}
		Collider[] output = new Collider[count];
		for(int s = 0; s < count; s++)
		{
			output[s] = tempB[s];
		}

		return output;
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
