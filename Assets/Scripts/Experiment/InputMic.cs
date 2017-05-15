﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMic : MonoBehaviour {

	public static float MicLoudness;
	public Dropdown micDrops;
	private float maxLoud=0f;
	private string _device;
	public bl_ProgressBar volProg;
	private bool cannotHear=true;
	public Text spokenWord;
	public string[] wordList;
	private int currentWord = 0;
	public Image marker;
	public Text beginExperimentText;
	public Image loud;
	public GameObject micTestTexts;
	public CanvasGroup samsonWarningGroup;
	private bool samsonFound=false;
	private List<string> micList=new List<string>();
	//mic initialization
	void Start()
	{
		

		beginExperimentText.enabled = false;
		loud.gameObject.SetActive (true);
		micDrops.AddOptions (micList);
		micTestTexts.SetActive (true);
	}

	IEnumerator WaitUntilSamsonConnection()
	{
		while (!samsonFound) {
			InitMic ();
			yield return new WaitForSeconds (2f);
			yield return 0;
		}
		yield return null;
	}
	void InitMic(){
		int chosenMicDrop = 0;
		for (int i = 0; i < Microphone.devices.Length; i++) {
			Debug.Log (Microphone.devices [i].ToString ());
			micList.Add (Microphone.devices [i].ToString ());
			if (Microphone.devices [i].ToString ().Contains ("Samson")) {
				samsonFound = true;
				chosenMicDrop = i;
				UnityEngine.Debug.Log ("SAMSON FOUND");
			}
		}
		if (!samsonFound) {
			UnityEngine.Debug.Log ("samson not found");
			samsonWarningGroup.alpha = 1f;
			StartCoroutine ("WaitUntilSamsonConnection");
		} else {
			UnityEngine.Debug.Log ("samson found");
		}
		if(_device == null) _device = Microphone.devices[chosenMicDrop];
		_clipRecord = Microphone.Start(_device, true, 999, 44100);
	}
	IEnumerator RotateWords()
	{
		spokenWord.text = wordList [0];
		float timer = 0f;
		while (cannotHear) {
			timer += Time.deltaTime;
			if (timer > 5f) {
				spokenWord.color = Color.red;
				spokenWord.text = "Sorry, I cannot hear you! \n Please adjust microphone \n Press (X) to Continue.";
				micTestTexts.SetActive (false);
				yield return new WaitForSeconds (1f);
				spokenWord.color = Color.white;
				timer = 0f;
				currentWord++;
				if (currentWord > 4)
					currentWord = 0;
				spokenWord.text = wordList [currentWord];
			}
				micTestTexts.SetActive (true);
			if (MicLoudness > Config_CoinTask.micLoudThreshold) {
				cannotHear = false;
				loud.gameObject.SetActive (false);
				marker.color = Color.green;
			}
			yield return 0;
		}
		yield return null;
	}

	public IEnumerator RunMicTest()
	{
		yield return StartCoroutine ("RotateWords");
		yield return new WaitForSeconds (4f);
		yield return null;

	}

	void StopMicrophone()
	{
		Microphone.End(_device);
	}


	AudioClip _clipRecord = new AudioClip();
	int _sampleWindow = 128;

	//get data from microphone into audioclip
	float  LevelMax()
	{
		float levelMax = 0;
		float[] waveData = new float[_sampleWindow];
		int micPosition = Microphone.GetPosition(null)-(_sampleWindow+1); // null means the first microphone
		if (micPosition < 0) return 0;
		_clipRecord.GetData(waveData, micPosition);
		// Getting a peak on the last 128 samples
		for (int i = 0; i < _sampleWindow; i++) {
			float wavePeak = waveData[i] * waveData[i];
			if (levelMax < wavePeak) {
				levelMax = wavePeak;
			}
		}
		return levelMax;
	}



	void Update()
	{
//		Debug.Log ("mic" +  LevelMax().ToString());
		// levelMax equals to the highest normalized value power 2, a small number because < 1
		// pass the value to a static var so we can access it from anywhere
		MicLoudness = LevelMax ();

		if (maxLoud < MicLoudness)
			maxLoud = MicLoudness;
		if (cannotHear)
			volProg.Value = MicLoudness;
		else {
			beginExperimentText.enabled = true;
			spokenWord.color = Color.green;
			spokenWord.text = "I heard you say " + wordList [currentWord];
		}
	}

	bool _isInitialized;
	// start mic when scene starts
	void OnEnable()
	{
		InitMic();
		_isInitialized=true;
	}

	//stop mic when loading a new level or quit application
	void OnDisable()
	{
		StopMicrophone();
	}

	void OnDestroy()
	{
		StopMicrophone();
	}


	// make sure the mic gets started & stopped when application gets focused
	void OnApplicationFocus(bool focus) {
		if (focus)
		{
			//Debug.Log("Focus");

			if(!_isInitialized){
				//Debug.Log("Init Mic");
				InitMic();
				_isInitialized=true;
			}
		}      
		if (!focus)
		{
			//Debug.Log("Pause");
			StopMicrophone();
			//Debug.Log("Stop Mic");
			_isInitialized=false;

		}
	}
}