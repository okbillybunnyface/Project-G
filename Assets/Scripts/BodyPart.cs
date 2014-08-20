using UnityEngine;
using System.Collections;

public class BodyPart : MonoBehaviour {

    public float time = 1f;
    public int goreParticles = 200;
    public SpriteRenderer sprite;

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(GoreTimer());
	}

    IEnumerator GoreTimer()
    {
        yield return new WaitForSeconds(Random.value * time);

        collider.enabled = false;
        sprite.enabled = false;
        for (int i = 0; i < goreParticles; i++)
        {
            particleSystem.Emit(transform.position, new Vector3(Random.value * 10 - 5, Random.value * 10 - 5, 0f), Random.value, Random.value, particleSystem.startColor);
        }
        rigidbody.isKinematic = true;

        yield return new WaitForSeconds(2f);

        Destroy(gameObject);
    }
}
