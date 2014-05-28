using UnityEngine;
using System.Collections;

public class Toggler : MonoBehaviour {

	public AudioClip clip;
	public AudioSource source;
	public GameObject[] toggledObjects;
	private GameObject player;
	private bool toggled = false;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void OnEnable()
	{
		toggled = false;
		if(source != null) source.clip = clip;
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject == player && !toggled)
		{
			toggled = true;
			if(source != null) source.Play();
			foreach(GameObject element in toggledObjects)
			{
				element.SetActive(!element.activeSelf);
			}
		}
	}
}
