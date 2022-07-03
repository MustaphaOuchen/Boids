using UnityEngine;

namespace Common
{
    public class BoidsManagerBase : BoidSettingsWithBehaviourBase
    {

        [Header("Boid Settings Only Manager")]
        public float perceptionRadius = 3f;
        public int amountBoids = 200;

        [Header("Analysis")]
        [SerializeField]protected bool activateDebugBreak = false;
        [SerializeField] protected float secondsUntilDebugBreak = 5f * 60f;

        [Header("Boid Prefab")]
        //The GameObject which is used as blueprint for the boids
        [SerializeField] protected GameObject boidPrefab;

        [Header("Algorithmic Settings")]
        //Configure Algorithm in editor
        public BoidMatesAlgorithm boidMatesAlgorithm = BoidMatesAlgorithm.Bruteforce;

        [Header("Realtime settings configuration")]
        [SerializeField] protected bool disableRealtimeConfiguration = false;
        
        //Singleton
        public static BoidsManagerBase Instance;


        //For Hashing
        public float conversionFactor;

        //Called before start
        protected void Awake()
        {

            //Initializing Lazy Unity Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}