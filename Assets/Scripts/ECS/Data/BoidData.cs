using Unity.Entities;
using Unity.Mathematics;

namespace ECS{
    public struct BoidData : IComponentData
    {
        //Data of a boid
        public float3 velocity;
        public float3 accelerationForce;
        public float maxSpeed;
        //Limiting length of the steering vector
        public float maxForce;
        //To scale sperationm allignment and cohesion
        public float separationScale;
        public float alignmentScale;
        public float cohesionScale;
        public bool allowAdditionalBehaviour;
        //The radius a boid may consider
        public float perceptionRadiusSquared; 
        public bool livesIn3DSpace;
        //Collision Detection
        //public float collisionAvoidanceScale;
        //public float castSphereRadius;
        //public float collisionRayDistance; 
        public float gravityFactor;
        //Allowed space
        public float xInterval;
        public float yInterval;
        public float zInterval;
        //NOT IMPLEMENTED: FOOD AND PHEREMONE
        //public float secondsToRegeneratePheromon;
        //public float pheromoneRayDistance;
        //public float pheromoneScale;
        //randomness
        public float randomScale;
    }
}