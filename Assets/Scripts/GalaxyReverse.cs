using UnityEngine;
using System.Collections;

public class GalaxyReverse : MonoBehaviour {

	public GameObject galaxy;
	// Use this for initializatio

	void Start () 
	{
		Time.timeScale = 1;
		galaxy.animation["blackHoleRotate"].speed = -1;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
