using UnityEngine;
using System.Collections;

public class ProjectionNode : MonoBehaviour {

	public LineRenderer line;//duh
	public GameObject node;//node prefab goes here
	private GameObject nextNode;//next node in chain
	private GameObject projectee;//Whatever is asking for a projection
	private ProjectionNode nodeScript;
	private GravityList gravityList;
	private Vector3 position, velocity;//These are the simulation values. transform.position is the actual position of the node
	private bool useGravity, //whether the projectee is set to use gravity or not
		inGravityField; //Physics.Gravity is disabled in gravity fields
	private float timeStep, //Time parameter for physics calculations
		cascadeDelay; //Time before next node activation (currently not used)

	// Use this for initialization
	void Start () 
	{
		//Sets up the line renderer
		line.SetVertexCount(2);
		line.SetWidth(0.1f, 0.1f);
		line.enabled = false;//makes sure the line is off


		//these need to be the same on upon initialization
		position = transform.position;
	}

	void OnEnable()
	{
		line.enabled = false;//makes sure the line is off

		//these need to be the same on upon initialization
		position = transform.position;

		//Resets the linkedlist of nearby gravity sources
		gravityList = new GravityList();
	}

	void OnDisable()
	{
		line.enabled = false;//makes sure the line is off

		//recursively deactivates the next node, so that all the nodes deactivate together by deactivating the first
		if(nextNode != null)
		{
			nextNode.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		//This lerp makes the line move smoothly to its destination
		if(transform.position != position)transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime / Time.timeScale * 8);


		//If the next node exists and is active, draw a line from this node to the next
		if(nextNode != null)
		{
			if(nextNode.activeSelf)
			{
				line.SetPosition(0, this.transform.position);
				line.SetPosition(1, nextNode.transform.position);
			}else
			{
				line.enabled = false;
			}
		}
	}

	//When the node collides with a gravity object, it adds the gravity object to its list.
	void OnTriggerEnter(Collider collider)
	{
		//This *should* be fine since nodes should only ever trigger with gravity sources

		gravityList.Enqueue(collider.gameObject);
		gravityList.Reset();
	}

	//When a node stops colliding with a gravity object, it removes the gravity object from its list.
	void OnTriggerExit(Collider collider)
	{
		//This *should* be fine since nodes should only ever trigger with gravity sources
		gravityList.Dequeue(collider.gameObject);
	}

	//THIS MUST BE CALLED IMMEDIATELY AFTER NODE CREATION OR THE WORLD WILL END
	public void Initialize(int nodeNumber, int maxNodes, float timeStep, float cascadeDelay, GameObject projectee)
	{
		this.timeStep = timeStep;//time parameter in physics calcs
		this.cascadeDelay = cascadeDelay;//time before next node activation
		this.projectee = projectee;//whatever wants its motion predicted
		gravityList = new GravityList();//linkedlist for gravity sources


		if(nodeNumber < maxNodes)//If we haven't reached the specified number of nodes then:
		{
			nextNode = (GameObject)GameObject.Instantiate(node, transform.position, Quaternion.identity);//Create a new node
			nodeScript = nextNode.GetComponent<ProjectionNode>();//Fetch the new node's script for easy access
			nodeScript.Initialize(nodeNumber + 1, maxNodes, timeStep, cascadeDelay, projectee);//Initialize the next node with the same parameters, but iterate nodeNumber
		}
	}

	//Recursive function to propogate physics calculations.
	public void Projection(Vector3 position, Vector3 velocity, bool gravity)
	{
		useGravity = gravity;

		//Set the position and velocity parameters to be what the previous node calculated they should be
		this.position = position;
		this.velocity = velocity;

		//Calculate what the instantaneous acceleration at position should be
		Vector3 acceleration = SumAcceleration();

		//Align the transform along the acceleration just like a Character gameObject (for better collision detection)
		transform.up = -acceleration.normalized;


		if(nextNode != null)
		{
			//Reuses the method vars. These don't change the class vars
			position = GetPosition(position, velocity, acceleration);//Calculate the position of the next node
			velocity = GetVelocity(velocity, acceleration);//Calculate the velocity of the next node


			if(!nextNode.activeSelf)//If the next node is inactive, move it to where it should be and activate it
			{
				nextNode.transform.position = position;
				nextNode.SetActive(true);
			}

			nextNode.transform.up = -acceleration.normalized;//Align the next node's transform

			//Recursively call this method on the next node, informing it of this node's calculations
			nodeScript.Projection(position, velocity, gravity);

			//Set up the line from this node to the next
			line.SetPosition(0, this.transform.position);
			line.SetPosition(1, nextNode.transform.position);
			line.SetColors(GetColor(this.velocity), GetColor(velocity));
			line.enabled = true;
		}

		//Old code, might be useful to see later:
		//StartCoroutine(ProjectionTimer(position, velocity, gravity));
	}

	Vector3 SumAcceleration()
	{
		Vector3 acceleration = Vector3.zero;//Start with no acceleration
		inGravityField = false;//set to true later if a GravityField is detected in the node's linkedlist

		if(!gravityList.IsEmpty())//If the list is empty don't bother
		{
			gravityList.Reset();//Reset the list to the first item in line

			//If the first item is a gravity field set inGravityField to be true
			if(gravityList.GetCurrent().GetItem().tag == "GravityField")
			{
				inGravityField = true;
			}

			//If the first item is set to affect the projectee, add the acceleration it would apply
			if(gravityList.GetCurrent().GetScript().Affect(projectee))
			{
				acceleration += gravityList.GetCurrent().GetScript().GetAcceleration(position);
			}

			//Repeat on all the remaining items
			while(gravityList.GetCurrent().HasNext())
			{
				gravityList.Next();

				if(gravityList.GetCurrent().GetItem().tag == "GravityField")
				{
					inGravityField = true;
				}

				if(gravityList.GetCurrent().GetScript().Affect(projectee))
				{
					acceleration += gravityList.GetCurrent().GetScript().GetAcceleration(position);
				}
			}
		}

		//Add Physics.gravity if it applies
		if(useGravity && !inGravityField)
		{
			acceleration += Physics.gravity;
		}

		return acceleration;
	}

	//Calculates velocity based upon ye olde equation
	Vector3 GetVelocity(Vector3 velocity, Vector3 acceleration)
	{
		//ye olde equation
		velocity = acceleration * timeStep + velocity;

		//if the velocity ever exceeds the max specified by Movement, reset it to the max
		if(velocity.sqrMagnitude > Movement.maxSpeed * Movement.maxSpeed)
		{
			velocity = velocity.normalized * Movement.maxSpeed;
		}

		return velocity;
	}

	//Calculates position based on ye olde equation. Or not. Unity's equation is fucked up, so it's theirs instead
	Vector3 GetPosition(Vector3 position, Vector3 velocity, Vector3 acceleration)
	{
		return timeStep * (acceleration * timeStep + velocity) + position;
		//Note: It might be worthwhile to calculate based upon initial and final velocity instead
	}

	//Calculates the color of the line
	Color GetColor(Vector3 velocity)
	{
		float temp = 1020f * velocity.magnitude / Character.maxSpeed - 765f;
		if(velocity.magnitude / Character.maxSpeed >= 0.75f)
		{
			return new Color(255f, 255f - temp, 0f);
		}
		else if(velocity.magnitude / Character.maxSpeed >= 0.5f)
		{
			return new Color(255f + temp, 255f, 0f);
		}
		else
		{
			return new Color(0f, 255f, 0f);
		}
	}

	//This coroutine does the same thing as the Projection method, but with a delay between nodes. I found it to be painfully slow.
	IEnumerator ProjectionTimer(Vector3 position, Vector3 velocity, bool gravity)
	{
		this.position = position;
		this.velocity = velocity;
		//this.transform.position = position;
		Vector3 acceleration = SumAcceleration();
		if(nextNode != null)
		{
			//yield return new WaitForSeconds(Time.timeScale * cascadeDelay);
			position = GetPosition(position, velocity, acceleration);
			velocity = GetVelocity(velocity, acceleration);
			if(!nextNode.activeSelf) 
			{
				nextNode.transform.position = position;
			}
			nextNode.SetActive(true);
			yield return new WaitForSeconds(Time.timeScale * cascadeDelay);
			nodeScript.Projection(position, velocity, gravity);
			//yield return new WaitForSeconds(Time.timeScale * cascadeDelay);
			line.SetPosition(0, this.transform.position);
			line.SetPosition(1, nextNode.transform.position);
			line.SetColors(GetColor(this.velocity), GetColor(velocity));
			line.enabled = true;
		}
	}
}
