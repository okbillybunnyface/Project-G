using UnityEngine;
using System.Collections;

public abstract class Enemy : Character 
{
	public float collisionDamageThreshold = 20;
	public float collisionSplatFactor = 2;
	protected GameObject player, myRoom;
	protected bool activeRoom;

	// Use this for initialization
	public override void Start () 
	{
		base.Start();
		player = GameObject.FindGameObjectWithTag("Player");
		startPos = transform.position;

		LayerMask boundsMask = 1 << 15;
		Physics.Raycast(transform.position, -transform.forward, out rayHit, 40f, boundsMask);
		myRoom = rayHit.collider.gameObject;
	}
	
	// Update is called once per frame
	public override void Update () 
	{
		base.Update();
		LayerMask boundsMask = 1 << 15;
		Physics.Raycast(transform.position, -transform.forward, out rayHit, 40f, boundsMask);
		if(myRoom != rayHit.collider.gameObject)
		{
			transform.position = startPos;
		}
	}

	public override void OnEnable()
	{
		base.OnEnable();
		player = GameObject.FindGameObjectWithTag("Player");
		PlayerScript.PlayerRoom += PlayerRoom;
	}

	public virtual void OnDisable()
	{
		PlayerScript.PlayerRoom -= PlayerRoom;
	}

	public override void OnCollisionEnter(Collision collision)
	{
		base.OnCollisionEnter(collision);
		if(collision.gameObject != player && collision.relativeVelocity.sqrMagnitude > collisionDamageThreshold * collisionDamageThreshold)
		{
			Damage(collision.relativeVelocity.magnitude * collisionSplatFactor);
		}
	}

	private void PlayerRoom(GameObject room)
	{
		//Is my room the room the player is in?
		activeRoom = (room == myRoom);
		if(activeRoom)
		{
			ActivationBehaviours(player.transform.position - transform.position);
		}
	}

	protected abstract void Seek(Vector3 target, float delay);

	public override void Respawn()
	{
		transform.position = startPos;
		myRoom.SendMessage("Respawn", this.gameObject);
		this.gameObject.SetActive(false);
	}

	protected abstract void ActivationBehaviours(Vector3 toPlayer);

	protected abstract void DeactivationBehaviours();
	
	protected abstract void Behaviours(Vector3 toPlayer);
}
