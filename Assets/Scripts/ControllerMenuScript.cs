using UnityEngine;
using System.Collections;

public class ControllerMenuScript : MonoBehaviour {

	public Texture2D controllerLayout;
	public Rect anyKeyButton;
	string buttonText = "Press Any Key";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.anyKeyDown)
		{
			LoadNextLevel(2);
		}
	}

	void LoadNextLevel(int lvlNum)
	{
		Application.LoadLevel(lvlNum);
		buttonText = "Loading";
	}

	void OnGUI()
	{
		GUI.skin.button.normal.background = GUI.skin.GetStyle("Box").normal.background;
		GUI.skin.button.hover.background = GUI.skin.GetStyle("Box").normal.background;
		anyKeyButton.Set(Screen.width * .75f, Screen.height * .75f, Screen.width *.25f, Screen.height * .1f);

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), controllerLayout, ScaleMode.ScaleToFit, true, 0f);

//		GUI.skin.button.normal.background = playButtonBlack;
//		GUI.skin.button.hover.background = playButtonWhite;
		if (GUI.Button(anyKeyButton, buttonText))
		{
			LoadNextLevel(2);

		}

		if (Event.current.type == EventType.KeyDown) 
		{
			LoadNextLevel(2);
		}
	}
}
