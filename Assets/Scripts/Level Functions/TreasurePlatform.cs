using UnityEngine;
using System.Collections;

public class TreasurePlatform : MonoBehaviour {

	
	public SarahAnimation saraAnimScript;
	public AudioSource upgradeSound;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter(Collision collision) 
	{
		if (collision.gameObject.tag == "Player" && !upgradeSound.isPlaying)
		{
			saraAnimScript.sarahIsUpgrading(true);
			upgradeSound.Play();
		}
	}
	void OnCollisionExit(Collision collision)
	{
			saraAnimScript.sarahIsUpgrading(false);
	}

}
