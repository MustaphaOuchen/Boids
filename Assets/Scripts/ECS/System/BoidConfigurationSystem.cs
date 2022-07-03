using Unity.Entities;
using Unity.Jobs;

namespace ECS
{
    public partial class BoidConfigurationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            //capture local variable
            BoidsManagerDOTS boidsManager = BoidsManagerDOTS.Instance as BoidsManagerDOTS;     
            float xInterval = boidsManager.xInterval;
            float yInterval = boidsManager.yInterval;
            float zInterval = boidsManager.zInterval;
            bool boidLivesIn3DSpace = boidsManager.boidLivesIn3DSpace; 
            float initialSpeed = boidsManager.initialSpeed;
            float maxSpeed = boidsManager.maxSpeed;
            float maxForce = boidsManager.maxForce;
            float separationScale = boidsManager.separationScale;
            float alignmentScale = boidsManager.alignmentScale;
            float cohesionScale = boidsManager.cohesionScale;
            float randomScale = boidsManager.randomScale;
            float gravityFactor = boidsManager.gravityFactor;
            bool allowAdditionalBehaviour = boidsManager.allowAdditionalBehaviour;
            float castSphereRadius = boidsManager.castSphereRadius;
            float collisionAvoidanceScale = boidsManager.collisionAvoidanceScale;
            float collisionRayDistance = boidsManager.collisionRayDistance;
            //SKIPPING PHEROMONE AND FOOD
            float perceptionRadius = boidsManager.perceptionRadius;
            //declare job
            Entities.ForEach((ref BoidData boidData) =>
            {
                boidData.separationScale = separationScale;
                boidData.alignmentScale = alignmentScale;
                boidData.cohesionScale = cohesionScale;
                boidData.maxSpeed = maxSpeed;
                //boidData.initialSpeed = initialSpeed;
                boidData.maxForce = maxForce;
                boidData.perceptionRadiusSquared = perceptionRadius * perceptionRadius;
                //boidData.collisionAvoidanceScale = collisionAvoidanceScale;
                //boidData.castSphereRadius = castSphereRadius;
                //boidData.collisionRayDistance = collisionRayDistance;
                
                boidData.allowAdditionalBehaviour = allowAdditionalBehaviour;
                if (allowAdditionalBehaviour)
                {
                    boidData.gravityFactor = gravityFactor;
                    boidData.randomScale = randomScale;
                }

            }).Schedule();
        }
    }
}