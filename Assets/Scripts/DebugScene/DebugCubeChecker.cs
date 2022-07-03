using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCubeChecker : MonoBehaviour
{


    public float xInterval = 40f;
    public float yInterval = 40f;
    public float zInterval = 40f;
    public float cellSize = 10f;     //each cube length is 20 Units, klappt nur bei geraden zahlen !

    public GameObject SomethingToSpawn;

    // Start is called before the first frame update
    void Start()
    {




        float min = -xInterval;
        float max = xInterval;
        float width = (max - min) / cellSize; //width must be 4???
        float numberOfBuckets = width * width * width;


        float scaller = 1 - (cellSize / (max - min));
        float inverse = 1 / scaller;
        //Debug.Log(inverse);
        inverse = (float)System.Math.Round((double)inverse, 2);

        float incrementor = cellSize * inverse;
        //float incrementor = cellSize * (1/scaller);
        int amount = 0;
        for (float x = min; x <= max; x += incrementor)
        {
            for (float y = min; y <= max; y += incrementor)
            {
                for (float z = min; z <= max; z += incrementor)
                {
                    //Vector3 center = new Vector3(x, y, z) * 0.75f;

                    Vector3 center = new Vector3(x, y, z) * scaller;

                    //Gizmos.DrawWireCube(center, new Vector3(cellSize, cellSize, cellSize));
                    GameObject gb = Instantiate(SomethingToSpawn,center,Quaternion.identity,this.transform);
                    gb.AddComponent<CubeDebug>();
                    amount++;
                  
                }

            }
        }


        Debug.Log($"Spawned: {amount}");



        //Check how many dupliacte space we have
        var children = gameObject.GetComponentsInChildren<CubeDebug>();

 

        Dictionary<string,int> collisionCounter = new Dictionary<string,int>();

        foreach (var child in children)
        {
            string hashOne = child.StringHash(child.transform.position).ToString();

            if (collisionCounter.ContainsKey(hashOne))
                collisionCounter[hashOne]++;
            else collisionCounter[hashOne] = 1;


  
        }

        int sum = 0;
        foreach (var key in collisionCounter.Keys)
        {
            Debug.Log(key + ":" + collisionCounter[key]);
            
            if(collisionCounter[key] > 1)
                sum ++;
        }

        Debug.Log("The sum:" + sum); //96
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0, 0.3f, 1, 0.2f);
        //Gizmos.DrawCube(transform.position, new Vector3(xInterval * 2, yInterval * 2, zInterval * 2));

        //Using double for accuracy?


        float min = -xInterval;
        float max = xInterval;

        float width = (max - min) / cellSize; //width must be 4???
        float numberOfBuckets = width * width * width; ////Since we are in 3D
        //Debug.Log(width);

        // for(int i = 0; i < numberOfBuckets; i++)
        // {
        //     Vector3 center = new Vector3(min,min,min) * 0.75f ; //Adjust center correctly
        //
        //     Vector3 center2 = new Vector3(min, min, min + cellSize*1.333f) * 0.75f;
        //
        //       Gizmos.color = Color.red;
        //     Gizmos.DrawWireCube(center,new Vector3(cellSize,cellSize,cellSize));
        //       Gizmos.color = Color.blue;
        //       Gizmos.DrawWireCube(center2, new Vector3(cellSize, cellSize, cellSize));
        //
        //   }
        //
        Gizmos.color = Color.red;

        float scaller = 1- (cellSize / (max - min));
        float inverse = 1 / scaller;
        //Debug.Log(inverse);
        inverse = (float) System.Math.Round((double)inverse, 2);

        float incrementor = cellSize * inverse;
        //float incrementor = cellSize * (1/scaller);
        int n = 0;
        for (float x = min; x <= max; x += incrementor)
        {
            for (float y = min; y <= max; y += incrementor)
            {
                for (float z = min; z <= max; z += incrementor)
                {
                    //Vector3 center = new Vector3(x, y, z) * 0.75f;

                    Vector3 center = new Vector3(x, y, z) * scaller;

                    Gizmos.DrawWireCube(center, new Vector3(cellSize, cellSize, cellSize));
                    n += 1;
                }

            }
        }
    }
}
