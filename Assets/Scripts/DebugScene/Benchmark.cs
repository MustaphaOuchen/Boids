using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
/// <summary>
/// C# Script which is intended to benchmark functionalities
/// </summary>
public class Benchmark : MonoBehaviour
{
    //https://sharpcoderblog.com/blog/unity-3d-how-to-use-profiler-to-optimize-your-code
    //https://github.com/Unity-Technologies/UnityCsReference/blob/master/Runtime/Export/Math/Vector3.cs


    //Check this out for boids 
    //https://www.youtube.com/watch?v=mNZq0RhM-98

    List<Vector3> randomPoints = new List<Vector3>();

    public int amountPoints = 10000;
    public float xIntervall = 20f;
    public float yIntervall = 20f;
    public float zIntervall = 20f;


    private float someTestDistance = 4f;
    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < amountPoints; i++)
        {

            Vector3 randomPos = new Vector3(Random.Range(-xIntervall, xIntervall), Random.Range(-yIntervall, yIntervall), Random.Range(-zIntervall, zIntervall));
            randomPoints.Add(randomPos);
        }




    }

    // Update is called once per frame
    void Update()
    {
        Profiler.BeginSample("RegularDistance");
        CompareDistances();
        Profiler.EndSample();
        Profiler.BeginSample("SquaredMagnitued");
        CompareDistancesSquared();
        Profiler.EndSample();
    }



    private void CompareDistances()
    {
        foreach (var vec1 in randomPoints)
        {
            foreach (var vec2 in randomPoints)
            {
                if (vec1 == vec2) continue;


                float distance = Vector3.Distance(vec1, vec2);
                if (distance > someTestDistance)
                {
                    //Do nothing
                }

            }
        }
    }

    private void CompareDistancesSquared()
    {
        foreach (var vec1 in randomPoints)
        {
            foreach (var vec2 in randomPoints)
            {
                if (vec1 == vec2) continue;


                float sqrDistance = Vector3.SqrMagnitude(vec2 - vec1);

                if (sqrDistance > someTestDistance * someTestDistance)
                {
                    //Do nothing
                }
            }
        }
    }

}
