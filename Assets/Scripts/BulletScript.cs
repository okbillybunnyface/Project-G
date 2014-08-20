using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    public float damage = 0f;
    public float lifetime = 5f;
    public int explosionParticles = 30;
    private Projector projector;

    void Start()
    {
        projector = gameObject.GetComponent<Projector>();
        particleSystem.renderer.sortingLayerName = "Effects";
    }

    void OnEnable()
    {
        StartCoroutine(DespawnTimer());
    }

	void OnCollisionEnter(Collision collision)
	{
		collision.gameObject.SendMessage("Damage",damage,SendMessageOptions.DontRequireReceiver);
        collider.enabled = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        if (projector != null) projector.Disable();
        for (int i = 0; i < explosionParticles; i++)
        {
            particleSystem.Emit(transform.position, new Vector3(Random.value * 10 - 5, Random.value * 10 - 5, 0f), Random.value, Random.value * 2, particleSystem.startColor);
        }
        rigidbody.isKinematic = true;
	}

    public void Respawn(Vector3 position)
    {
        gameObject.SetActive(false);
        if (projector != null) projector.Enable();
        collider.enabled = true;
        rigidbody.isKinematic = false;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        rigidbody.velocity = Vector3.zero;
        transform.position = position;
        gameObject.GetComponent<ParticleSystem>().Clear();
        gameObject.SetActive(true);
    }

    IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(lifetime);
        gameObject.SetActive(false);
    }
}
