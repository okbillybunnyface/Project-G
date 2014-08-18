using UnityEngine;
using System.Collections;

/*The maximum size of any gravity sphere is the size you make its collider in the scene (not its transform scale).
 It will always be a fraction of this size.*/
[RequireComponent(typeof(SphereCollider))]
public class GravitySphereScript : GravityScript 
{	
	//Instance variables
    public ParticleSystem antiParticles;
    public GameObject gravObject, antiObject;
    public SpriteRenderer spriteRenderer;
    public Sprite gravSprite, antiSprite;
	public float dissipationRate = 0.5f;
	public float effectFactor = 0.2f;
	public bool isSticky = false;
    public bool isStable = false;

    public void Start()
    {
        gravParticles.renderer.sortingLayerName = "Effects";
        antiParticles.renderer.sortingLayerName = "Effects";
    }

    public void OnEnable()
    {
        base.OnEnable();
        gravParticles.Clear();
        antiParticles.Clear();
    }

	//Called once per frame
	public override void Update()
	{
		Debug.DrawRay(transform.position, transform.right * transform.localScale.x, Color.red);
		Debug.DrawRay(transform.position, -transform.right * transform.localScale.x, Color.red);
		Debug.DrawRay(transform.position, transform.up * transform.localScale.y, Color.red);
		Debug.DrawRay(transform.position, -transform.up * transform.localScale.y, Color.red);
	}

	//Physics updates
	public override void FixedUpdate () 
	{
		base.FixedUpdate();

        if (isEnvironment)
        {
            if (!isStable && energy * anti < maxEnergy)
            {
                energy += anti * maxEnergy * dissipationRate * Time.deltaTime;
            }
        }
        else if (!isCharging)
        {
            if (!isStable)
            {
                energy -= energy * dissipationRate * Time.deltaTime;
            }
        }


        ParticleUpdate();
	}

	public void SetIsSticky(bool truthiness)
	{
		isSticky = truthiness;
	}

	public override Vector3 GetDirection(Vector3 position)
	{
		Vector3 direction = this.transform.position - position;
		return direction + direction.normalized * 0.2f;
	}

	public override void ParticleUpdate()
	{
		float scale = energy/ maxEnergy;
		gravParticles.startSpeed = -8 * scale;
        antiParticles.startSpeed = 8 * scale;
		gravParticles.emissionRate = anti *  energy * effectFactor * effectFactor / 5;

        //Alters the gravity sphere's size based upon available energy.
        scale *= anti;
        this.transform.localScale = new Vector3(scale, scale, 1f);
		
		if(energy >= 0)
		{
            spriteRenderer.sprite = gravSprite;
            antiObject.SetActive(false);
            gravObject.SetActive(true);
		}
		else
		{
            spriteRenderer.sprite = antiSprite;
            gravObject.SetActive(false);
            antiObject.SetActive(true);
		}
	}

    public void StartDespawnTimer(float time)
    {
        StartCoroutine(Timer(time));
    }

    public void SetIsStable(bool truthiness)
    {
        isStable = truthiness;
    }

    IEnumerator Timer(float time)
    {
        isStable = true;
        yield return new WaitForSeconds(time);
        isStable = false;
    }
}
