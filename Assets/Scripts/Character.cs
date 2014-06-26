using UnityEngine;
using System.Collections;

public abstract class Character : Movement
{	
	//Instance variables

	public float energyRecharge = 10, energyMax = 100;
	public float healthRecharge = 1, healthMax = 100;//, currentHealth;
	public float health, energy;
	public ParticleSystem bloodParticles;
	protected Vector3 startPos;
	protected bool canAct = true;
	private Vector3 velocityRef = Vector3.zero;
	private Vector3 netGravity;
	protected GameObject room;
	protected RaycastHit rayHit;
	public AudioClip death;
	
	public override void Start()
	{
		base.Start();
		startPos = transform.position;
		netGravity = new Vector3(0, 0, 0);
	}
	
	public virtual void Update()
	{	
		if(energy < energyMax)
		{
			energy += Time.deltaTime * energyRecharge;
		} else
		{
			energy = energyMax;
		}
		
		if(health < healthMax)
		{
			health += Time.deltaTime * healthRecharge;
		} else
		{
			health = healthMax;
		}

		//Raycast to detect room bounds.
		LayerMask boundsMask = 1 << 15;
		if(Physics.Raycast(transform.position, -Vector3.forward, out rayHit, 40f, boundsMask))
		{
			room = rayHit.transform.gameObject;
		}
		else if(Physics.Raycast(transform.position, -Vector3.forward, out rayHit, 40f, boundsMask))
		{
			room = rayHit.transform.gameObject;
		}
		else Respawn();
		Debug.DrawRay(transform.position, -transform.forward * 30, Color.green);

		//Adds the background gravity
		if(rigidbody.useGravity)
		{
			netGravity += Physics.gravity;
		}
		
		netGravity.z = 0;
		netGravity.Normalize();
		netGravity *= -1;
		
		//Rotates the object based on net gravity
		//transform.up = Vector3.Lerp(transform.up, netGravity, Time.deltaTime * 2);
		//transform.up = netGravity;
		if(Vector3.Angle(transform.up, netGravity) > 179)
		{
			transform.RotateAround(transform.position, transform.forward, 1);
		}
		transform.up = Vector3.SmoothDamp(transform.up, netGravity, ref velocityRef, Time.deltaTime * 4); 


		//transform.RotateAround(transform.position, transform.forward,)
		
		netGravity = new Vector3(0, 0, 0);

		bloodParticles.emissionRate = (healthMax - health) / 4;
		bloodParticles.Play();
	}
	
	//Called whenever the gameObject is enabled.
	public override void OnEnable()
	{
		base.OnEnable();
		health = healthMax;
		energy = energyMax;
		gameObject.collider.enabled = true;
		gameObject.renderer.enabled = true;
		canAct = true;
	}

	//Removes health from the character.
	public virtual void Damage(float amount)
	{
		health -= amount;
		
		//Blooooooood
		bloodParticles.Emit((int) amount);
		
		if(health <= 0)
		{
			Die();
		}
	}
	
	public void IncrementForce(Vector3 force)
	{
		netGravity += force;
	}

	public abstract void Respawn();

	//Does whatever happens when this entity dies.
	public void Die()
	{
		AudioSource.PlayClipAtPoint(death,this.transform.position);
		for (int i = 0; i < this.transform.GetChildCount(); ++i)
		{
			this.transform.GetChild(i).gameObject.SetActive(false);
		}
		StartCoroutine(DeathBurst());
	}

	IEnumerator DeathBurst()
	{
		collider.enabled = false;
		renderer.enabled = false;
		bloodParticles.Emit(1000);
		yield return new WaitForSeconds(2);
		health = healthMax;
		energy = energyMax;
		collider.enabled = true;
		renderer.enabled = true;
		Respawn();
	}
}
