﻿using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using LabJack.LabJackUD;
using LabJack;
public class SyncboxControl : MonoBehaviour
{
    Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

#if GERMAN
	[DllImport ("FreiburgSyncboxPlugin")]
	private static extern IntPtr OpenUSB();
	[DllImport ("FreiburgSyncboxPlugin")]
	private static extern IntPtr CloseUSB();
	[DllImport ("FreiburgSyncboxPlugin")]
	private static extern IntPtr TurnLEDOn();
	[DllImport ("FreiburgSyncboxPlugin")]
	private static extern IntPtr TurnLEDOff();

#else
    [DllImport("ASimplePlugin")]
    private static extern IntPtr OpenUSB();
    [DllImport("ASimplePlugin")]
    private static extern IntPtr CloseUSB();
    [DllImport("ASimplePlugin")]
    private static extern IntPtr TurnLEDOn();
    [DllImport("ASimplePlugin")]
    private static extern IntPtr TurnLEDOff();
    //[DllImport ("ASimplePlugin")]
    //private static extern long SyncPulse();
    //[DllImport ("ASimplePlugin")]
    //private static extern IntPtr StimPulse(float durationSeconds, float freqHz, bool doRelay);
#endif

    public bool ShouldSyncPulse = true;
    public float PulseOnSeconds;
    public float PulseOffSeconds;

    public bool isUSBOpen = false;

    //u3 specific
    private U3 u3;
    double dblDriverVersion;
    LJUD.IO ioType = 0;
    LJUD.CHANNEL channel = 0;

    //SINGLETON
    private static SyncboxControl _instance;

    public static SyncboxControl Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {

        if (_instance != null)
        {
            UnityEngine.Debug.Log("Instance already exists!");
            Destroy(transform.gameObject);
            return;
        }
        _instance = this;

    }

    // Use this for initialization
    void Start()
    {

        //Read and display the UD version.
        //dblDriverVersion = LJUD.GetDriverVersion();
        //UnityEngine.Debug.Log(dblDriverVersion);
        if (Config_CoinTask.isSyncbox)
        {
            StartCoroutine(ConnectSyncbox());
        }
    }




    IEnumerator TurnOnOff()
    {
        LJUD.eDO(u3.ljhandle, 0, 1);
        yield return new WaitForSeconds(2f);
        LJUD.eDO(u3.ljhandle, 0, 0);
        yield return null;
    }
    public void ShowErrorMessage(LabJackUDException e)
    {
        UnityEngine.Debug.Log("ERROR: " + e.ToString());


    }


    IEnumerator ConnectSyncbox()
    {

        string connectionError = "";
        while (!isUSBOpen)
        {
            /*
			string usbOpenFeedback = Marshal.PtrToStringAuto (OpenUSB());
			UnityEngine.Debug.Log(usbOpenFeedback);
            */
            try
            {

                u3 = new U3(LJUD.CONNECTION.USB, "0", true); // Connection through USB
                                                             //Start by using the pin_configuration_reset IOType so that all
                                                             //pin assignments are in the factory default condition.


            }
            catch (LabJackUDException e)
            {
                connectionError = e.ToString();
                ShowErrorMessage(e);
            }
            //   StartCoroutine("TurnOnOff");
            UnityEngine.Debug.Log("connectionerror " + connectionError);
            if (connectionError == "")
            {
                isUSBOpen = true;
            }
            else
            {
                exp.trialController.ConnectionText.text = "Please connect Syncbox and Restart";
            }

            yield return 0;
        }

        StartCoroutine(RunSyncPulseManual());
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    void GetInput()
    {
        //use this for debugging if you'd like
    }

    float syncPulseDuration = 0.05f;
    float syncPulseInterval = 1.0f;
    /*	IEnumerator RunSyncPulse(){
            Stopwatch executionStopwatch = new Stopwatch ();
            while (ShouldSyncPulse) {
                executionStopwatch.Reset();
                SyncPulse(); //executes pulse, then waits for the rest of the 1 second interval
                executionStopwatch.Start();
                long syncPulseOnTime = SyncPulse();
                LogSYNCOn(syncPulseOnTime);
                while(executionStopwatch.ElapsedMilliseconds < 1500){
                    yield return 0;
                }
                executionStopwatch.Stop();
            }
        }*/

    IEnumerator RunSyncPulseManual()
    {
        float jitterMin = 0.1f;
        float jitterMax = syncPulseInterval - syncPulseDuration;

        Stopwatch executionStopwatch = new Stopwatch();

        while (ShouldSyncPulse)
        {
            executionStopwatch.Reset();


            float jitter = UnityEngine.Random.Range(jitterMin, jitterMax);//syncPulseInterval - syncPulseDuration);
            yield return StartCoroutine(WaitForShortTime(jitter));

            ToggleLEDOn();
            yield return StartCoroutine(WaitForShortTime(syncPulseDuration));
            ToggleLEDOff();

            float timeToWait = (syncPulseInterval - syncPulseDuration) - jitter;
            if (timeToWait < 0)
            {
                timeToWait = 0;
            }

            yield return StartCoroutine(WaitForShortTime(timeToWait));

            executionStopwatch.Stop();
        }
    }

    //return microseconds it took to turn on LED
    void ToggleLEDOn()
    {


        LJUD.eDO(u3.ljhandle, 0, 1);
        LogSYNCOn(GameClock.SystemTime_Milliseconds);
    }

    void ToggleLEDOff()
    {


        LJUD.eDO(u3.ljhandle, 0, 0);
        LogSYNCOff(GameClock.SystemTime_Milliseconds);

    }

    long GetMicroseconds(long ticks)
    {
        long microseconds = ticks / (TimeSpan.TicksPerMillisecond / 1000);
        return microseconds;
    }

    IEnumerator WaitForShortTime(float jitter)
    {
        float currentTime = 0.0f;
        while (currentTime < jitter)
        {
            currentTime += Time.deltaTime;
            yield return 0;
        }

    }

    void LogSYNCOn(long time)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            exp.eegLog.Log(time, exp.eegLog.GetFrameCount(), "ON"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
        }
    }

    void LogSYNCOff(long time)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            exp.eegLog.Log(time, exp.eegLog.GetFrameCount(), "OFF"); //NOTE: NOT USING FRAME IN THE FRAME SLOT
        }
    }

    void LogSYNCStarted(long time, float duration)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            exp.eegLog.Log(time, exp.eegLog.GetFrameCount(), "SYNC PULSE STARTED" + Logger_Threading.LogTextSeparator + duration);
        }
    }

    void LogSYNCPulseInfo(long time, float timeBeforePulseSeconds)
    {
        if (ExperimentSettings_CoinTask.isLogging)
        {
            exp.eegLog.Log(time, exp.eegLog.GetFrameCount(), "SYNC PULSE INFO" + Logger_Threading.LogTextSeparator + timeBeforePulseSeconds * 1000); //log milliseconds
        }
    }

    void OnDestroy()
    {
        //UnityEngine.Debug.Log(Marshal.PtrToStringAuto (CloseUSB()));
    }

    void OnApplicationQuit()
    {
        //UnityEngine.Debug.Log(Marshal.PtrToStringAuto (CloseUSB()));
        LJUD.Close();
    }

}