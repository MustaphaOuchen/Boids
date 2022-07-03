using UnityEngine;

namespace Common
{

    public class BoidSettingsWithBehaviourBase : MonoBehaviour
    {
        //Allowed space
        [Header("World Space Boid Settings")]
        public float xInterval = 40f;
        public float yInterval = 40f;
        public float zInterval = 40f;
        public bool boidLivesIn3DSpace = true;
        [Header("Boid Settings")]
         public float initialSpeed = 2.5f;
         public float maxSpeed = 10.0f;
        //Limiting length of the steering vector
         public float maxForce = 0.3f;
        //To scale sperationm allignment and cohesion
        public float separationScale = 1f;
        public float alignmentScale = 0.6f;
        public float cohesionScale = 0.6f;
        //randomization
        public float randomScale = 1.5f;
        public float gravityFactor = 0.000981f;
        //public float perceptionRadius = 3f;
        //Additional behavior
        public bool allowAdditionalBehaviour = false;
        //Casting a sphere 
        public float castSphereRadius = 3f;
        //Collisions
        public float collisionAvoidanceScale = 12f;
        public float collisionRayDistance = 3f;
        //Pheromone
        public float secondsToRegeneratePheromone = 3f; 
        public float pheromoneRayDistance = 3f; 
        public float pheromoneScale = 1f; 
        public GameObject pheromonePrefab; 
        //Food
        public float foodRayDistance = 3f; //Boid Has Access to food
        public float lifePointSummand = 10f; //How much food increases lifetime
    }
}