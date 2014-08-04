
	using UnityEngine;
using System.Collections;

public class PlayerScript : Character
{
	//Events
	static KeyCode jump = KeyCode.JoystickButton0, jetPack = KeyCode.JoystickButton5, 
	stickyToggle = KeyCode.JoystickButton1, gravityShape = KeyCode.JoystickButton3,
	chargeJump = KeyCode.JoystickButton2, dash = KeyCode.Joystick1Button4;
	public delegate void MyRoom(GameObject myRoom);
	public static event MyRoom PlayerRoom;
	private GameObject spawnPoint;
	
	//Instance variables
	//public KeyCode moveUp, moveDown, moveLeft, moveRight, jumpKey;
	public bool useGravity = true;
	public ParticleSystem jetpackParticles;
	public float chargeRate = 5;
	public float jetPackForce = 10;
	public float jetPackDrain = 30;
	public float chargeJumpEnergy = 25;
	public float chargeJumpRate = 25;
	public float projectileOffset = 0;
	public float recoil = 10;
	public AudioClip gunCharge, gunFull, gunFire;
	public AudioSource gunSource;
	public GameObject gravityCone;
	public GameObject projectionBase;
    public LineRenderer projectionRenderer;
	public GravityConeScript gravityConeScript;
	private ProjectionBase projectionScript;
	private GameObject[] gravityClip = new GameObject[100];
	private int fireMode = 1;// 0: Sphere, 1: Cone
	private Vector3 directionL;
	private Vector3 directionR;
	private bool charging = false, isSticky = false, timeStop = false;
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
		projectionBase = (GameObject)GameObject.Instantiate(projectionBase);
		projectionScript = projectionBase.GetComponent<ProjectionBase>();
		projectionScript.Initialize(this.gameObject);
		//projectionBase.SetActive(false);
		rigidbody.useGravity = useGravity;
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
			//StartCoroutine(JumpCharger());
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

		if(Input.GetKeyDown(chargeJump) || Input.GetKeyDown(KeyCode.LeftControl))
		{
            if(!timeStop)
            {
                timeStop = true;
                StartCoroutine(JumpCharger());
            }
            else 
            {
                timeStop = false;
            }
		}

        if (Input.GetKeyDown(dash))
        {
            Dash(((Vector3.Dot(rigidbody.velocity, transform.right) > 0) ? transform.right : -transform.right), 10f);
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
		{						// Jump, Jetpack, sticky, GravityShape, chargeJump, dash
			PlayerScript.setKeys(KeyCode.JoystickButton16, KeyCode.JoystickButton14, KeyCode.JoystickButton19, KeyCode.JoystickButton18, KeyCode.JoystickButton13);
			SoundScript.setSoundKeys(KeyCode.JoystickButton16, KeyCode.JoystickButton14);
			ControlScript.mac = true;
			SoundScript.mac = true;
		}
		else
		{							// Jump, Jetpack, sticky, GravityShape, chargeJump, dash
			PlayerScript.setKeys(KeyCode.JoystickButton0, KeyCode.JoystickButton5, KeyCode.JoystickButton3, KeyCode.JoystickButton2, KeyCode.JoystickButton4);
			SoundScript.setSoundKeys(KeyCode.JoystickButton0, KeyCode.JoystickButton5);
			ControlScript.mac = false;
			ControlScript.mac = false;
		}
	}

	public static void setKeys(KeyCode jump1, KeyCode jetpack1, KeyCode gravityShape1, KeyCode chargeJump1, KeyCode dash1)
	{
		PlayerScript.jump = jump1;
		PlayerScript.jetPack = jetpack1;
		PlayerScript.gravityShape = gravityShape1;
		PlayerScript.chargeJump = chargeJump1;
		PlayerScript.dash = dash1;
		
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

    public void Dash(Vector3 direction, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, direction, out hit, distance, 9);

        float tempSpeed = jetpackParticles.startSpeed;
        float tempLife = jetpackParticles.startLifetime;
        jetpackParticles.startLifetime = 0.5f;
        jetpackParticles.startSpeed = 4f;
        jetpackParticles.Emit(200);
        transform.position = (hit.collider != null) ? hit.point : transform.position + direction * distance;
        jetpackParticles.Emit(200);
        jetpackParticles.startLifetime = tempLife;
        jetpackParticles.startSpeed = tempSpeed;
    }

	//Jump prediction charge thingy
	IEnumerator JumpCharger()
	{
		Vector3 force = Vector3.zero;//Force for jump
		Time.timeScale = 0.0000001f;//Pauses time without pausing time.
		Time.fixedDeltaTime = 0.01f * Time.timeScale;//Update fixedDeltatime based on timescale
		float time = Time.realtimeSinceStartup;//To keep track of time independently of the time freeze
		projectionBase.transform.position = transform.position;//Set the projection Base to be the current position, and activate it
		projectionBase.SetActive(true);
        projectionRenderer.enabled = true;
        float originalEnergy = energy;

		//While we have energy and the charge jump key is being held
		while(timeStop)
		{
            //Calculate what our velocity change will be after this update
            Vector3 tempForce = force + directionL * chargeJumpRate * (Time.realtimeSinceStartup - time);
            //If the force is greater than the max, reset it to the max
            if (tempForce.sqrMagnitude > jumpForce * jumpForce)
            {
                tempForce = tempForce.normalized * jumpForce;
            }
            //Calculate the cost
            float tempCost = tempForce.magnitude * chargeJumpEnergy;

            //If the cost is less than how much our current energy, do it
            if (tempCost < originalEnergy)
            {
                energy = originalEnergy - tempCost;
                force = tempForce;
            }

            //Set velocity change arrow based on how much we're changing it
            projectionRenderer.SetPosition(1, force / 2);

			//Inform the jump rope of our status
			projectionScript.Project(transform.position, force + rigidbody.velocity, useGravity);

			yield return null;//Stop execution until after next frame
		}

		//At this point, the user has stopped holding down the jump projection key

		//Turn the projection system off
		projectionBase.SetActive(false);
        projectionRenderer.enabled = false;
        //BAM. SUCH FORCE. WOW.
        rigidbody.AddForce(force, ForceMode.VelocityChange);

		//Reset the time stuff to normal
		Time.timeScale = 1;
		Time.fixedDeltaTime = 0.01f * Time.timeScale;
        timeStop = false;
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
