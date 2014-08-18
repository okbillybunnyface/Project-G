using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    public float damage = 0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter(Collision collision)
	{
		collision.gameObject.SendMessage("Damage",damage,SendMessageOptions.DontRequireReceiver);
		Destroy(this.gameObject);
	}
}
