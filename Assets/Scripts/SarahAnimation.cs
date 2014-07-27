using UnityEngine;
using System.Collections;

public class SarahAnimation : MonoBehaviour {


	Animator anim;
	public PlayerScript playScript;
	private Vector3 directionL;
	private bool facingRight,jumping;  //defining from Movement

	//SpriteRenderer sr = new SpriteRenderer();
	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		jumping = playScript.jumping;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Movement();
		jumping = playScript.jumping;
	}


	void Movement()
	{
	
		if(Vector3.Dot (transform.right, directionL) > 0.1f && jumping == false) 
		{
			facingRight = true;
			anim.SetTrigger("runRight");
		}
		else if(Vector3.Dot (transform.right, directionL) < -0.1f && jumping == false) 
		{
			facingRight = false;
			anim.SetTrigger("runLeft");

		}else if(jumping == true)
		{
			if (facingRight == false)
			{
				//transform.eulerAngles = new Vector2 (0, 180); //flips jumping sprite
				//anim.SetTrigger("sarahJumpL");
				return;
			}
			else// if (facingRight == true)
			{
				//transform.eulerAngles = new Vector2 (0,0);
				//anim.SetTrigger("sarahJump");
				return;
			}
				
		}
		else
		{
			anim.SetTrigger("idle");
		}

	}

	public void sarahIsUpgrading(bool upgrade)
	{
		if (upgrade)
		{
			anim.SetBool("sarahUpgrade", true);
		}
		else
		{
			anim.SetBool("sarahUpgrade", false);
		}
	}


	//Sets up the controller delegate
	void controlCourier(Vector3 directionL, Vector3 directionR, float triggers)
	{
		this.directionL = directionL;
	}

	void OnEnable()
	{
		ControlScript.controlCourier += controlCourier;
	}

	void OnDisable()
	{
		ControlScript.controlCourier -= controlCourier;
	}
}
