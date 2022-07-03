using UnityEngine;

using float3 = Unity.Mathematics.float3;
using quaternion = Unity.Mathematics.quaternion;
using math = Unity.Mathematics.math;

namespace Common
{
    public static class HelperMethods
    {
        //Unity Engine Classic
        public static Vector3 GenerateRandomPositionVector3(float xInterval,float yInterval, float zInterval, bool boidsLiveIn3DSpace)
        {
            float randomX = UnityEngine.Random.Range(-xInterval, xInterval);
            float randomY = UnityEngine.Random.Range(-yInterval,yInterval) ;
            float randomZ = boidsLiveIn3DSpace?UnityEngine.Random.Range(-zInterval, zInterval):0f;
            return new Vector3(randomX, randomY, randomZ);
        }

        public static Quaternion GenerateRandomRotation(bool boidsLiveIn3DSpace)
        {
            return Quaternion.Euler(
                UnityEngine.Random.Range(-360f, 360f),
                boidsLiveIn3DSpace ? UnityEngine.Random.Range(-360f, 360f) : 90f,
                boidsLiveIn3DSpace?Random.Range(-360f, 360f):0f
            );
        }

        public static Vector3 GetRandomVelocityVector3(bool boidsLiveIn3DSpace, float initialSpeed)
        {
            float randX = Random.Range(-1f, 1f);
            float randY = Random.Range(-1f, 1f);
            float randZ = boidsLiveIn3DSpace ? Random.Range(-1f, 1f) : 0f;
            return  new Vector3(randX, randY, randZ).normalized * initialSpeed;
        }


        //Unity Dots
       public static float3 GenerateRandomPositionFloat3(bool boidsLiveIn3DSpace, float xInterval, float yInterval, float zInterval)
        {
            float randomX = UnityEngine.Random.Range(-xInterval, xInterval);
            float randomY = UnityEngine.Random.Range(-yInterval, yInterval);
            float randomZ = boidsLiveIn3DSpace ? UnityEngine.Random.Range(-zInterval, zInterval) : 0f;
            return new float3(randomX, randomY, randomZ);

        }

        public static quaternion GenerateRandomRotationq(bool boidsLiveIn3DSpace)
        {
            return quaternion.Euler(
                UnityEngine.Random.Range(-360f, 360f),
                boidsLiveIn3DSpace ? UnityEngine.Random.Range(-360f, 360f): 90f,
                boidsLiveIn3DSpace ? UnityEngine.Random.Range(-360f, 360f) : 0f
            ) ;
        }

        public static float3 GetRandomVelocityFloat3(bool boidsLiveIn3DSpace,float initialSpeed)
        {
            float randomX = UnityEngine.Random.Range(-1f, 1f);
            float randomY = UnityEngine.Random.Range(-1f, 1f);
            float randomZ = boidsLiveIn3DSpace ? UnityEngine.Random.Range(-1f, 1f) : 0f;
            return math.normalize(new float3(randomX, randomY, randomZ)) * initialSpeed;
        }

    }
}