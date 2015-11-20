﻿using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

public class SyncboxControl : MonoBehaviour {
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	//DYNLIB FUNCTIONS
	[DllImport ("liblabjackusb")]
	private static extern float LJUSB_GetLibraryVersion( );


	[DllImport ("ASimplePlugin")]
	private static extern int PrintANumber();
	[DllImport ("ASimplePlugin")]
	private static extern float AddTwoFloats(float f1,float f2);
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr OpenUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr CloseUSB();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOn();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr TurnLEDOff();
	[DllImport ("ASimplePlugin")]
	private static extern float SyncPulse();
	[DllImport ("ASimplePlugin")]
	private static extern IntPtr StimPulse(float durationSeconds, float freqHz, bool doRelay);
	
	public bool ShouldPulse = false;
	public float PulseOnSeconds;
	public float PulseOffSeconds;
	public TextMesh DownCircle;
	public Color DownColor;
	public Color UpColor;

	public bool isUSBOpen = false; //TODO: set to true.

	bool isToggledOn = false;


	//SINGLETON
	private static SyncboxControl _instance;
	
	public static SyncboxControl Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
		
		if (_instance != null) {
			UnityEngine.Debug.Log("Instance already exists!");
			Destroy(transform.gameObject);
			return;
		}
		_instance = this;
		
	}


	// Use this for initialization
	void Start () {
		if(ExperimentSettings_CoinTask.isSyncbox){
			//Debug.Log(AddTwoFloats(2.5F,4F));
			//Debug.Log ("OH HAYYYY");
			//Debug.Log(PrintANumber());
			//Debug.Log (LJUSB_GetLibraryVersion ());
			StartCoroutine(ConnectSyncbox());
		}
	}

	IEnumerator ConnectSyncbox(){
		while(!isUSBOpen){
			string usbOpenFeedback = Marshal.PtrToStringAuto (OpenUSB());
			Debug.Log(usbOpenFeedback);
			if(usbOpenFeedback != "didn't open USB..."){
				isUSBOpen = true;
			}

			yield return 0;
		}

		//Debug.Log(Marshal.PtrToStringAuto (CloseUSB()));
		StartCoroutine (Pulse ());
	}
	
	// Update is called once per frame
	void Update () {
		if(ExperimentSettings_CoinTask.isSyncbox){
			if (!ShouldPulse) {
				GetInput ();
			}
		}
	}

	void GetInput(){
		if (Input.GetKey (KeyCode.DownArrow)) {
			ToggleOn();
		}
		else{
			ToggleOff ();
		}
		if(Input.GetKeyDown(KeyCode.S)){
			SetSyncPulse();
			//SetStimPulse();
		}
	}


	//LOGGING
	void LogSYNCBOX(long time, bool isOn){
		if(ExperimentSettings_CoinTask.isLogging){
			if(isOn){
				exp.eegLog.Log(time, exp.eegLog.GetFrameCount(), "SYNCBOX ON");
			}
			else{
				exp.eegLog.Log(time, exp.eegLog.GetFrameCount(), "SYNCBOX OFF");
			}
		}
	}


	void ToggleOn(){
		if (!isToggledOn) {
			DownCircle.color = DownColor;
			Debug.Log(Marshal.PtrToStringAuto (TurnLEDOn()));
		}
		isToggledOn = true;
	}

	void ToggleOff(){
		if (isToggledOn) {
			DownCircle.color = UpColor;
			Debug.Log(Marshal.PtrToStringAuto (TurnLEDOff()));
		}
		isToggledOn = false;
	}

	//ex: a 10 ms pulse every second — until the duration is over...
	void SetSyncPulse(){
		//Debug.Log(Marshal.PtrToStringAuto (SyncPulse()));
		Debug.Log (SyncPulse ());
	}

	void SetStimPulse(){
		Debug.Log(Marshal.PtrToStringAuto (StimPulse (1.0f, 10, false)));
	}

	IEnumerator Pulse (){
		yield return new WaitForSeconds(TCP_Config.numSecondsBeforeAlignment);
		while (true) {
			if(ShouldPulse){
				ToggleOn();
				LogSYNCBOX(GameClock.SystemTime_Milliseconds, true);
				yield return new WaitForSeconds(PulseOnSeconds);
				ToggleOff();
				LogSYNCBOX(GameClock.SystemTime_Milliseconds, false);
				yield return new WaitForSeconds(PulseOffSeconds);
			}
			else{
				yield return 0;
			}
		}
	}

	void OnApplicationQuit(){
		Debug.Log(Marshal.PtrToStringAuto (CloseUSB()));
	}

}
