using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision collision)
	{
		collision.gameObject.SendMessage("Damage",25f,SendMessageOptions.DontRequireReceiver);
		Destroy(this.gameObject);
	}
}
