using UnityEngine;
using System.Collections;

public class ControlScript : MonoBehaviour 
{
	
	public static bool mac = false;
	public delegate void ControlCourier(Vector3 directionL, Vector3 directionR, float triggers);
	public static event ControlCourier controlCourier;
	private Vector3 directionL, directionR;
	private float threshold = 0.1f;
	private float triggers, triggerL, triggerR, 
	prevControllerX, prevControllerY, prevMouseX, prevMouseY;
	private bool ready = false, mouseChange = false, controllerChange = false;
	
	void Start()
	{
		if(mac)
		{
			directionR.x = Input.GetAxis("MacHorizontalR");
			directionR.y = Input.GetAxis("MacVerticalR");
		}
		else
		{
			directionR.x = Input.GetAxis("HorizontalR");
			directionR.y = Input.GetAxis("VerticalR");
		}
		prevControllerX = directionR.x;
		prevControllerY = directionL.y;
		prevMouseX = Input.GetAxis("Mouse X");
		prevMouseY = Input.GetAxis("Mouse Y");
		
		StartCoroutine(Mouse());
	}
	
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
			directionR.z = 0;
		} else
		{
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

		if(directionR.magnitude < 0.02f)
		{
			directionR = directionL / 2f;
		}

		controlCourier(directionL, directionR, triggers);
	}
	
	IEnumerator Mouse()
	{
		do
		{
			directionR.x += Input.GetAxis("Mouse X") / 20;
			if(directionR.x > 1) directionR.x = 1;
			if(directionR.x < -1) directionR.x = -1;
			directionR.y += Input.GetAxis("Mouse Y") / 20;
			if(directionR.y > 1) directionR.y = 1;
			if(directionR.y < -1) directionR.y = -1;
			if(mac)
			{
				controllerChange = !(prevControllerX == Input.GetAxis("MacHorizontalR") && prevControllerY == Input.GetAxis("MacVerticalR"));
				prevControllerX = Input.GetAxis("MacHorizontalR");
				prevControllerY = Input.GetAxis("MacVerticalR");
			}
			else
			{
				controllerChange = !(prevControllerX == Input.GetAxis("HorizontalR") && prevControllerY == Input.GetAxis("VerticalR"));
				prevControllerX = Input.GetAxis("HorizontalR"); 
				prevControllerY = Input.GetAxis("VerticalR");
			}
			yield return new WaitForFixedUpdate();
		}
		while(!controllerChange);
		StartCoroutine(Controller());
	}
	
	IEnumerator Controller()
	{
		do
		{
			if(mac)
			{
				directionR.x = Input.GetAxis("MacHorizontalR");
				directionR.y = Input.GetAxis("MacVerticalR");
			}
			else
			{
				directionR.x = Input.GetAxis("HorizontalR");
				directionR.y = Input.GetAxis("VerticalR");
			}
			mouseChange = !(Input.GetAxis("Mouse X") == prevMouseX && Input.GetAxis("Mouse Y") == prevMouseY);
			prevMouseX = Input.GetAxis("Mouse X");
			prevMouseY = Input.GetAxis("Mouse Y");
			yield return new WaitForFixedUpdate();
		}
		while(!mouseChange);
		StartCoroutine(Mouse());
	}
}
