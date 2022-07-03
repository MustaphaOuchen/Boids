using UnityEngine;

namespace ConventionalBoid
{
    //Original Idea by SebLague https://github.com/SebLague/Boids/tree/master
    //Discussion and explanation of the golden spiral method
    //https://stackoverflow.com/questions/9600801/evenly-distributing-n-points-on-a-sphere/44164075#44164075

    //3D sunflower spiral algorithm

    //https://bendwavy.org/pack/pack.htm

    //https://perswww.kuleuven.be/~u0017946/publications/Papers97/art97a-Saff-Kuijlaars-MI/Saff-Kuijlaars-MathIntel97.pdf



    //Here a simple optimization by offsetting is explained!!!
    //http://extremelearning.com.au/how-to-evenly-distribute-points-on-a-sphere-more-effectively-than-the-canonical-fibonacci-lattice/


    public static class BoidCollisionHelper
    {
        const int numberOfPointsToGenerate = 30; 
        public static readonly Vector3[] unitSpherePoints;

        //For avoidance
        //Evenly distributing n points on a sphere on a unit sphere
        static BoidCollisionHelper()
        {
            //To ray a sphere around the boid
            unitSpherePoints = new Vector3[BoidCollisionHelper.numberOfPointsToGenerate];


            //By SebLague
            //phi/inclination is the angle from the northpole
            //theta/azimuth is the longitude

            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2; //Irrational number recommened which is used to generate the points
            float angleIncrement = Mathf.PI * 2 * goldenRatio; //turning fraction of a circle, growth factor


            //Unit sphere
            for (int i = 0; i < numberOfPointsToGenerate; i++)
            {
                float iterationFraction = (float)i / numberOfPointsToGenerate;
                float inclination = Mathf.Acos(1 - 2 * iterationFraction); //phi/r, using inverse for uniformity
                float azimuth = angleIncrement * i; //theta = azimuth
                //Calculating points/projecting sphere coordinates
                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);
                unitSpherePoints[i] = new Vector3(x, y, z);

            }


            


            //Better packing distance
            /*
         float epsilon;

         //c# 8 does not allow that
          //   switch (numberOfPointsToGenerate)
          // {
          //      case >= 600000:
          //          epsilon = 214f;
          //          break;
          //
          //
          //  }
          if (numberOfPointsToGenerate >= 600000f)
            {
                epsilon = 214f;
            }
          else if (numberOfPointsToGenerate >= 400000f)
            {
                epsilon = 75f;
            }
          else if (numberOfPointsToGenerate >= 11000f )
            {
                epsilon = 27f;
            }
          else if (numberOfPointsToGenerate >= 890f)
            {
                epsilon = 10f;
            }
          else if(numberOfPointsToGenerate >= 177f)
            {
                epsilon = 3.33f;
            }
          else if(numberOfPointsToGenerate >= 24f)
            {
                epsilon = 1.33f;
            }
            else
            {
                epsilon = 0.33f;
            }


            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2; //Irrational number recommened which is used to generate the points
            //float angleIncrement = Mathf.PI * 2 * goldenRatio; //turning fraction of a circle, growth factor


            //Unit sphere

            //phi is the angle from the northpole
            //theta is the longitude

            for (int i = 0; i < numberOfPointsToGenerate; i++)
            {
                float iterationFraction = ((float)i + epsilon) / ((numberOfPointsToGenerate-1)+ 2 * epsilon) ;
                float phi = Mathf.Acos(1 - 2 * iterationFraction); //phi/r, using inverse for uniformity
                //float theta = angleIncrement * i; //theta = azimuth
                //Calculating points/projecting coordinates
                float theta = 2 * Mathf.PI * (float)i / goldenRatio; //Basically the angle Increment
                //float theta = 2 * Mathf.PI * (float)i * goldenRatio;
                float x = Mathf.Sin(phi) * Mathf.Cos(theta);
                float y = Mathf.Sin(phi) * Mathf.Sin(theta);
                float z = Mathf.Cos(phi);
                unitSpherePoints[i] = new Vector3(x, y, z);

            }

            */





            //Using Unity API method instead

            /*
            for (int i = 0; i <numberOfPointsToGenerate; i++)
            {
                unitSpherePoints[i] = Random.insideUnitSphere;
            }*/



            //Debug
            /*
            for (int i = 0; i < numViewDirections; i++)
            {
                Vector3 dir = directions[i];
                float lengthOfDir = dir.magnitude;

                Debug.Log($"Generated Dot on Unit Sphere: {dir} --> length: {lengthOfDir}");


            }*/
        }

    }
}