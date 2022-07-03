//Modified from:
// Copyright (c) 2016 Unity Technologies. MIT license - license_unity.txt
// #NVJOB FPS counter and graph. MIT license - license_nvjob.txt
// #NVJOB FPS counter and graph V2.0 - https://nvjob.github.io/unity/nvjob-fps-counter-and-graph
// #NVJOB Nicholas Veselov (independent developer) - https://nvjob.github.io


using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public class FPSCounter : MonoBehaviour
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    [Header("Settings")]
    public float timeUpdate = 1.0f;
   // public int highestPossibleFPS = 300;
   // public Color graphColor = new Color(1, 1, 1, 0.5f);
    public bool logWrite = true;

    //--------------

    //GameObject counter, graph;
    //Transform graphTr;
    Vector3Int allFps;
    // Text counterText;
    //float ofsetX;
    //int lineCount;

    //--------------

    //static WaitForSeconds stGraphUpdate;
    //static GameObject[] stLines;
    //static int stNumLines;


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    //Seconds until exit
    private float secondsUntilExit = 60f * 6f;


    void Awake()
    {
        //--------------

        //Application.targetFrameRate = highestPossibleFPS;
        //CreateCounter();

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void OnEnable()
    {
        //--------------

        //StartCoroutine(DrawGraph());

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void OnApplicationQuit()
    {
        //--------------

        if (logWrite == true) StFPS.LogWrite();

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    void Update()
    {
        //--------------

        // StFPS.Counter(Time Update).x - min fps
        // StFPS.Counter(Time Update).y - avg fps
        // StFPS.Counter(Time Update).z - max fps

        secondsUntilExit -= Time.unscaledDeltaTime;
        allFps = StFPS.Counter(timeUpdate);

        if (secondsUntilExit < 0)
        {
            
            Application.Quit();
        }


      //  counterText.text = "MIN " + allFps.x.ToString() + " | AVG " + allFps.y.ToString() + " | MAX " + allFps.z.ToString();
      //
      //  //-------------- 
      //
      //  if (Input.GetKeyDown(KeyCode.F1)) // Hide Counter
      //  {
      //      counter.SetActive(!counter.activeSelf);
      //      graph.SetActive(!graph.activeSelf);
      //  }
      //
        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


public static class StFPS
{
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    static List<float> fpsBuffer = new List<float>();
    static float fpsB, timeCounter;
    static Vector3Int fps;
    static List<Vector3Int> logWrite = new List<Vector3Int>();


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static Vector3Int Counter(float timeUpdate)
    {
        //--------------

        int fpsBCount = fpsBuffer.Count;

        //Falls eine Sekunde nicht vergangengen ist
        if (timeCounter <= timeUpdate)
        {
            timeCounter += Time.unscaledDeltaTime;
            fpsBuffer.Add(1.0f / Time.unscaledDeltaTime); //The Inverse
           // Debug.Log(1.0f / Time.smoothDeltaTime);
        }
        else
        //Falls eine Sekunde vergangen ist, berechne die min,max,avg
        {
            fps.x = Mathf.RoundToInt(fpsBuffer.Min());
            fps.z = Mathf.RoundToInt(fpsBuffer.Max());
            for (int f = 0; f < fpsBCount; f++) fpsB += fpsBuffer[f];
            fpsBuffer = new List<float> { 1.0f / Time.unscaledDeltaTime };
            //Durchschnitt
            fpsB = fpsB / fpsBCount;
            fps.y = Mathf.RoundToInt(fpsB);
            fpsB = timeCounter = 0;
            if (Time.timeScale == 1)
            {
                logWrite.Add(fps); //fps as vector
                //Debug.Log(fpsBuffer);
                //Debug.Log(fps);
            }
            else logWrite.Add(Vector3Int.zero);
        }

        if (Time.timeScale == 1 && fps.y > 0) return fps;
        else return Vector3Int.zero;

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public static void LogWrite()
    {
        //--------------

        string filePath = Directory.GetCurrentDirectory() + "/fpslog/";
        if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
        //string date = System.DateTime.Now.ToString("_yyyy.MM.dd_HH.mm.ss");
        //filePath = filePath + "log" + date + ConventionalBoid.BoidsManager.Instance.boidMatesAlgorithm +"_"+ ConventionalBoid.BoidsManager.Instance.amountBoids+ ".csv";

        //ConventionalBoid.BoidsManager boidsManager = ConventionalBoid.BoidsManager.Instance as ConventionalBoid.BoidsManager;
        //ECS.BoidsManagerDOTS boidsManager = ECS.BoidsManagerDOTS.Instance as ECS.BoidsManagerDOTS;
        Common.BoidsManagerBase boidsManager = Common.BoidsManagerBase.Instance;
        

        filePath = filePath + boidsManager.boidMatesAlgorithm + "_" + boidsManager.amountBoids + ".csv";
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine("MIN;AVG;MAX");
        for (int i = 0; i < logWrite.Count; ++i) writer.WriteLine(logWrite[i].x + ";" + logWrite[i].y + ";" + logWrite[i].z);
        writer.Flush();
        writer.Close();

        //--------------
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}