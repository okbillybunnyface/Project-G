using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour{

	public bool interact = false;
	public bool grounded = false;
	public Transform lineStart, lineEnd, groundedEnd;
	public GameObject enemyToKill;

	//		B, Y, X, A
	//PC	1, 2, 3, 4
	//MAC  	17, 19, 18, 16
	public KeyCode killCommand, jumpKey; 
	public float jumpForce;
	float jumpTime, jumpDelay = .5f;
	bool jumped;
	Animator anim;
	RaycastHit2D whatIHit;

	void Start()
	{
		anim = GetComponent<Animator>();
	}

	//stores the object the raycast hit
	//RaycastHit2D whatIHit;


	void Update()
	{
		Movement ();
		RayCast();
	}
	void RayCast()
	{
		//For going forward to enemy
		Debug.DrawLine (lineStart.position, lineEnd.position, Color.green);
		//This will start in the player
		Debug.DrawLine (this.transform.position, groundedEnd.position, Color.green);

		//grounded is true when the linecast contacts the ground
		grounded = Physics2D.Linecast(this.transform.position, groundedEnd.position, 1 << LayerMask.NameToLayer("Surface"));


		if(Physics2D.Linecast(lineStart.position, lineEnd.position, 1 << LayerMask.NameToLayer("Enemy")))
		{
			//Store what object was contacted by the raycast, otherwise it would knock out everything in the guard layer
			whatIHit = Physics2D.Linecast(lineStart.position, lineEnd.position, 1 << LayerMask.NameToLayer("Enemy"));
			interact = true;
		}
		else
		{
			interact = false;
		}

		//Key to kill the enemy
		if(Input.GetKeyDown(killCommand))// && interact == true)
		{
			anim.SetTrigger("attack");
			Debug.Log(whatIHit.collider.gameObject);
			//Destroy(whatIHit.collider.gameObject);
			Destroy(enemyToKill);
		}

		Physics2D.IgnoreLayerCollision(8, 10); //objects from layers 8 and 9 will ignore each others collisions

	}
	void Movement()
	{
		anim.SetFloat("speed", Mathf.Abs(Input.GetAxis("Horizontal")));
		//if (Input.GetKey (KeyCode.D))
		if (Input.GetAxisRaw ("Horizontal") > 0)   // direction of horizontal goes from -1 to 1 
		{
			transform.Translate (Vector2.right * 4f * Time.deltaTime);  //movement
			transform.eulerAngles = new Vector2(0,0);   //image rotation
		}
		//if (Input.GetKey (KeyCode.A))
		if (Input.GetAxisRaw ("Horizontal") < 0) 
		{
			transform.Translate (Vector2.right * 4f * Time.deltaTime);
			transform.eulerAngles = new Vector2(0,180);
		}

		if(Input.GetKeyDown (jumpKey)&& grounded == true)
		{
			rigidbody2D.AddForce(Vector2.up * jumpForce);
			jumpTime = jumpDelay;      //sets the jump time to .5 and it then counts down 
			anim.SetTrigger("Jump");
			jumped = true;
		}

		//counts down jump time
		jumpTime -= Time.deltaTime;
		//when grounded and time is up
		if (jumpTime <= 0 && grounded && jumped)
		{
			anim.SetTrigger("Land");
			jumped = false;
		}
	}
}
