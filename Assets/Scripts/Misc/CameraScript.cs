using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	
	
	public GameObject player;
	public Camera mainCamera;
	public float maximumSize = 20;
	private GameObject playerRoom;
	//public GameObject PauseGUI;
	//public GUIText PauseGUI;
	public float lookFactor = 25;    //how far it looks
	public float lookTime = 0.25f;   //camera speed
	public Vector3 cameraDistance = new Vector3(0, 5, -25);
	private Vector3 direction = new Vector3(0, 0, 0);  //camera direction added from delegate
	private float maxY, minY, maxX, minX;//coordinate limits imposed by roombounds
	private float scale;
	private Vector3 currVelocity; //For the SmoothDamp in Update()
	public bool paused = false, quitMenu = false;
	public KeyCode pauseButton;
	public Rect pauseRect, quitRect, yesQuitRect, noQuitRect;
	public Texture2D pausedButton, yesButton, yesButton2, noButton, noButton2, quitButton, quitButton2;
	public GUISkin defaultSkin;

	
	// Use this for initialization
	void Start () 
	{
		Time.timeScale = 1;
		transform.parent = null;
	}

	void OnEnable()
	{
		ControlScript.controlCourier += GetInput;
		PlayerScript.PlayerRoom += SetPlayerRoom;
	}
	
	void OnDisable()
	{
		ControlScript.controlCourier -= GetInput;
		PlayerScript.PlayerRoom -= SetPlayerRoom;
	}

	// Update is called once per frame
	void Update()
	{
		Vector3 look = new Vector3(player.transform.position.x + cameraDistance.x + direction.x * lookFactor * mainCamera.aspect, player.transform.position.y + cameraDistance.y + direction.y * lookFactor, player.transform.position.z + cameraDistance.z);
		transform.position = Vector3.SmoothDamp(transform.position, look, ref currVelocity, lookTime);

		if(transform.position.x > maxX) transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
		if(transform.position.x < minX) transform.position = new Vector3(minX, transform.position.y, transform.position.z);
		if(transform.position.y > maxY) transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
		if(transform.position.y < minY) transform.position = new Vector3(transform.position.x, minY, transform.position.z);


		//-- This pauses the game
		if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(pauseButton) || (Input.GetKeyDown(KeyCode.JoystickButton9))) && paused == false)
		{
			//PauseGUI.SetActive(true);
			//PauseGUI.text = "Game Paused";
			paused = true;
			Screen.showCursor = true;
			Time.timeScale = 0;

		}
		else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(pauseButton) || (Input.GetKeyDown(KeyCode.JoystickButton9))) && paused == true)
		{
			//PauseGUI.SetActive(false);
			//PauseGUI.text = "";
			paused = false;
			Screen.showCursor = false;
			Time.timeScale = 1;
		}
	}

	#region onGui
	void OnGUI()
	{
		//playRect.Set(Screen.width * .6f, Screen.height * .5f, Screen.width *.25f, Screen.height * .1f);
		GUI.skin.button.normal.background = GUI.skin.GetStyle("Box").normal.background;
		GUI.skin.button.hover.background = GUI.skin.GetStyle("Box").normal.background;

		pauseRect.Set(Screen.width * .45f, Screen.height * .5f, Screen.width *.15f, Screen.height * .05f);
		quitRect.Set(Screen.width * .45f, Screen.height * .55f, Screen.width *.15f, Screen.height * .05f);
		yesQuitRect.Set(Screen.width * .30f, Screen.height * .55f, Screen.width *.15f, Screen.height * .05f);
		noQuitRect.Set(Screen.width * .60f, Screen.height * .55f, Screen.width *.15f, Screen.height * .05f);



		if (paused == true && quitMenu == false)
		{

			GUI.Button(pauseRect, "Paused");
			if(GUI.Button(quitRect, "Quit"))
			{
				quitMenu = true;
//				paused = false;
//				Application.LoadLevel(0);
			}
		}

		if (paused == true && quitMenu == true)
		{
			if(GUI.Button (yesQuitRect, "Yes"))
			{
				paused = false;
				Application.LoadLevel(0);
			}
			if(GUI.Button (noQuitRect, "No"))
			{
				quitMenu = false;
			}
		}
	}
	#endregion
	
	//Makes the camera move properly when the character is in crazy orientations
	void GetInput(Vector3 directionL, Vector3 directionR, float triggers)
	{
		this.direction = directionR;
	}

	void SetPlayerRoom(GameObject room)
	{
		this.playerRoom = room;
		BoxCollider roomCollider = (BoxCollider)playerRoom.collider;
		float roomX = roomCollider.size.x * roomCollider.transform.localScale.x;
		float roomY = roomCollider.size.y * roomCollider.transform.localScale.y;
		float camSize;
		if(roomX >= roomY * mainCamera.aspect)
		{
			camSize = roomY / 2;
		}
		else
		{
			camSize = roomX / mainCamera.aspect / 2;
		}
		if(camSize > maximumSize) camSize = maximumSize;

		mainCamera.orthographicSize = camSize;

		maxY = roomCollider.transform.position.y + roomY / 2 - camSize;
		//if(maxY < player.transform.position.y) maxY = player.transform.position.y;
		minY = roomCollider.transform.position.y - roomY / 2 + camSize;
		//if(minY > player.transform.position.y) minY = player.transform.position.y;

		maxX = roomCollider.transform.position.x + roomX / 2 - camSize * mainCamera.aspect;
		//if(maxX < player.transform.position.x) maxX = player.transform.position.x;
		minX = roomCollider.transform.position.x - roomX / 2 + camSize * mainCamera.aspect;
		//if(minX > player.transform.position.x) minX = player.transform.position.x;
	}


}