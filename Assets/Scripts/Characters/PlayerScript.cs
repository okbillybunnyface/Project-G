using UnityEngine;
using System.Collections;

public class PlayerScript : Character
{
	//Events
	static KeyCode jump = KeyCode.JoystickButton0, jetPack = KeyCode.JoystickButton5, 
	stickyToggle = KeyCode.JoystickButton1, gravityShape = KeyCode.JoystickButton3;
	public delegate void MyRoom(GameObject myRoom);
	public static event MyRoom PlayerRoom;
	private GameObject spawnPoint;
	
	//Instance variables
	//public KeyCode moveUp, moveDown, moveLeft, moveRight, jumpKey;
	public ParticleSystem jetpackParticles;
	public float chargeRate = 5;
	public float jetPackForce = 10;
	public float jetPackDrain = 30;
	public float projectileOffset = 0;
	public float recoil = 10;
	public AudioClip gunCharge, gunFull, gunFire;
	public AudioSource gunSource;
	public GameObject gravityCone;
	public GravityConeScript gravityConeScript;
	private GameObject[] gravityClip = new GameObject[100];
	private int fireMode = 1;// 0: Sphere, 1: Cone
	private Vector3 directionL;
	private Vector3 directionR;
	private bool charging = false, isSticky = false;
	private int clipIter = 0;
	private float triggers = 0;
	private GameObject prevRoom, reticle;
	public bool mac = false;
	public Texture2D healthBarGreen, healthBarRed;
	
	//Animator anim;
	
	
	// Use this for initialization
	public override void Start ()
	{
		base.Start();
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		Screen.showCursor = false;
	}
	
	//Called whenever the gameObject is enabled.
	public override void OnEnable ()
	{
		base.OnEnable();
		
		ControlScript.controlCourier += GetInput;
		spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
	}
	
	void OnDisable()
	{
		
		ControlScript.controlCourier -= GetInput;
	}
	
	// Update is called once per frame
	public override void Update () 
	{
		base.Update();
		
		//Checking for debug mode
		if(Debug.isDebugBuild)
		{
			if (Input.GetKey(KeyCode.K))
			{
				Respawn();
			}
			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				mac = !mac;
				changeKeysMethod();
				Debug.Log("Control System Changed");
			}
		}
		
		if(prevRoom != room)
		{
			prevRoom = room;
			if(PlayerRoom != null)
			{
				PlayerRoom(room);
			}
		}
		
		//print (triggers);
		
		if(moveSpeed > 0 && canAct)
		{
			//audio.Play ();
		}
		
		if(Input.GetKeyDown(stickyToggle))
		{
			if(isSticky)
			{
				isSticky = false;
			} else isSticky = true;
			
			if(isSticky)
			{
				print("Sticky Mode activated.");
			}
			else print("Sticky Mode deactivated.");
		}
		
		if(Input.GetKeyDown(jump) || Input.GetKeyDown(KeyCode.Space))
		{
			if(Vector3.Dot(directionL, transform.up) < -0.7)
			{
				Drop();
			}
			else
			{
				if(directionL.sqrMagnitude < 0.02)
				{
					Jump(transform.up);
				}
				else Jump(directionL);
			}
		}
		
		if(Input.GetKeyDown(gravityShape))
		{
			fireMode = 1;
			//fireMode = (fireMode + 1) % 2;
			if(fireMode == 0)
			{
				Debug.Log("Gravity Sphere selected.");
			}
			else if(fireMode == 1)
			{
				Debug.Log("Gravity Cone selected.");
			}
			else Debug.Log("Gravity Mode Selection Error!");
		}
		
	}
	
	//Physics
	public override void FixedUpdate()
	{
		base.FixedUpdate();
		
		if(directionL.magnitude > 1) directionL.Normalize();
		
		Walk(directionL, Time.deltaTime);
		
		//Jetpack
		if(canAct && (Input.GetKey(jetPack) || Input.GetKey(KeyCode.LeftShift)))
		{
			if(energy >= jetPackDrain)
			{
				//Magnitude = absolute value.
				energy -= jetPackDrain * Time.deltaTime * new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0).magnitude;
				rigidbody.AddForce(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * jetPackForce * 10 * Time.deltaTime, ForceMode.Acceleration);
				
				
				//Jetpack effect
				
				jetpackParticles.enableEmission = true;
				
			}
		}
		else
		{
			jetpackParticles.enableEmission = false;
		}
		
		
		if((triggers != 0 || Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !charging)
		{
			if(fireMode == 0)
			{
				StartCoroutine(SphereCharger());
			}
			else if(fireMode == 1)
			{
				StartCoroutine(ConeCharger());
			}
		}
	}
	
	//Draws the current HP bar and Energy bar
	void OnGUI()
	{
		//Draws HUD image under HP / En bars
		//setting skins to default
		GUI.skin.button.normal.background = GUI.skin.GetStyle("Box").normal.background;
		GUI.skin.button.hover.background = GUI.skin.GetStyle("Box").normal.background;
		
		if ( this.health < (this.healthMax/2))
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width/2, Screen.height/4), healthBarRed, ScaleMode.ScaleToFit, true, 0f);
		}
		else
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width/2, Screen.height/4), healthBarGreen, ScaleMode.ScaleToFit, true, 0f);
		}	
		GUI.color = Color.red;
		GUI.Button(new Rect(300, 20, Screen.width/4/(healthMax/health),20), "HP: " + (int)health + "/" + healthMax);
		GUI.color = Color.yellow;
		GUI.Button(new Rect(300, 40, Screen.width/4/(energyMax/energy),20), "EN: " + (int)energy + "/" + (int)energyMax);
		
	}
	
	public void changeKeysMethod()
	{
		if(mac)
		{
			PlayerScript.setKeys(KeyCode.JoystickButton16, KeyCode.JoystickButton14, KeyCode.JoystickButton19);
			SoundScript.setSoundKeys(KeyCode.JoystickButton16, KeyCode.JoystickButton14);
			ControlScript.mac = true;
			SoundScript.mac = true;
		}
		else
		{
			PlayerScript.setKeys(KeyCode.JoystickButton0, KeyCode.JoystickButton5, KeyCode.JoystickButton3);
			SoundScript.setSoundKeys(KeyCode.JoystickButton0, KeyCode.JoystickButton5);
			ControlScript.mac = false;
			ControlScript.mac = false;
		}
	}
	public static void setKeys(KeyCode jump1, KeyCode jetpack1, KeyCode gravityShape1)
	{
		PlayerScript.jump = jump1;
		PlayerScript.jetPack = jetpack1;
		PlayerScript.gravityShape = gravityShape1;
		
		Debug.Log("buttons set");
	}
	
	public void GetInput(Vector3 directionL, Vector3 directionR, float triggers)
	{
		this.directionL = directionL;
		this.directionR = directionR;
		this.triggers = triggers;
	}
	
	public override void Respawn()
	{
		if(spawnPoint != null)
		{
			transform.position = spawnPoint.transform.position;
		}
		else
		{
			transform.position = startPos;
		}
		
		StartCoroutine(respawnChildren());
		
	}
	
	public void EnergyOverflow(float energy)
	{
		//chargeFull = true;
	}
	
	IEnumerator SphereCharger()
	{
		bool full = false;
		float sphereEnergy = 0;
		charging = true;
		//iterate to next ball in clip
		clipIter = (clipIter + 1) % gravityClip.Length;
		//if it doesn't exist, create a new one
		if(gravityClip[clipIter] == null)
		{
			gravityClip[clipIter] = (GameObject)GameObject.Instantiate(Resources.Load("GravitySphere", typeof(GameObject)));
		}
		//set it to be active, and have the proper values
		gravityClip[clipIter].SetActive(true);
		gravityClip[clipIter].SendMessage("SetAffectPlayer", false);
		gravityClip[clipIter].SendMessage("SetAffectEnemies", true);
		gravityClip[clipIter].SendMessage("SetAffectOther", true);
		
		//set the gravity ball to be the right kind of sticky
		gravityClip[clipIter].SendMessage("SetIsSticky", isSticky);
		
		
		float pitch = gunSource.pitch;
		gunSource.clip = gunCharge;
		gunSource.ignoreListenerVolume = true;
		gunSource.volume = 0.1f;
		gunSource.loop = true;
		gunSource.Play();
		
		do
		{
			//set it's position to the right place
			gravityClip[clipIter].transform.position = transform.position + directionR.normalized * (1 + projectileOffset);
			//give teh gravity ball some energy
			float amount = chargeRate * Time.deltaTime;
			if(amount <= energy)
			{
				sphereEnergy += amount;
				energy -= amount;
				if(triggers < 0 || Input.GetMouseButton(1)) amount *= -1;
				gravityClip[clipIter].SendMessage("Charge", amount);
			}
			
			if(sphereEnergy > GravityScript.maxEnergy)
			{
				energy += sphereEnergy - GravityScript.maxEnergy;
				sphereEnergy = GravityScript.maxEnergy;
				if(!full)
				{
					full = true;
					gunSource.Stop();
					gunSource.clip = gunFull;
					gunSource.volume = 0.8f;
					gunSource.pitch = pitch;
					gunSource.Play();
				}
			}
			else if(!full)
			{
				gunSource.pitch = pitch + pitch * sphereEnergy/GravityScript.maxEnergy;
			}
			
			yield return null;
			
			if(triggers == 0 && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
			{
				charging = false;
			}
		}
		while(charging);
		
		//tell the gravity ball it's not being charged anymore
		gravityClip[clipIter].SendMessage("SetIsCharging", false);
		gravityClip[clipIter].SendMessage("SetAffectPlayer", true);
		gravityClip[clipIter].SendMessage("Fire", directionR);
		
		gunSource.Stop();
		gunSource.pitch = pitch;
		gunSource.PlayOneShot(gunFire, 1.0f);
	}
	
	IEnumerator ConeCharger()
	{
		float coneEnergy = 0;
		charging = true;
		bool full = false;
		
		
		float pitch = gunSource.pitch;
		gunSource.clip = gunCharge;
		gunSource.ignoreListenerVolume = true;
		gunSource.volume = 0.1f;
		gunSource.loop = true;
		gunSource.Play();
		
		gravityCone.SetActive(false);
		gravityCone.SetActive(true);
		gravityConeScript.affectPlayer = false;
		gravityConeScript.affectEnemies =  true;
		gravityConeScript.affectOther =  true;
		
		do
		{
			//Moves the gravity cone so that it follows the player and faces the right direction
			gravityCone.transform.position = transform.position + directionR.normalized * (1 + projectileOffset);
			gravityCone.transform.up = directionR.normalized;
			
			if(energy >= chargeRate * Time.deltaTime)
			{
				energy -= chargeRate * Time.deltaTime;
				coneEnergy += chargeRate * Time.deltaTime;
			}
			
			if(triggers == 0 && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
			{
				charging = false;
			}
			
			if(coneEnergy > GravityScript.maxEnergy)
			{
				energy += coneEnergy - GravityScript.maxEnergy;
				coneEnergy = GravityScript.maxEnergy;
				if(!full)
				{
					full = true;
					gunSource.Stop();
					gunSource.clip = gunFull;
					gunSource.volume = 0.8f;
					gunSource.pitch = pitch;
					gunSource.Play();
				}
			}
			else if(!full)
			{
				gunSource.pitch = pitch + pitch * coneEnergy/GravityScript.maxEnergy;
			}
			
			gravityConeScript.ChargingEffect(coneEnergy);
			
			yield return null;
		} 
		while(charging);
		
		gunSource.Stop();
		gunSource.pitch = pitch;
		gunSource.PlayOneShot(gunFire, 1.0f);
		
		gravityConeScript.ChargingEffect(-1);
		gravityConeScript.Charge(coneEnergy);
	}
	
	IEnumerator respawnChildren()
	{
		yield return new WaitForSeconds(2);
		for (int i = 0; i < this.transform.GetChildCount(); ++i)
		{
			this.transform.GetChild(i).gameObject.SetActive(true);
		}	
	}
}