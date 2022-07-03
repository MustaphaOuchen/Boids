using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Rotation = Unity.Transforms.Rotation;

namespace ECS
{
    public partial class BoidRotationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            //capture local variable
            float deltaTime = Time.DeltaTime;
            //declare job
            Entities.ForEach( (ref Rotation rotation, in BoidData boidData) =>
            { 
                if (boidData.livesIn3DSpace)
                {
                    //Perform rotation
                    quaternion lookRotation = quaternion.LookRotation(boidData.velocity, math.up());
                    //Setting and smoothing out
                    rotation.Value = math.slerp(rotation.Value, lookRotation, deltaTime);
                }
                else
                {
                    //2D has different rotation
                    // rotate that vector by 90 degrees around the Z axis
                    float3 rotatedVector = math.mul(quaternion.Euler(0, 0, 90) ,boidData.velocity);                   
                    quaternion targetRotation = quaternion.LookRotation(forward: boidData.velocity, up: rotatedVector);
                    //Smoothing out
                    rotation.Value = math.slerp(rotation.Value, targetRotation, deltaTime);
                }
            }).ScheduleParallel();
        }
    }
}