using UnityEngine;
using System.Collections;

public class SoundScript : MonoBehaviour {

	
	public AudioSource walkingSource, packSource, damageSource;
	public AudioClip pack, metalWalking, dirtWalking, gun, death;
	public PlayerScript playScript;
	static KeyCode jetpack = KeyCode.JoystickButton5, jump = KeyCode.JoystickButton0;
	public static bool mac;
	public bool walkingOnDirt;
	private GameObject player;
	private Vector3 directionL;
	private RaycastHit hit;
	private float triggers;

	//float soundPlaySpeed = 1f, soundPlaySpeedMultiplier = 10f;
	// Use this for initialization

	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void OnEnable()
	{
		ControlScript.controlCourier += GetInput;
	}

	void OnDisable()
	{
		ControlScript.controlCourier -= GetInput;
	}

	// Update is called once per frame
	void Update () 
	{
		playGPack ();
		playWalking();
		//playGDeath();
	}

	void playWalking()
	{
		if ( (Vector3.Dot(this.transform.right, directionL) > 0.1f || Vector3.Dot(this.transform.right, directionL) < -0.1f) && !walkingSource.isPlaying && playScript.jumping == false )
		{
			//			soundPlaySpeed = Input.GetAxisRaw("Horizontal");
			//			source.pitch = (Mathf.Abs(soundPlaySpeed)) * soundPlaySpeedMultiplier;
			//			Debug.Log("the pitch is " + source.pitch);

			//detecting surface type
			if (Physics.Raycast (this.transform.position, this.transform.up *-1, out hit, 10f) && hit.transform.tag == "Dirt")
			{
				walkingSource.clip = dirtWalking;
			}
			else
			{
				walkingSource.clip = metalWalking;
			}
	
			walkingSource.Play();

			return;
		}
		else if(Mathf.Abs(Vector3.Dot(this.transform.right, directionL)) < 0.1f)
		{
			walkingSource.Stop();
			return;
		}else{
		}
	}


	void GetInput(Vector3 directionL, Vector3 directionR, float triggers)
	{
		this.directionL = directionL;
		this.triggers = triggers;
	}

	void playGPack()
	{
		if(Input.GetKey(jetpack) == true && !packSource.isPlaying && playScript.jumping == true)
		{
			packSource.clip = pack;
			packSource.Play ();
			return;

		}else if(Input.GetKey(jetpack) == false || playScript.jumping == false)
		{
			packSource.Stop ();
			return;
		}else{
		}
	}

	public void playGDeath()
	{
			damageSource.clip = death;
			damageSource.Play();
	}

	public static void setSoundKeys(KeyCode jump1, KeyCode jetpack1)
	{
		SoundScript.jump = jump1;
		SoundScript.jetpack = jetpack1;
	}
}


