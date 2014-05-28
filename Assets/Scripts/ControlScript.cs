using UnityEngine;
using System.Collections;

public class ControlScript : MonoBehaviour 
{

	public static bool mac = false;
	public delegate void ControlCourier(Vector3 directionL, Vector3 directionR, float triggers);
	public static event ControlCourier controlCourier;
	private Vector3 directionL, directionR;
	private float threshold = 0.1f;
	private float triggers, triggerL, triggerR;
	private bool ready = false;

	void Update()
	{
		//print (Input.GetAxis("MacTriggerL") + ", " + Input.GetAxis("MacTriggerR"));
	}

	void FixedUpdate()
	{
		directionL.x = Input.GetAxis("Horizontal");
		directionL.y = Input.GetAxis("Vertical");
		directionL.z = 0;
		if(mac)
		{
			triggerL = Input.GetAxis("MacTriggerL");
			triggerR = Input.GetAxis("MacTriggerR");
			if(triggerL != 0 && triggerR != 0)
			{
				ready = true;
			}
			if(!ready)
			{
				if(triggerL == 0)
				{
					triggerL = -1;
				}
				if(triggerR == 0)
				{
					triggerR = -1;
				}
			}

			triggers = (triggerR - triggerL) / 2;
			directionR.x = Input.GetAxis("MacHorizontalR");
			directionR.y = Input.GetAxis("MacVerticalR");
			directionR.z = 0;
		} else
		{
			directionR.x = Input.GetAxis("HorizontalR");
			directionR.y = Input.GetAxis("VerticalR");
			directionR.z = 0;
			triggers = Input.GetAxis("Triggers");
		}

		if(Mathf.Abs(directionL.x) < threshold)
		{
			directionL.x = 0f;
		}
		if(Mathf.Abs(directionL.y) < threshold)
		{
			directionL.y = 0f;
		}

		controlCourier(directionL, directionR, triggers);
	}
}
