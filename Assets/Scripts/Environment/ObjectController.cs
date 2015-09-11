using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour {

	public GameObject DefaultObject;
	public List<SpawnableObject> CurrentTrialSpecialObjects;


	//experiment singleton
	Experiment_CoinTask experiment { get { return Experiment_CoinTask.Instance; } }

	//object array & list
	List<GameObject> gameObjectList_Spawnable;
	//List<GameObject> gameObjectList_Spawned; //a list to keep track of the objects currently in the scene



	// Use this for initialization
	void Start () {
		gameObjectList_Spawnable = new List<GameObject> ();
		CurrentTrialSpecialObjects = new List<SpawnableObject> ();
		CreateObjectList (gameObjectList_Spawnable);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void CreateObjectList(List<GameObject> gameObjectListToFill){
		gameObjectListToFill.Clear();
		Object[] prefabs = Resources.LoadAll("Prefabs/Objects");
		for (int i = 0; i < prefabs.Length; i++) {
			gameObjectListToFill.Add((GameObject)prefabs[i]);
		}
	}

	void CreateSpawnableList (List<SpawnableObject> spawnableListToFill){
		spawnableListToFill.Clear();
		Object[] prefabs = Resources.LoadAll("Prefabs/Objects");
		for (int i = 0; i < prefabs.Length; i++) {
			SpawnableObject spawnable = ( (GameObject)prefabs[i] ).GetComponent<SpawnableObject>();
			spawnableListToFill.Add(spawnable);
		}
	}

	public GameObject ChooseSpawnableObject(string objectName){
		List<SpawnableObject> allSpawnables = new List<SpawnableObject>(); //note: this is technically getting instantiated twice now... as it's instantiated in CREATE as well.
		CreateSpawnableList (allSpawnables);

		for (int i = 0; i < allSpawnables.Count; i++) {
			if(allSpawnables[i].GetName() == objectName){
				return allSpawnables[i].gameObject;
			}
		}
		return null;
	}


	GameObject ChooseRandomObject(){
		if (gameObjectList_Spawnable.Count == 0) {
			Debug.Log ("No MORE objects to pick! Recreating object list.");
			CreateObjectList(gameObjectList_Spawnable); //IN ORDER TO REFILL THE LIST ONCE ALL OBJECTS HAVE BEEN USED
			if(gameObjectList_Spawnable.Count == 0){
				Debug.Log ("No objects to pick at all!"); //if there are still no objects in the list, then there weren't any to begin with...
				return null;
			}
		}


		int randomObjectIndex = Random.Range(0, gameObjectList_Spawnable.Count);
		GameObject chosenObject = gameObjectList_Spawnable[randomObjectIndex];
		gameObjectList_Spawnable.RemoveAt(randomObjectIndex);
		
		return chosenObject;
	}


	public float GenerateRandomRotationY(){
		float randomRotY = Random.Range (0, 360);
		return randomRotY;
	}

	//for use in trial with configuration setup of the grid
	/*public void SpawnDefaultObjects(List<Vector2> GridIndices){
		for(int i = 0; i < GridIndices.Count; i++){
			Vector2 currIndices = GridIndices[i];
			Vector3 objPos = experiment.environmentController.myGrid.GetGridPosition( (int)currIndices.x, (int)currIndices.y );
			objPos = new Vector3(objPos.x, DefaultObject.transform.position.y, objPos.z);
			GameObject newObj = Instantiate(DefaultObject, objPos, DefaultObject.transform.rotation) as GameObject;

			GridItem newGridItem = newObj.GetComponent<GridItem>();
			newGridItem.rowIndex = (int)currIndices.x;
			newGridItem.colIndex = (int)currIndices.y;

			SpawnableObject newSpawnableObj = newObj.GetComponent<SpawnableObject>();
			newSpawnableObj.SetNameID(i);
		}
	}*/

	//special positions get passed in so that the default object can get a special tag for later use for spawning special objects
	public void SpawnDefaultObjects(List<Vector2> defaultPositions, List<Vector2> specialPositions){
		for(int i = 0; i < defaultPositions.Count; i++){
			Vector2 currPos = defaultPositions[i];
			Vector3 objPos = new Vector3(currPos.x, DefaultObject.transform.position.y, currPos.y);
			GameObject newObj = Instantiate(DefaultObject, objPos, DefaultObject.transform.rotation) as GameObject;
			
			SpawnableObject newSpawnableObj = newObj.GetComponent<SpawnableObject>();
			newSpawnableObj.SetNameID(i);

			if( specialPositions.Contains(currPos) ){
				newObj.tag = "DefaultSpecialGridItem";
			}
		}
	}


	//for more generic object spawning -- such as in Replay!
	public GameObject SpawnObject( GameObject objToSpawn, Vector3 spawnPos ){
		GameObject spawnedObj = Instantiate(objToSpawn, spawnPos, objToSpawn.transform.rotation) as GameObject;

		return spawnedObj;
	}

	//spawn random object at a specified location
	public GameObject SpawnSpecialObjectXY (Vector3 spawnPosition){
		GameObject objToSpawn = ChooseRandomObject ();
		if (objToSpawn != null) {

			GameObject newObject = Instantiate(objToSpawn, spawnPosition, objToSpawn.transform.rotation) as GameObject;

			float randomRot = GenerateRandomRotationY();
			newObject.transform.RotateAround(newObject.transform.position, Vector3.up, randomRot);

			CurrentTrialSpecialObjects.Add(newObject.GetComponent<SpawnableObject>());

			//newObject.GetComponent<GridItem>().rowIndex = (int)gridIndices.x;
			//newObject.GetComponent<GridItem>().colIndex = (int)gridIndices.y;
		

			//make object face the player -- DOESN'T WORK FOR OBJECTS WITH INCONSISTENT Z-FACING TRANSFORMS.
			//don't want object to tilt downward at the player -- use object's current y position
			// Vector3 lookAtPos = new Vector3(experiment.player.transform.position.x , newObject.transform.position.y, experiment.player.transform.position.z);
			//newObject.transform.LookAt(lookAtPos);

			return newObject;
		}
		else{
			return null;
		}
	}

	public List<Vector2> GenerateDefaultObjectPositions (int numDefaultObjects){
		List<Vector2> defaultPositions = new List<Vector2> ();

		for (int i = 0; i < numDefaultObjects; i++) {
			//generate random position in the environment
			Vector3 randomEnvPosition = experiment.environmentController.GetRandomPositionWithinWallsXZ( Config_CoinTask.objectToWallBuffer );
			Vector2 randomEnvPositionVec2 = new Vector2(randomEnvPosition.x, randomEnvPosition.z);

			int numTries = 0;
			int maxNumTries = 15; //ARBITRARY.
			//make sure that this position is far enough away from all other default object locations we've already generated
			while( !CheckObjectsFarEnoughXZ( randomEnvPositionVec2, defaultPositions) && numTries < maxNumTries ){
				//try again to generate a valid position
				randomEnvPosition = experiment.environmentController.GetRandomPositionWithinWallsXZ( Config_CoinTask.objectToWallBuffer );
				randomEnvPositionVec2 = new Vector2(randomEnvPosition.x, randomEnvPosition.z);
				numTries++;
			}

			if (numTries == 15){
				Debug.Log("Tried 15 times to place default objects!");
			}

			defaultPositions.Add(randomEnvPositionVec2);

		}

		return defaultPositions;
	}

	public bool CheckObjectsFarEnoughXZ(Vector2 newObjectPos, List<Vector2> otherObjectPositions){
		for(int i = 0; i < otherObjectPositions.Count; i++){
			Vector2 previousDefaultObjectLocation = otherObjectPositions[i];
			if( (newObjectPos - previousDefaultObjectLocation).magnitude < Config_CoinTask.objectToObjectBuffer ){
				return false;
			}
		}
		
		return true;
	}

	public List<Vector2> GenerateSpecialObjectPositions (List<Vector2> defaultObjectLocationsXZ, int numSpecialObjects){
		List<Vector2> specialPositions = new List<Vector2> ();
		List<Vector2> defaultPositionsCopy = new List<Vector2> ();
		
		//copy the list so we can delete from it...
		for (int i = 0; i < defaultObjectLocationsXZ.Count; i++) {
			Vector2 currPosition = defaultObjectLocationsXZ[i];
			Vector2 positionCopy = new Vector2(currPosition.x, currPosition.y);
			defaultPositionsCopy.Add(positionCopy);
		}
		
		for (int i = 0; i < numSpecialObjects; i++) {
			int randomIndex = Random.Range(0, defaultPositionsCopy.Count);
			
			//if number of special objects exceeds the number of free spots, we'll get stuck.
			//...so exit the loop instead.
			if(i > defaultPositionsCopy.Count){
				break;
			}
			
			Vector2 currPosition = defaultPositionsCopy[i];

			specialPositions.Add (currPosition);
			
			//remove it from the parameter list so that no two special objects are in the same spot.
			//this will not change the original list, as the list was passed by copy.
			defaultPositionsCopy.RemoveAt(randomIndex);
		}
		
		return specialPositions;
	}

	public float GetMaxDefaultObjectColliderBoundXZ(){
		//create an active instance in order to retrieve the collider bounds. otherwise the bounds will be zero.
		//there might be a less computationally expensive way to do this -- perhaps instead put a default object in the scene and keep it there for this, turn it off when not being used.
		GameObject activeDefaultObj = Instantiate (DefaultObject, transform.position, Quaternion.identity) as GameObject;

		Collider defaultCollider = activeDefaultObj.GetComponent<Collider> ();
		Vector3 bounds = defaultCollider.bounds.size;
		float maxBound = bounds.x;
		if (bounds.z > bounds.x){
			maxBound = bounds.z;
		}

		Destroy (activeDefaultObj);

		return maxBound;
	}

}
