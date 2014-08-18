using UnityEngine;
using System.Collections;

public class GunTurretScript : MonoBehaviour {

	// Use this for initialization
	public GameObject gunTurret, bulletPrefab;
    public GameObject bulletSpawn;
	private GameObject clone, player;
    public float maxAngle = 60f;
    public float rotationSpeed = 5f; 
	public float bulletSpeed = 60f;
    public float range = 30;
    public float fireRate = 10f;
    private GameObject[] bullets;
	private bool canFire = true, sighted = false;
    private float rate;
    private int bulletIter = 0;
    private int bulletMagSize = 50;//Size of bullet array


	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player");
        rate = 1 / fireRate;
        
        bullets = new GameObject[bulletMagSize];
        for (int b = 0; b < bullets.Length; b++)
        {
            bullets[b] = (GameObject)GameObject.Instantiate(bulletPrefab);
            bullets[b].SetActive(false);
        }
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

    bool canSwitch = true;

	void shouldGunRotate()
	{
		if(sighted)
		{
			gunTurret.transform.up = transform.position - player.transform.position;
		}
		else
		{
			float maxAngle = Vector3.Angle(-transform.up, (-1 * gunTurret.gameObject.transform.up));
            if (maxAngle > this.maxAngle)
            {
                if (canSwitch)
                {
                    rotationSpeed *= -1;
                    canSwitch = false;
                }
            }
            else canSwitch = true;

            Debug.DrawRay(gunTurret.transform.position, -gunTurret.transform.up * range, Color.green);
			moveGunTurret(rotationSpeed * Time.deltaTime);
		}
	}

	void moveGunTurret(float rotate)
	{
			gunTurret.gameObject.transform.Rotate(new Vector3(0,0,rotate));
	}


	void fireGun()
	{
		//fire Gun
        bullets[bulletIter].GetComponent<BulletScript>().Respawn(bulletSpawn.transform.position);
        bullets[bulletIter].rigidbody.AddForce(-gunTurret.transform.up * bulletSpeed, ForceMode.VelocityChange);

        bulletIter = (bulletIter + 1) % bullets.Length;

        bulletSpawn.GetComponent<ParticleSystem>().Emit(25);
	}
	
	IEnumerator FireDelay()
	{
		yield return new WaitForSeconds(rate);
		canFire = true;
	}

}
