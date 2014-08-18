using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

    public float damage = 0f;
    public float lifetime = 5f;

    void OnEnable()
    {
        StartCoroutine(DespawnTimer());
    }

	void OnCollisionEnter(Collision collision)
	{
		collision.gameObject.SendMessage("Damage",damage,SendMessageOptions.DontRequireReceiver);
        gameObject.SetActive(false);
	}

    public void Respawn(Vector3 position)
    {
        gameObject.SetActive(false);
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
