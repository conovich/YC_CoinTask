﻿using UnityEngine;
using System.Collections;

public class GridItem : MonoBehaviour {

	public ParticleSystem DefaultParticles;
	public ParticleSystem SpecialParticles;
	
	public AudioSource defaultCollisionSound;
	public AudioSource specialCollisionSound;
	bool shouldDie = false;

	public int rowIndex;
	public int colIndex;

	EnvironmentGrid envGrid { get { return Experiment_CoinTask.Instance.environmentController.myGrid; } }

	// Use this for initialization
	void Start () {

	}

	public Vector2 GetGridIndices(){
		return new Vector2 (rowIndex, colIndex);
	}
	
	// Update is called once per frame
	void Update () {
		if(shouldDie && (defaultCollisionSound.isPlaying == false && specialCollisionSound.isPlaying == false)){
			Destroy(gameObject); //once audio has finished playing, destroy the item!
		}
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "Player" && tag == "DefaultGridItem") {

			//turn invisible, play sound
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
			shouldDie = true;

			EnvironmentGrid.GridSpotType mySpotType = envGrid.GetGridSpotType (rowIndex, colIndex);
			Debug.Log(mySpotType.ToString());
			if (mySpotType == EnvironmentGrid.GridSpotType.specialGridItem && tag == "DefaultGridItem") {

				Experiment_CoinTask.Instance.scoreController.AddSpecialPoints();

				//if it was a special spot and this is the default object...
				//...we should spawn the special object!
				//TODO: spawn with default coins, show on collision???
				GameObject specialObject = Experiment_CoinTask.Instance.objectController.SpawnSpecialObjectXY(new Vector2(rowIndex, colIndex), transform.position);
				SpecialParticles.Play();
				specialCollisionSound.Play ();

				//tell the trial controller to wait for the animation
				StartCoroutine(Experiment_CoinTask.Instance.trialController.WaitForSpecialAnimation(specialObject));
			}
			else{
				Experiment_CoinTask.Instance.scoreController.AddDefaultPoints();

				DefaultParticles.Play();
				defaultCollisionSound.Play();
				Experiment_CoinTask.Instance.trialController.IncrementNumDefaultObjectsCollected();
			}
		}
	}

	void OnDestroy(){
		envGrid.RemoveGridItem (rowIndex, colIndex);
	}
}
