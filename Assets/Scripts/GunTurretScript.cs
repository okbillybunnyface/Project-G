using UnityEngine;
using System.Collections;

public class GunTurretScript : MonoBehaviour {

	// Use this for initialization
	public GameObject gunTurret, bulletPrefab, gunSightEnd;
    public GameObject bulletSpawn;
	private GameObject clone, player;
	public float bulletSpeed = 60f;
    public float range = 30;
    public float fireRate = 10f;
	private float rotationAngle = 1; 
	private bool canFire = true, sighted = false;
    private float rate;


	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
        rate = 1 / fireRate;
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

		if(Physics.Raycast(gunTurret.transform.position, -gunTurret.transform.up, out castOut, range))
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
		clone = Instantiate(bulletPrefab, bulletSpawn.gameObject.transform.position, Quaternion.identity) as GameObject;
		clone.transform.position = bulletSpawn.transform.position;
		clone.SetActive(true);
		clone.rigidbody.AddForce(-gunTurret.transform.up * bulletSpeed, ForceMode.VelocityChange);
	}
	
	IEnumerator FireDelay()
	{
		yield return new WaitForSeconds(rate);
		canFire = true;
	}

}
