using UnityEngine;
using System.Collections;

public class ElevatorScript: MonoBehaviour {
	
	public int numberCars;
	public float speed;
    private float distance, timeSpawnGap, laptime;
	private CarClass [] allCars;  //private to not display in editor.  Set from numberCars
    private CarClass ghostCar;
	public Transform [] allStops;
	public GameObject platform;
	
	
	void Start () {
		
		allCars = new CarClass[numberCars];
        timeSpawnGap = getTotalDistance();  //get path total distance

        ////Specify class for inner class and spawn them
        //for(int i =0; i < numberCars; i ++){
        //    allCars[i] = new CarClass(platform, this, allStops[0], allStops[1]);
        //}

        ghostCarSpawning();
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
	public float getTotalDistance(){
        //Starting at index 1 to calculate from the preceding car
		for(int i = 1; i < allStops.Length; i++){
			distance += (allStops[i].position - allStops[i-1].position).magnitude;
		}
		laptime = distance / speed;
        return laptime / numberCars;
	}

	public Transform nextStop(bool next, int stopNumber){
		
		if(next)
		{
			if ((stopNumber + 1) < allStops.Length){
                stopNumber += 1;
				return allStops[stopNumber];
			}
			return null;
		}
		else
			return allStops[stopNumber];
		
	}


    public void ghostCarSpawning() {

        float timeIncrement = 0;
        ghostCar = new CarClass(platform, this, allStops[0], allStops[1]);

        for (int i =0; i < allCars.Length; i++){
            //If timeGap passes a spawn, it will increment with nextStop(), need parameters if it surpases more than one stop in an increment
            allCars[i] = new CarClass(platform, this, ghostCar.platform.transform, ghostCar.destination);
            allCars[i].currentStopNumber = ghostCar.currentStopNumber;
            timeIncrement += timeSpawnGap; 
            ghostCar.canIMove(timeIncrement);
                          
        }
        ghostCar.platform.gameObject.SetActive(false);

    }
	
	[System.Serializable]
	public class CarClass {
		
		private ElevatorScript elevator;
		public Transform destination, spawn;
		public int currentStopNumber = 0;
		public GameObject platform;
		
		public CarClass(GameObject platform, ElevatorScript wrapper, Transform spawn, Transform destintion){
			this.elevator = wrapper;
			this.platform = (GameObject)Instantiate(elevator.platform);
			this.spawn = spawn;
			this.destination = destintion;

			platform.transform.position = spawn.transform.position;
		}

		public void canIMove (float time){
			
			Vector3 dist = time * elevator.speed * (platform.transform.position - destination.transform.position).normalized;
            //Vector3 dist = (platform.transform.position - destination.transform.position);
			
			if (dist.sqrMagnitude > (platform.transform.position - destination.transform.position).sqrMagnitude) 
			{
				destination = elevator.nextStop(true, currentStopNumber);
                Debug.Log("stop udpate");
				currentStopNumber++;
			}

			movement(time);
			
		}
		
		
		public void movement(float time){

			//platform.transform.position += (platform.transform.position - destination.transform.position).normalized * time * elevator.speed;
            platform.transform.position += (destination.transform.position - platform.transform.position).normalized * time * elevator.speed;
		}
	}
	
}


