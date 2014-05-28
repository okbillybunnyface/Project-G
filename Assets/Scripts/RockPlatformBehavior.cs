using UnityEngine;
using System.Collections;

public class RockPlatformBehavior : MonoBehaviour {

	public AudioClip sound;
	public GameObject sarahRayCast;
	public bool touchingGround = false;
	// Use this for initialization

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		PlaySound(); 
	}

	public void OnTriggerEnter(Collider sarah) {
		Debug.Log (sarah.gameObject.name);
		if(sarah.gameObject.name == "Sarah"){
			touchingGround = true;
		}
	}

	public void OnTriggerExit(Collider sarah) {
		Debug.Log (sarah.gameObject.name);
		if(sarah.gameObject.name == "Sarah"){
			touchingGround = false;
		}
	}

	void PlaySound(){
		if(Input.GetAxis("Horizontal") > 0){
			if( touchingGround == true && !audio.isPlaying){
				audio.clip = sound;
				audio.loop = true;
				audio.Play();
			}	else{
			audio.Stop();
		}
	}
}

}
