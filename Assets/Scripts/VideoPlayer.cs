﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VideoPlayer : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }
	
	MovieTexture movie;
	AudioSource movieAudio;
	
	public CanvasGroup group;
	
	void Awake(){
		group.alpha = 0.0f;
	}
	
	// Use this for initialization
	void Start () {
		RawImage rim = GetComponent<RawImage>();
		if(rim != null){
			movie = (MovieTexture)rim.mainTexture;
		}
		movieAudio = GetComponent<AudioSource> ();
	}
	
	bool isMoviePaused = false;
	void Update () {
		if (movie != null) {
			if (movie.isPlaying) {
				if (Input.GetAxis (Config_CoinTask.ActionButtonName) > 0.2f) { //skip movie!
					Stop ();
				}
				if (TrialController.isPaused) {
					Pause ();
				}
			}
			if (!TrialController.isPaused) {
				if (isMoviePaused) {
					UnPause ();
				}
			}
		} 
		//else {
			//Debug.Log("No movie attached! Can't update.");
		//}
	}
	
	public IEnumerator Play(){
		if (movie != null) {
			group.alpha = 1.0f;
			
			movie.Stop ();
			movieAudio.Play ();
			movie.Play ();
			
			while (movie.isPlaying || isMoviePaused) {
				yield return 0;
			}
			
			isMoviePaused = false;
			
			group.alpha = 0.0f;
			yield return 0;
		} 
		else {
			Debug.Log("No movie attached! Can't play.");
		}
	}
	
	void Pause(){
		if(movie != null){
			movie.Pause();
			movieAudio.Pause ();
			isMoviePaused = true;
		} 
		else {
			Debug.Log("No movie attached! Can't pause.");
		}
	}
	
	void UnPause(){
		if(movie != null){
			movie.Play ();
			movieAudio.UnPause ();
			isMoviePaused = false;
		} 
		else {
			Debug.Log("No movie attached! Can't unpause.");
		}
	}
	
	void Stop(){
		if(movie != null){
			isMoviePaused = false;
			movie.Stop ();
		} 
		else {
			Debug.Log("No movie attached! Can't stop.");
		}
	}
	
}
