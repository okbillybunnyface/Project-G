using UnityEngine;
using System.Collections;


public abstract class GravityScript : MonoBehaviour {

	//Instance Variables
	public ParticleSystem particles;
	public float intensity = 2500;
	public float maxForce = 100;
	public float initialEnergy = 0;
	public static float maxEnergy = 100;
	public static float minEnergy = 1;
	public bool affectOther = false;
	public bool affectPlayer = false;
	public bool affectEnemies = false;
	public bool isEnvironment = false;
	protected bool isCharging = true;
	protected GameObject parent;
	protected float energy;
	protected int anti = 1;

	public virtual void OnEnable()
	{
		energy = initialEnergy;
		isCharging = true;
	}

	public virtual void OnDisable()
	{
		energy = 0;

		//Resets to default state.
		isCharging = true;
	}

	public virtual void Update()
	{
		ParticleUpdate();
	}

	//FixedUpdate is called at a fast, fixed rate (for physics)
	public virtual void FixedUpdate()
	{
		//anti allows certain things to not care about energy being negative
		if(energy < 0)
		{
			anti = -1;
		} else anti = 1;
		
		//Keeps energy from exceeding maximum while being charged.
		if(energy * anti > maxEnergy)
		{
			if(isCharging)
			{
				energy = maxEnergy * anti;
			}
		}

		//Deactivates the object if the energy is too close to zero, the object has been fired, and the object is not environmental
		if(energy * anti < minEnergy && !isCharging && !isEnvironment)
		{
			gameObject.SetActive(false);
		}
	}

	public virtual void OnTriggerStay(Collider satellite)
	{
		Vector3 direction = GetDirection(satellite);
		if(Affect(satellite.gameObject))
		{
			ApplyGravity(satellite.gameObject, direction + direction.normalized * 0.5f);
		}
	}

	//Checks to see whether the object is affected or not
	public virtual bool Affect(GameObject satellite)
	{
		if(satellite.gameObject.tag == "Enemy" && affectEnemies)
		{
			return true;
		}
		else if(satellite.gameObject.tag == "Player" && affectPlayer)
		{
			return true;
		}
		else if(affectOther)
		{
			return true;
		} 

		return false;
	}

	//Applies the proper gravitational force to the parameter satellite
	protected void ApplyGravity(GameObject satellite, Vector3 direction)
	{
		if(satellite.rigidbody != null)
		{
			float forceMag = 100 * intensity * energy  * Time.fixedDeltaTime / direction.sqrMagnitude;
			if(anti * forceMag > maxForce) forceMag = maxForce * anti;
			Vector3 force = direction.normalized * forceMag;
			
			//Applies acceleration
			satellite.rigidbody.AddForce(force, ForceMode.Acceleration);
			
			//Helps the object affected keep track of the net gravitational force it's experiencing
			satellite.SendMessage("IncrementForce", force, SendMessageOptions.DontRequireReceiver);

			Debug.DrawLine(transform.position, satellite.transform.position, Color.magenta);
			Debug.DrawRay(satellite.transform.position, force / 5, Color.cyan);
		}
	}

	//Gives the round energy
	public float Charge(float juice)
	{
		energy += juice;
		float extra = 0;
		if(energy > maxEnergy)
		{
			extra = energy - maxEnergy;
			energy = maxEnergy;
		}

		return extra;
	}
	
	void SetIsCharging(bool truthiness)
	{
		isCharging = truthiness;
	}
	void SetParent(GameObject parent)
	{
		this.parent = parent;
	}

	//Affect type of thing setters
	void SetAffectPlayer(bool truthiness)
	{
		affectPlayer = truthiness;
	}
	void SetAffectEnemies(bool truthiness)
	{
		affectEnemies = truthiness;
	}
	void SetAffectOther(bool truthiness)
	{
		affectOther = truthiness;
	}

	public abstract void ParticleUpdate();

	public abstract Vector3 GetDirection(Collider satellite);
}