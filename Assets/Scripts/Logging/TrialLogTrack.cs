using UnityEngine;
using System.Collections;

public class TrialLogTrack : LogTrack {


	bool firstLog = false;

	// Update is called once per frame
	void Update () {
		//just log the environment info on the first frame
		if (ExperimentSettings_CoinTask.isLogging && !firstLog) {
			LogEnvironmentDimensions ();
			firstLog = true;
		}
	}

	//gets called from trial controller instead of in update!
	public void Log(int trialNumber, int numTreasureChests, int numSpecialObjects, bool isSequential){
		if (ExperimentSettings_CoinTask.isLogging) {
			LogTrial (trialNumber, numTreasureChests, numSpecialObjects, isSequential);
		}
	}

	//LOGGED ON THE START OF THE TRIAL.
	void LogTrial(int trialNumber, int numTreasureChests, int numSpecialObjects, bool isSequential){
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Trial Info" + separator + "NUM_TRIALS" + separator + trialNumber + separator
		                + "NUM_TREASURE" + separator + numTreasureChests + separator + "NUM_SPECIAL_OBJECTS" + separator + numSpecialObjects + separator 
		                + "IS_SEQUENTIAL" + separator + isSequential);
	}


	//TODO: move to an experiment or an environment logger... just want to log this once at the beginning of the trials so there is a reference for all positions in the world.
	void LogEnvironmentDimensions(){
		//log center
		Vector3 envCenter = exp.environmentController.center;
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_CENTER" + separator + envCenter.x + separator + envCenter.y + separator + envCenter.z);
	
		//log walls
		Vector3 wallPos = exp.environmentController.WallsXPos.position;
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_WALL_XPOS" + separator + wallPos.x + separator + wallPos.y + separator + wallPos.z);

		wallPos = exp.environmentController.WallsXNeg.position;
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_WALL_XNEG" + separator + wallPos.x + separator + wallPos.y + separator + wallPos.z);

		wallPos = exp.environmentController.WallsZPos.position;
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_WALL_ZPOS" + separator + wallPos.x + separator + wallPos.y + separator + wallPos.z);

		wallPos = exp.environmentController.WallsZNeg.position;
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount(), "Experiment Info" + separator + "ENV_WALL_ZNEG" + separator + wallPos.x + separator + wallPos.y + separator + wallPos.z);
		Debug.Log ("LOGGED ENV");
	}



	//TODO: move to an experiment logger
	public void LogWaitForJitterStarted(float jitter){
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "RANDOM_JITTER_STARTED" + separator + jitter);
		Debug.Log ("JITTER STARTED LOGGED: " + jitter);
	}
	
	//TODO: move to an experiment logger
	public void LogWaitForJitterEnded(float jitter){
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "RANDOM_JITTER_ENDED" + separator + jitter);
		Debug.Log ("JITTER ENDED LOGGED: " + jitter);
	}




	public void LogDoubleDownResponse(bool response){
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "DOUBLE_DOWN_RESPONSE" + separator + response);
		Debug.Log ("DOUBLE DOWN LOGGED: " + response);
	}

	public void LogRememberResponse(bool response){
		subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "REMEMBER_RESPONSE" + separator + response);
		Debug.Log ("REMEMBER LOGGED: " + response);
	}

	//if the UI answer selector has moved TODO: move to an answer selector logger?
	public void LogAnswerPositionMoved(bool isYesPosition, bool isRememberResponse){ //either remember response or double down response
		string answerPosition = "NO";
		if (isYesPosition) {
			answerPosition = "YES";
		}

		if(isRememberResponse){
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "REMEMBER_ANSWER_MOVEMENT" + separator + answerPosition);
			Debug.Log ("REMEMBER MOVEMENT LOGGED: " + answerPosition);
		}
		else{
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "DOUBLE_DOWN_ANSWER_MOVEMENT" + separator + answerPosition);
			Debug.Log ("DOUBLE DOWN MOVEMENT LOGGED: " + answerPosition);
		}
	}


	//THE FOLLOWING ARE EVENTS

	public void LogInstructionEvent(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "SHOWING_INSTRUCTIONS");
			Debug.Log ("Logged instruction event.");
		}
	}

	public void LogBeginningExplorationEvent(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "FREE_EXPLORATION_STARTED");
			Debug.Log ("Logged exploration event.");
		}
	}

	public void LogTransportationToHomeEvent(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "HOMEBASE_TRANSPORT_STARTED");
			Debug.Log ("Logged home transport event.");
		}
	}

	public void LogTransportationToTowerEvent(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "TOWER_TRANSPORT_STARTED");
			Debug.Log ("Logged tower transport event.");
		}
	}

	public void LogTrialNavigationStarted(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "TRIAL_NAVIGATION_STARTED");
			Debug.Log ("Logged nav started event.");
		}
	}

	public void LogDistractorGameStarted(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "DISTRACTOR_GAME_STARTED");
			Debug.Log ("Logged distractor game started event.");
		}
	}

	public void LogRecallPhaseStarted(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "RECALL_PHASE_STARTED");
			Debug.Log ("Logged recall started event.");
		}
	}

	public void LogObjectToRecall(SpawnableObject spawnableToRecall){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "RECALL_SPECIAL" + separator + spawnableToRecall.GetName ());
			Debug.Log ("Logged object recall event.");
		}
	}

	public void LogFeedbackStarted(){
		if (ExperimentSettings_CoinTask.isLogging) {
			subjectLog.Log (Experiment_CoinTask.Instance.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), "Trial Event" + separator + "FEEDBACK_STARTED");
			Debug.Log ("Logged feedback event.");
		}
	}

}