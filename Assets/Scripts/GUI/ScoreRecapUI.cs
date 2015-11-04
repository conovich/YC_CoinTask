﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreRecapUI : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } } 


	public ParticleSystem funParticles;
	public TextMesh[] ObjectLocationScores;
	public TextMesh[] ObjectNames;
	public Vector3 ObjectVisualOffset;
	public TextMesh TimeBonusText;
	public TextMesh TotalTrialScoreText;
	public TextMesh TrialNumText;

	public Transform ObjectScoreContent; //this may have to be moved/realigned depending on how many objects were in the trial

	Vector3 centralContentOrigPos;

	// Use this for initialization
	void Start () {
		Enable (false);
		centralContentOrigPos = ObjectScoreContent.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Play(int numTrialsComplete, int currentTrialScore, int maxNumTrials, List<int> objectScores, List<GameObject> specialObjects, int timeBonus, float time){
		Enable (true);

		Reset();

		PlayJuice ();

		if (objectScores.Count > ObjectLocationScores.Length) {
			Debug.Log ("TOO MANY OBJECTS WERE FOUND. NOT ENOUGH TEXT MESHES.");
		}
		else {
			int trialScore = 0;
			for (int i = 0; i < objectScores.Count; i++) {
				//put objects in the right places
				//objectFoundVisuals [i].gameObject.transform.position = ObjectLocationScores [i].transform.position + ObjectVisualOffset;

				//set object score text & object names
				string currObjectScore = FormatScore(objectScores[i]);
				ObjectLocationScores [ObjectLocationScores.Length - 1 - i].text = currObjectScore;
				ObjectNames [ObjectNames.Length - 1 - i].text = specialObjects [i].GetComponent<SpawnableObject>().GetName () + ":";

				trialScore += objectScores [i];
			}

			//adjust positioning of the central content based on how many objects there were. or weren't.
			if(ObjectLocationScores.Length > 2){
				float distanceBetweenObjectText = ObjectLocationScores[0].transform.position.y - ObjectLocationScores[1].transform.position.y;
				int spaceToMoveMult = ObjectLocationScores.Length - objectScores.Count;
				ObjectScoreContent.transform.position += Vector3.up * ( Mathf.Abs(distanceBetweenObjectText) * spaceToMoveMult );
			}

			TimeBonusText.text = FormatScore(timeBonus);


			TrialNumText.text = "trial " + (numTrialsComplete) + "/" + maxNumTrials + " completed";

			TotalTrialScoreText.text = FormatScore(trialScore + timeBonus);

		}

	}

	void PlayJuice(){
		if (Config_CoinTask.isJuice) {
			JuiceController.PlayParticles (funParticles);
		}
	}

	public void Stop(){
		Enable (false);
	}

	string FormatScore(int score){
		string scoreText = score.ToString ();
		if(score > 0){
			scoreText = "+" + scoreText;
		}

		return scoreText;
	}

	void Reset(){
		ObjectScoreContent.position = centralContentOrigPos;

		for (int i = 0; i < ObjectLocationScores.Length; i++) {
			ObjectLocationScores[i].text = "";
			ObjectNames[i].text = "";
		}

	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);
		
		UsefulFunctions.EnableChildren( transform, shouldEnable );
	}
}
