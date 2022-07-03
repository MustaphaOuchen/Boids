using Unity.Mathematics;
using UnityEngine;
namespace Common
{
    public static class SpatialHasher
    {
        private static readonly int p1 = 73856093;
        private static readonly int p2 = 39916801;//19349663; //No prime ???
        private static readonly int p3 = 83492791;

        public static int HashPositionFloat3(float3 position, int n, float conversionFactor)
        {
            return (int)((p1 * math.floor(position.x * conversionFactor)) + (p2 * math.floor(position.y * conversionFactor)) + (p3 * math.floor(position.z * conversionFactor)) % n);
        }

        public static int HashPositionVector3(Vector3 position, int n, float conversionFactor)
        {
            return (int)((p1 * Mathf.Floor(position.x * conversionFactor)) + (p2 * Mathf.Floor(position.y * conversionFactor)) + (p3 * Mathf.Floor(position.z * conversionFactor)) % n);
        }
    }
}