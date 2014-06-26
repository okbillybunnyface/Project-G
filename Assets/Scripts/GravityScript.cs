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
		if(Affect(satellite.gameObject) && Time.timeScale == 1)
		{
			ApplyGravity(satellite.gameObject);
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
	protected void ApplyGravity(GameObject satellite)
	{

		if(satellite.rigidbody != null)
		{
			//Get the amount of acceleration over the physics step
			Vector3 acceleration = GetAcceleration(satellite.transform.position);
			
			//Applies acceleration
			satellite.rigidbody.AddForce(acceleration, ForceMode.Acceleration);
			
			//Helps the object affected keep track of the net gravitational force it's experiencing
			satellite.SendMessage("IncrementForce", acceleration, SendMessageOptions.DontRequireReceiver);

			Debug.DrawLine(transform.position, satellite.transform.position, Color.magenta);
			Debug.DrawRay(satellite.transform.position, acceleration / 5, Color.cyan);
		}
	}

	//Returns the amount of acceleration for that position over the specified amount of time.
	public virtual Vector3 GetAcceleration(Vector3 position)
	{
		position = GetDirection(position);
		float forceMag = intensity * energy / position.sqrMagnitude;
		if(anti * forceMag > maxForce)
		{ 
			forceMag = maxForce * anti;
		}

		return position.normalized * forceMag;
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

	public abstract Vector3 GetDirection(Vector3 position);
}