using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
	
	//Instance variables
	public float velocity = 30;
	public float damage = 10;
	public bool dissipateOnCollision = false;
	public bool maintainVelocity = false;
	
	// Update is called once per frame
	void Update()
	{
		if(maintainVelocity)
		{
			rigidbody.velocity = rigidbody.velocity.normalized * velocity;
		}
	}
	
	//Called upon collision.
	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemy")
		{
			collision.gameObject.SendMessage("damage", damage);
		}
		
		if(dissipateOnCollision)
		{
			gameObject.SetActive(false);
		}
	}
	
	//Fires the projectile in "direction" at "initialVelocity".
	void Fire(Vector3 direction)
	{
		rigidbody.velocity = direction.normalized * velocity;
	}
	
	void SetDissipateOnCollision(bool truthiness)
	{
		dissipateOnCollision = truthiness;
	}
}
