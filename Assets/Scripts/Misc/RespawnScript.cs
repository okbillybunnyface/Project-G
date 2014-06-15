using UnityEngine;
using System.Collections;

public class RespawnScript : MonoBehaviour {
	
	public float respawnTimer = 0;
	public float enemyRespawnTimer = 60;
	private GameObject[] enemies;
	private GameObject player;
	
	// Use this for initialization
	void Start () 
	{
		enemies = GameObject.FindGameObjectsWithTag("Enemy");
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
		//If the player is deactivated...
		if(!player.activeSelf)
		{//...set the player to be active.
			player.SetActive(true);
		}
		
		for(int i = 0; i < enemies.Length; i++)
		{//If all the enemies are deactivated...
			if(!enemies[i].activeSelf)
			{//...set them to be active in enemyRespawnTimer seconds.
				StartCoroutine(respawn(enemyRespawnTimer, enemies[i]));
			}
		}
	}
	
	IEnumerator respawn (float seconds, GameObject thingy) 
	{
		yield return new WaitForSeconds(seconds);
		thingy.SetActive(true);
	}
}
