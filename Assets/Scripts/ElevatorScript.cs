using UnityEngine;
using System.Collections;

public class ElevatorScript: MonoBehaviour {
	
	public int numberCars;
	public float speed, distance, laptime;
	private CarClass [] allCars;
	public Transform [] allStops;
	public GameObject platform;
	
	
	void Start () {
		
		allCars = new CarClass[numberCars];
		getTotalDistance();  //get path total distance

		//Specify class for inner class
		for(int i =0; i < numberCars; i ++){
			allCars[i] = new CarClass(platform, this, allStops[0], allStops[1]);
		}
	}
	
	void Update () {
		startRunning();
	}
	
	public void startRunning () {
		
		for(int i =0; i < numberCars; i ++){
			allCars[i].canIMove(Time.deltaTime);
		}
	}

	//Calculates total distance of lap and time for lap
	public void getTotalDistance(){

		for(int i = 1; i < allStops.Length; i++){

			distance += (allStops[i].position - allStops[i-1].position).magnitude;
		}

		laptime = distance / speed;
	}

	public Transform nextStop(bool next, int stopNumber){
		
		if(next)
		{
			if ((stopNumber + 1) <= allStops.Length){
				return allStops[stopNumber+1];
			}
			return allStops[stopNumber];
		}
		else
			return allStops[stopNumber];
		
	}
	
	
	[System.Serializable]
	public class CarClass {
		
		private ElevatorScript elevator;
		Transform destination, spawn;
		int currentStopNumber = 0;
		private GameObject platform;
		
		public CarClass(GameObject platform, ElevatorScript wrapper, Transform spawn, Transform destintion){
			this.elevator = wrapper;
			this.platform = (GameObject)Instantiate(elevator.platform);
			this.spawn = spawn;
			this.destination = destintion;

			platform.transform.position = spawn.transform.position;
		}

		public void canIMove (float time){
			
			Vector3 dist = time * elevator.speed * (platform.transform.position - destination.transform.position).normalized;
			
			if (dist.sqrMagnitude > (platform.transform.position - destination.transform.position).sqrMagnitude) 
			{
				elevator.nextStop(true, currentStopNumber);
				currentStopNumber++;
			}

			movement(Time.deltaTime);
			
		}
		
		
		public void movement(float time){

			platform.transform.position += (platform.transform.position - destination.transform.position).normalized * time * elevator.speed;
		}
	}
	
}


