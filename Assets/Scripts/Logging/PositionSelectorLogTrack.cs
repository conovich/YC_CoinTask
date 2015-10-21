using UnityEngine;
using System.Collections;

public class PositionSelectorLogTrack : LogTrack {
	EnvironmentPositionSelector envPosSelector { get { return Experiment_CoinTask.Instance.environmentController.myPositionSelector; } }
	GameObject selectorObject { get { return Experiment_CoinTask.Instance.environmentController.myPositionSelector.PositionSelector; } }
	GameObject selectorVisuals { get { return Experiment_CoinTask.Instance.environmentController.myPositionSelector.PositionSelectorVisuals; } }

	
	// Use this for initialization
	void Start () {
		InitialLog ();
	}

	void InitialLog(){
		LogSelectorSize ();
	}

	void LogSelectorSize(){
		float diameter = Config_CoinTask.selectionDiameter;
		subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "SELECTOR_SIZE" + separator + "DIAMETER" + separator + diameter);
	}

	public void LogPositionChosen(Vector3 chosenPosition, Vector3 correctPosition, SpawnableObject specialSpawnable){
		if (ExperimentSettings_CoinTask.isLogging) {
			//log chosen position
			subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "CHOSEN_TEST_POSITION" + separator + chosenPosition.x + separator + chosenPosition.y + separator + chosenPosition.z + separator + specialSpawnable.GetName());
			//log correct position
			subjectLog.Log (exp.theGameClock.SystemTime_Milliseconds, subjectLog.GetFrameCount (), gameObject.name + separator + "CORRECT_TEST_POSITION" + separator + correctPosition.x + separator + correctPosition.y + separator + correctPosition.z + separator + specialSpawnable.GetName());
		}
	}
	
}
