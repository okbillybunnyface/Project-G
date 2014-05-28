using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public static float maxSpeed = 40;
	public float moveSpeed = 5, moveAccel = 50, jumpForce = 2, jumpingAccel = 5;
	protected float probeLength, groundAngle;
	public bool jumping = false;
	private bool grounded;
	private Vector3 groundTangent, normal;
	private GameObject ground, collided;
	protected int originalLayer;
	public Transform probeEnd;

	public virtual void Start()
	{
		originalLayer = gameObject.layer;
		//jumping = false;
	}

	public virtual void OnEnable()
	{
		probeLength = (this.transform.position - probeEnd.position).magnitude;
		RaycastHit tether = new RaycastHit();
		LayerMask groundedMask = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11;
		if(Physics.Raycast(this.transform.position, this.transform.up * -1, out tether, 10000, ~groundedMask))
		{
			ground = tether.collider.gameObject;
		}
	}

	//Physics update
	public virtual void FixedUpdate()
	{
		RaycastHit tether = new RaycastHit();
		//Draws a line representing the grounding raycast. This will start in the player.

		LayerMask groundedMask = 1 << 8 | 1 << 9 | 1 << 10 | 1 << 11;
		grounded = Physics.Raycast(this.transform.position, this.transform.up * -1, out tether, probeLength, ~groundedMask);
		if(grounded)
		{
			normal = tether.normal;
			ground = tether.collider.gameObject;
			groundAngle = Vector3.Angle(normal, transform.up);
			groundTangent.x = normal.y;
			groundTangent.y = -normal.x;
			//Draws a line representing the normal to the surface representing ground. Starts in the player.
			Debug.DrawLine(this.transform.position, probeEnd.position, Color.green);
			Debug.DrawRay(this.transform.position, normal, Color.yellow);
			Debug.DrawRay(this.transform.position, groundTangent, Color.yellow);
		} 
		else if(!jumping)
		{
			JumpPrep();
			StartCoroutine(Jumping(transform.up, 0f));
			Debug.DrawLine(this.transform.position, probeEnd.position, Color.red);
		}
		else Debug.DrawLine(this.transform.position, probeEnd.position, Color.red);

		if(!jumping && Vector3.Angle(normal, transform.up) > 45)
		{
			Drop();
		}

		if(rigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed)
		{
			Vector3 force = Time.deltaTime * moveAccel * -rigidbody.velocity.normalized * (rigidbody.velocity.sqrMagnitude - maxSpeed * maxSpeed);
			rigidbody.AddForce(force, ForceMode.Acceleration);
		}
	}

	//called upon collision with an object
	public virtual void OnCollisionEnter(Collision collision)
	{
		collided = collision.gameObject;
		if(collided == ground)
		{
			jumping = false;
		}
	}

	//Makes the character jump relative to the characters current rotation.
	public void Jump(Vector3 direction)
	{
		if(!jumping)
		{
			JumpPrep();
			gameObject.layer = originalLayer + 1;
			StartCoroutine(Jumping(direction, jumpForce));
		}
	}

	public void Drop()
	{
		if(ground != null)
		{
			if(!jumping && ground.layer == 13)
			{
				JumpPrep();
				gameObject.layer = originalLayer + 1;
				StartCoroutine(Jumping(transform.up, 0f));
			}
		}
	}

	//Makes the character translate left and right relative to the characters current rotation.
	public void Walk(Vector3 input, float delay)
	{
		if(input.sqrMagnitude > 1)
		{
			input.Normalize();
		}

		else if(!jumping)
		{
			//groundAngle is the angle between the normal vector to ground and the transform.up vector
			float maxVelocity = Mathf.Cos(groundAngle * Mathf.PI / 180) * moveSpeed;
			//groundTagent is the vector parallel to the ground
			Vector3 force = 100 * delay * groundTangent * moveAccel * (Vector3.Dot(input, groundTangent) - Vector3.Dot(groundTangent, rigidbody.velocity) / maxVelocity);
			rigidbody.AddForce(force, ForceMode.Acceleration);

			Debug.DrawRay(this.transform.position, force / 4, Color.blue);
		} 
		else
		{
			Vector3 force = 100 * delay * transform.right * jumpingAccel * Vector3.Dot(input, transform.right);
			if(rigidbody.velocity.sqrMagnitude < moveSpeed * moveSpeed || Vector3.Dot(rigidbody.velocity, force) < 0)
			{
				rigidbody.AddForce(force, ForceMode.Acceleration);

				Debug.DrawRay(this.transform.position, force / 4, Color.blue);
			}
		}
	}

	//
	protected void JumpPrep()
	{
		collided = null;
		jumping = true;
		StartCoroutine(PlatformChecker());
	}


	protected IEnumerator Jumping(Vector3 direction, float force)
	{
		if(direction.sqrMagnitude > 1)
		{
			direction.Normalize();
		}

		float angle = Vector3.Angle(direction, transform.up);
		if(angle > 180)
		{
			direction = Vector3.RotateTowards(direction, transform.up, (angle - 30) * Mathf.PI / 180, 0.0f);
		}
		else
		{
			direction = Vector3.RotateTowards(direction, transform.up, (5 * angle / 6) * Mathf.PI / 180, 0.0f);
		}
		rigidbody.AddForce(direction * force, ForceMode.Impulse);

		while(jumping)
		{
			if(collided == ground)
			{
				jumping = false;
			}
			yield return new WaitForFixedUpdate();
		}
	}

	//Checks to see when the character can return to the original layer.
	IEnumerator PlatformChecker()
	{
		//Waits until the character isn't grounded, unless the character stops jumping.
		while(grounded)
		{
			if(!jumping)
			{
				break;
			}
			yield return new WaitForFixedUpdate();
		}
		//Then, waits until the character is grounded, unless the character stops jumping.
		while(!grounded)
		{
			if(!jumping)
			{
				break;
			}
			yield return new WaitForFixedUpdate();
		}
		//Finally, returns the character to the original layer.
		gameObject.layer = originalLayer;
	}
}

