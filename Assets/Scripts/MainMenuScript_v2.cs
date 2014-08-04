using UnityEngine;
using System.Collections;

public class MainMenuScript_v2 : MonoBehaviour {

	//public GameObject go;
	public AudioClip select, verify, back;
	public Rect playRect, optionsRect, quitRect, macControls, pcControls, backButton;// = new Rect();//(Screen.width * .6f, Screen.height * .5f, Screen.width *.25f, Screen.height * .1f);
	public Rect sentRect;
	public bool isPlaying = false, lightOn = false;
	public bool optionsMenu = false;
	bool buttonClicked1 = false, buttonClicked2 = false,buttonClicked3 = false;
	int count = 0;
	public GameObject galaxy;
	private KeyCode selectMenuOption = KeyCode.JoystickButton16;
	public Texture2D lightOnG, lightOffG, starsBackground, playButtonWhite, playButtonBlack, options1, options2, quit, quit2, macCon, macCon2, PCCon, PCCon2,
	mainMenu, mainMenu2, defaultSkin;


	// Use this for initialization
	void Start () 
	{
		StartCoroutine(lightFlicker());
		//lightOnG.SetActive(lightOn);
		Time.timeScale = 1;
		galaxy.animation["blackHoleRotate"].speed = -1;

	}
	
	// Update is called once per frame
	void Update () 
	{
		buttonClicked();
	}
	void buttonClicked()
	{
			switch(count)
			{
			case 0:
				buttonClicked1 = true;
				buttonClicked2 = false;
				buttonClicked3 = false;
				break;
			case 1:
				buttonClicked1 = false;
				buttonClicked2 = true;
				buttonClicked3 = false;
				break;
			case 2:
				buttonClicked1 = false;
				buttonClicked2 = false;
				buttonClicked3 = true;
				break;
			}

		if (Input.GetKey(selectMenuOption))
		{
			switch(count)
			{
			case 0:
				Screen.showCursor = false;
				Application.LoadLevel(1);
				break;
			case 1:
				optionsMenu = true;
				break;
			case 2:
				Application.Quit();
				break;
			}

		}
	}


	IEnumerator lightFlicker()
	{
		while (true)
		{
			float waitTime = Random.Range(1f, 5f);
			yield return new WaitForSeconds(waitTime);
			lightOn = !lightOn;
			//lightOnG.SetActive(lightOn);
		}
	}


	void OnGUI()
	{
		//Can only define based on screen inside OnGUI
		playRect.Set(Screen.width * .02f, Screen.height * .5f, Screen.width *.25f, Screen.height * .15f);
		optionsRect.Set(Screen.width * .02f, Screen.height * .67f, Screen.width *.25f, Screen.height * .15f);
		quitRect.Set(Screen.width * .02f, Screen.height * .84f, Screen.width *.25f, Screen.height * .15f);
		macControls.Set(Screen.width * .02f, Screen.height * .5f, Screen.width *.25f, Screen.height * .15f);
		pcControls.Set(Screen.width * .02f, Screen.height * .67f, Screen.width *.25f, Screen.height * .15f);
		backButton.Set(Screen.width * .02f, Screen.height * .84f, Screen.width *.25f, Screen.height * .15f);

		defaultSkin =  GUI.skin.button.normal.background;
		defaultSkin = GUI.skin.button.hover.background;

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), lightOffG, ScaleMode.ScaleToFit, true, 0f);

		if (lightOn)
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), lightOnG, ScaleMode.ScaleToFit, true, 0f);
		}

		//Main Menu Options
		if ( optionsMenu == false)
		{
		//sets up buttons
			GUI.skin.button.normal.background = playButtonBlack;
			GUI.skin.button.hover.background = playButtonWhite;
			if (GUI.Button(playRect, ""))
			{
				Screen.showCursor = false;
				Application.LoadLevel(1);
				buttonClicked1 = !buttonClicked1;
			}
			GUI.skin.button.normal.background = options1;
			GUI.skin.button.hover.background = options2;
			if (GUI.Button(optionsRect, ""))
			{
				//load options
				optionsMenu = true;
				buttonClicked2 = !buttonClicked2;
			}
			GUI.skin.button.normal.background = quit;
			GUI.skin.button.hover.background = quit2;
			if (GUI.Button(quitRect, ""))
			{
				Application.Quit();

			}
			audioCheck();

		//Options Menu Options
		}else
		{
			GUI.skin.button.normal.background = macCon;
			GUI.skin.button.hover.background = macCon2;
			if(GUI.Button(macControls, ""))
			{
				PlayerScript.setKeys(KeyCode.JoystickButton16, KeyCode.JoystickButton14, KeyCode.JoystickButton19, KeyCode.JoystickButton18, KeyCode.JoystickButton13);
				SoundScript.setSoundKeys(KeyCode.JoystickButton16, KeyCode.JoystickButton14);
				ControlScript.mac = true;
				SoundScript.mac = true;
			}
			GUI.skin.button.normal.background = PCCon;
			GUI.skin.button.hover.background = PCCon2;
			if (GUI.Button(pcControls, ""))
			{
				PlayerScript.setKeys(KeyCode.JoystickButton0, KeyCode.JoystickButton5, KeyCode.JoystickButton3, KeyCode.JoystickButton2, KeyCode.JoystickButton4);
				SoundScript.setSoundKeys(KeyCode.JoystickButton0, KeyCode.JoystickButton5);
				ControlScript.mac = false;
				ControlScript.mac = false;
			}
			GUI.skin.button.normal.background = mainMenu;
			GUI.skin.button.hover.background = mainMenu2;
			if(GUI.Button(backButton, ""))
			{
				optionsMenu = false;
			}
			audioCheck();
		}

		mouseExit();

		GUI.skin.button.normal.background = GUI.skin.GetStyle("Box").normal.background;
		GUI.skin.button.hover.background = GUI.skin.GetStyle("Box").normal.background;

	}
	//Checks if the buttons contain the mouse to make a sound
	void audioCheck()
	{
		//Mouse over audio
		if (playRect.Contains(Event.current.mousePosition))
		{
			if (isPlaying == false)
			{
				isPlaying = true;
				audio.PlayOneShot(select);
				setRect(playRect);
			}
		}
		if (optionsRect.Contains(Event.current.mousePosition))
		{
			//guiTexture.color = Color.green;

			if (isPlaying == false)
			{
				isPlaying = true;
				audio.PlayOneShot(select);
				setRect(optionsRect);
			}
		}
		if (quitRect.Contains(Event.current.mousePosition))
		{
			if (isPlaying == false)
			{
				isPlaying = true;
				audio.PlayOneShot(select);
				setRect(quitRect);
			}
		}
		// Options buttons 
		if (macControls.Contains(Event.current.mousePosition))
		{
			if (isPlaying == false)
			{
				isPlaying = true;
				audio.PlayOneShot(select);
				setRect(macControls);
			}
		}
		if (pcControls.Contains(Event.current.mousePosition))
		{
			if (isPlaying == false)
			{
				isPlaying = true;
				audio.PlayOneShot(select);
				setRect(pcControls);
			}
		}
		if (backButton.Contains(Event.current.mousePosition))
		{
			if (isPlaying == false)
			{
				isPlaying = true;
				audio.PlayOneShot(select);
				setRect(backButton);
			}
		}
	}

	//sets the GUI button entered to test for the mouse exiting it
	void setRect(Rect temp)
	{
		sentRect = temp;
	}
	//tests if the mouse has exited the GUI button
	void mouseExit()
	{
		if(!sentRect.Contains(Event.current.mousePosition))
		{
			isPlaying = false;
		}
	}

}
