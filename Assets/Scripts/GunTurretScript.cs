using UnityEngine;
using System.Collections;

public class GunTurretScript : MonoBehaviour {

	// Use this for initialization
	public GameObject gunTurret, bulletPrefab, gunSightEnd;
	public GameObject LeftBulletSpawn, RightBulletSpawn, LeftBulletDirection, RightBulletDirection;
	private GameObject clone, player;
	public float bulletSpeed = 60f;
	private float rotationAngle = 1; 
	private bool canFire = true, sighted = false;


	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void FixedUpdate () 
	{

	}
	void Update()
	{
		shouldGunRotate();

		if (Input.GetKey(KeyCode.J))
		{
			fireGun();
		}

		RaycastHit castOut;

		if(Physics.Linecast(gunTurret.transform.position, gunSightEnd.transform.position, out castOut))
		{
			sighted = (castOut.collider.gameObject.tag == "Player");
			if (sighted && canFire)
			{
				canFire = false;
				fireGun();
				StartCoroutine(FireDelay());
			}
		}
		else sighted = false;
	}

	void shouldGunRotate()
	{
		if(sighted)
		{
			gunTurret.transform.up = transform.position - player.transform.position;
		}
		else
		{
			float maxAngle = Vector3.Angle(Vector3.down, (-1 * gunTurret.gameObject.transform.up));
			if ( maxAngle > 45.0f)
			{
				rotationAngle *= -1;
			}
			
			Debug.DrawLine(gunTurret.transform.position,gunSightEnd.transform.position, Color.green);
			moveGunTurret(rotationAngle);
		}
	}

	void moveGunTurret(float rotate)
	{
			gunTurret.gameObject.transform.Rotate(new Vector3(0,0,rotate));
	}


	void fireGun()
	{
		//fire Left Gun
		clone = Instantiate(bulletPrefab, LeftBulletSpawn.gameObject.transform.position, LeftBulletSpawn.gameObject.transform.rotation) as GameObject;
		clone.transform.position = LeftBulletSpawn.transform.position;
		clone.SetActive(true);
		clone.rigidbody.AddForce(-gunTurret.transform.up * bulletSpeed, ForceMode.VelocityChange);


		//fire Right Gun
		clone = Instantiate(bulletPrefab, RightBulletSpawn.gameObject.transform.position, RightBulletSpawn.gameObject.transform.rotation) as GameObject;
		clone.transform.position = RightBulletSpawn.transform.position;
		clone.SetActive(true);
		clone.rigidbody.AddForce(-gunTurret.transform.up * bulletSpeed, ForceMode.VelocityChange);
	}
	
	IEnumerator FireDelay()
	{
		yield return new WaitForSeconds(0.02f);
		canFire = true;
	}

}
