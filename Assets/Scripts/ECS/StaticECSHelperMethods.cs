using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;

namespace ECS
{
    /// <summary>
    /// Class to maximize code usage of common methods which do not exists in the DOTS/ECS API.
    /// Static methods can be called in the For.Each-Lambda - Construct
    /// </summary>
    public static class StaticECSHelperMethods
    {
        //Vector operations
        //Same as Vector3.ClampMagnitude since Unity.Mathematic.math does not have this method
        public static float3 ClampMagnitude(float3 vector, float maxLength)
        {

            if ((vector.x * vector.x + vector.y * vector.y + vector.z * vector.z) > maxLength * maxLength)
                return math.normalize(vector) * maxLength;
            return vector;
        }
    }
}