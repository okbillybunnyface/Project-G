using UnityEngine;
using System.Collections;

public class ProjectionBase : MonoBehaviour {

	public float physicsTimeStep = 0.02f;//time parameter in projection calculations
	public float cascadeDelay = 0.001f;//time before a node projects the next node
	public float updateDelay = 0.2f;//time delay between node updates
	public GameObject node;//node prefab goes here
	public bool globalGravity;//whether to use physics.usegravity or not
	private Vector3 position, velocity;//should be the current position and velocity of the projectee
	private GameObject initialNode;//first node
	private GameObject projectee;//Whatever wants its trajectory predicted
	private ProjectionNode nodeScript;//first node's script

	void OnEnable()
	{
		//Sets teh initial node to be at the base's location if it exists
		if(initialNode != null)
		{
			initialNode.transform.position = transform.position;
		}
		this.position = transform.position;
		this.velocity = Vector3.zero;
		StartCoroutine(Projection());
	}

	//Set the base to be disabled to stop trajectory predictions.
	void OnDisable()
	{
        nodeScript.ResetSources();

		//I disabled this to show off the feature. Normally, this would make the line disappear as soon as projection ceases.
		initialNode.SetActive(false);
	}

	//MUST BE CALLED IMMEDIATELY UPON INSTANTIATION!!!
	public void Initialize(GameObject creator/*Whatever is initializing this projection*/, int nodes)
	{
		//Instantiate the first node and put it on top of the projectee
		initialNode = (GameObject)GameObject.Instantiate(node, transform.position, Quaternion.identity);
		initialNode.transform.position = creator.transform.position;
		nodeScript = initialNode.GetComponent<ProjectionNode>();

		//Initialize the nodes
		nodeScript.Initialize(0, nodes, physicsTimeStep, cascadeDelay, creator);
	}

	//Method to be called by the projectee to update the position and velocity vars
	public void Project(Vector3 position, Vector3 velocity, bool gravity)
	{
		this.position = position;
		this.velocity = velocity;
		globalGravity = gravity;
	}

	//Coroutine to update the nodes based upon the information supplied by Project()
	IEnumerator Projection()
	{
		while(true)
		{
			yield return new WaitForSeconds(Time.timeScale * updateDelay);
			//initialNode.transform.position = transform.position;
			initialNode.SetActive(true);
			nodeScript.Projection(position, velocity, globalGravity);
		}
	}
}
