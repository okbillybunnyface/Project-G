using UnityEngine;
using System.Collections;

public class MainMenuScript : MonoBehaviour {


	public KeyCode selection, downSelect, upSelect;
	public GameObject start, options, exit;
	private int count = 0;
	private GameObject[] selectors;
	public AudioClip select, verify, back;


	// Use this for initialization
	void Start () 
	{
		selectors = new GameObject[3];
		selectors [0] = start;
		selectors [1]= options;
		selectors [2] = exit;
	}
	
	// Update is called once per frame
	void Update () {

		buttonClicked();
		movement();
		//Debug.Log(count);
	
	}


	void buttonClicked()
	{
		if (Input.GetKeyDown(selection) || Input.GetKeyDown(KeyCode.Return))
		{
			switch(count)
			{
			case 0:
				audio.PlayOneShot(verify);
				Application.LoadLevel(1);
				break;
			case 1:
				//open options menu
				audio.PlayOneShot(verify);
				break;
			case 2:
				audio.PlayOneShot(back);
				Application.Quit();
				Debug.Log ("this is hitting quit");
				break;

			}

		}
	}

	void movement()
	{
		//Move cursor down
		//moves every update, too quick
		//if (Input.GetAxisRaw ("Vertical") < 0) 
		if(Input.GetKeyDown(downSelect))
		{
			selectors[count].SetActive(false);
			count++;
			audio.PlayOneShot(select);
			if (count > 2)
			{
				count = 0;
			}
			selectors[count].SetActive(true);
		}

		// Move cursor up
		//if (Input.GetAxisRaw ("Vertical") > 0) 
		if(Input.GetKeyDown(upSelect))
		{
			selectors[count].SetActive(false);
			count--;
			audio.PlayOneShot(select);
			if(count <0)
			{
				count = 2;
			}
			selectors[count].SetActive(true);
		}
	}

}
