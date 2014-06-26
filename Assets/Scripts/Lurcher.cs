using UnityEngine;
using System.Collections;

public class Lurcher : Walker {

	private float damage = 10;
	private bool canLurch = false;

	public override void OnCollisionEnter(Collision col)
	{
		base.OnCollisionEnter(col);
		if(col.gameObject == player)
		{
			player.SendMessage("Damage", damage);
		}
	}

	protected override void ActivationBehaviours(Vector3 toPlayer)
	{
		base.ActivationBehaviours(toPlayer);
		StartCoroutine(LurchReset());
	}

	protected override void DeactivationBehaviours()
	{
		transform.position = startPos;
		health = healthMax;
		rigidbody.velocity = Vector3.zero;
		canLurch = false;
	}

	protected override void Behaviours(Vector3 toPlayer)
	{
		toPlayer = player.transform.position - this.transform.position;

		LurchChecker(toPlayer);

		if(canLurch && toPlayer.magnitude > range)
		{
			Seek(toPlayer, Time.deltaTime);
		}
		else if(toPlayer.magnitude < range * 4)
		{
			Seek(-toPlayer, Time.deltaTime);
		}
		else
		{
			Walk(toPlayer * 0f, Time.deltaTime);
		}
	}

	void LurchChecker(Vector3 toPlayer)
	{
		if(!jumping && canLurch && toPlayer.magnitude < 2 * range && toPlayer.magnitude > 3 / 2 * range)
		{
			StartCoroutine(LurchReset());
			Lurch(toPlayer);
		}
	}

	void Lurch(Vector3 direction)
	{
		JumpPrep();
		StartCoroutine(Jumping(transform.up, 0f));
		rigidbody.AddForce(jumpForce * (direction.normalized + transform.up / 2), ForceMode.Impulse);
	}

	IEnumerator LurchReset()
	{
		canLurch = false;
		yield return new WaitForSeconds(3);
		canLurch = true;
	}
}
