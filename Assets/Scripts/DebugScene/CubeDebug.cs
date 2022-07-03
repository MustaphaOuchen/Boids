using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CubeDebug : MonoBehaviour
{
    int cellsWidth = 4;


    int currentBucket = -1;

    public float xInterval = 40f;
    public float yInterval = 40f;
    public float zInterval = 40f;


    float conversionFactor = 1f / 20f; //20f = cellSize


    private int p1 = 73856093;
    private int p2 = 19349663; //No prime ???
    private int p3 = 83492791;

    //
    public float cellSize = 10f;
    public float width = 4;
    public int numberOfBuckets = 64;


    private void Awake()
    {
        float min = -xInterval;
        float max = xInterval;
        cellSize = 10f; //each cube length is 20 Units
        width = (max - min) / cellSize; //width must be 4???
        numberOfBuckets = (int)(width * width * width); ////Since we are in 3D
        conversionFactor = 1 / cellSize;

        //Debug.Log($"min: {min},max:{max},cellSize: {cellSize},width: {width},numberOfBuckets: {numberOfBuckets}");
    }

    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 position = transform.position;
        //
        ////position *= conversionFactor;
        //
        ////Hash
        //
        ////int bucket = (Mathf.FloorToInt(position.x* conversionFactor) + (Mathf.FloorToInt(position.y*conversionFactor)) + (Mathf.FloorToInt(position.z*conversionFactor))) * (int)cellsWidth;
        //
        ////int bucket = ((int)Mathf.Floor(position.x) + ((int)Mathf.Floor(position.y)) + ((int)Mathf.Floor(position.z))) * (int)cellsWidth;
        //
        //int bucket = ((Mathf.FloorToInt(position.x * conversionFactor)) + (Mathf.FloorToInt(position.y * conversionFactor))) * cellsWidth;
        //
        ////int bucket = (int) (position.x * conversionFactor + position.y * conversionFactor) * cellsWidth;
        //
        //if (bucket != currentBucket)
        //{
        //    currentBucket = bucket;
        //    Debug.Log($"I am in bucket: {bucket}");
        //}
    }


    public int Hash(Vector3 position)
    {
        //Verschiebe positiv
        //position += new Vector3(xInterval, yInterval, zInterval);



        //Vector3 position = transform.position;
        //return (((Mathf.FloorToInt(position.x * conversionFactor)) + (Mathf.FloorToInt(position.y * conversionFactor))) * cellsWidth).ToString();

        
        //Higher probability of collision
        //position *= conversionFactor;


        //return (((Mathf.FloorToInt(position.x)  * p1) ^ (Mathf.FloorToInt(position.y) * p2) ^ (Mathf.FloorToInt(position.z) * p3))* (int) cellsWidth).ToString();

        //Grid cell
        //return (((Mathf.FloorToInt(position.x) ) + (Mathf.FloorToInt(position.y)) + (Mathf.FloorToInt(position.z) ))* (int) cellsWidth).ToString();

        //return (((Mathf.FloorToInt(position.x)) + (Mathf.FloorToInt(position.y)) + (Mathf.FloorToInt(position.z)))).ToString();

        //position += new Vector3(xInterval, yInterval, zInterval);

        //string str = $"{Mathf.FloorToInt(position.x )} : {Mathf.FloorToInt(position.y)} : {Mathf.FloorToInt(position.z)}";
        //return str;


        //standard
        // return (((Mathf.FloorToInt(position.x) * p1) ^ (Mathf.FloorToInt(position.y) * p2) ^ (Mathf.FloorToInt(position.z) * p3))).ToString();

        //Nur y mit cellwidth multiplizieren???
        //return (((Mathf.FloorToInt(position.x)) + (Mathf.FloorToInt(position.y)* cellsWidth) + (Mathf.FloorToInt(position.z) ))).ToString();

        if (numberOfBuckets == 0)
        {
            numberOfBuckets = 64;
        }

        return ((Mathf.FloorToInt(position.x) * p1) ^ (Mathf.FloorToInt(position.y) * p2) ^ (Mathf.FloorToInt(position.z) * p3)) % numberOfBuckets;

    }

    


    //String hash for zero collisions

    public string StringHash(Vector3 position)
    {

        position *= conversionFactor;
        string hashStr = $"{Mathf.FloorToInt(position.x)}{Mathf.FloorToInt(position.y)}{Mathf.FloorToInt(position.z)}";
        return hashStr;

    }


    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0, 0.3f, 1, 0.2f);
        //Gizmos.DrawCube(Vector3.zero, new Vector3(xInterval * 2, yInterval * 2, zInterval * 2));

#if UNITY_EDITOR

        Gizmos.color = Color.white;
        float conversionFactor = 1f / (2f * 3f);
        string debugText = Common.SpatialHasher.HashPositionVector3(transform.position, 2197, conversionFactor).ToString();//StringHash(transform.position).ToString();
       
        Handles.Label(transform.position, debugText);


#endif

    }

}
