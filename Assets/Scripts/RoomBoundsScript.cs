using UnityEngine;
using System.Collections;

public class RoomBoundsScript : MonoBehaviour {

	private bool activeRoom = false;

	void OnEnable()
	{
		PlayerScript.PlayerRoom += PlayerRoom;
	}

	void OnDisable()
	{
		PlayerScript.PlayerRoom -= PlayerRoom;
	}

	void PlayerRoom(GameObject room)
	{
		activeRoom = (room == this.gameObject);
	}

	public void Respawn(GameObject thingy)
	{
		StartCoroutine(Respawner(thingy));
	}

	IEnumerator Respawner(GameObject thingy)
	{
		while(activeRoom)
		{
			yield return new WaitForSeconds(1f);
		}

		thingy.SetActive(true);
	}
}
