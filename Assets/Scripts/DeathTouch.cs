using UnityEngine;
using System.Collections;

public class DeathTouch : MonoBehaviour {
	
	public float damage = 200;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision)
	{
		collision.gameObject.SendMessage("damage", damage, SendMessageOptions.DontRequireReceiver);
	}
}
