using UnityEngine;
using System.Collections;

public class EnableAfterSeconds : MonoBehaviour {

    public float seconds = 10f;

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(Enabler());
        gameObject.collider.enabled = false;
	}

    IEnumerator Enabler()
    {
        yield return new WaitForSeconds(seconds);

        gameObject.collider.enabled = true;
    }
}
