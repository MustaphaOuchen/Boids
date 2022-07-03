using Common;
using Unity.Collections;
//For data
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
using UnityEngine;

namespace ECS
{

    public class BoidsManagerDOTS : BoidsManagerBase
    {

        private World world;
        private EntityManager entityManager;
        private Entity unitEntityBoidsPrefab;


        // Start is called before the first frame update
        void Start()
        {

           //LoadJsonSettings();

           conversionFactor = 1 / (2 * perceptionRadius);
           SetupECSConversion();
           SpawnBoidsECSConversion(amountBoids);


            if (allowAdditionalBehaviour)
            {
                var movementSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BoidMovementSystemSpatialHashing>();
                movementSystem.InitRandomData();
            }
           World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BoidConfigurationSystem>().Enabled = !disableRealtimeConfiguration;

        }


        private void Update()
        {
            //Debug Break
            if (activateDebugBreak)
            {
                secondsUntilDebugBreak -= Time.unscaledDeltaTime;
                if (secondsUntilDebugBreak < 0)
                {
                    Debug.Break();
                }
            }
        }

        private void SetupECSConversion()
        {
            //Conversion converts GameObjects to Entities
            world = World.DefaultGameObjectInjectionWorld;
            entityManager = world.EntityManager;
            //Provide cache for conversion
            BlobAssetStore blobAssetStore = new BlobAssetStore();
            //world conversion setting
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(world, blobAssetStore);
            //Create entity prefab, converts gameObject with all its children
            unitEntityBoidsPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(boidPrefab, settings);
            //clear cache
            blobAssetStore.Dispose();
        }

        private void SpawnBoidsECSConversion(int aountBoids = 1)
        {

            for (int i = 0; i < aountBoids; i++)
            {
                //Instantiate entity using prefab, first one already in init step created
                Entity myEntity = entityManager.Instantiate(unitEntityBoidsPrefab);
                //Manipulate entities,these one have a prefab component
                float3 randomPosition = HelperMethods.GenerateRandomPositionFloat3(boidLivesIn3DSpace,xInterval,yInterval,zInterval);
                entityManager.SetComponentData(myEntity, new Translation { Value = randomPosition });
                //Scale must be added
                entityManager.AddComponentData(myEntity, new Scale { Value = 1f });
                //Rotation
                entityManager.SetComponentData(myEntity,new Unity.Transforms.Rotation { Value = HelperMethods.GenerateRandomRotationq(boidLivesIn3DSpace)});
                BoidData boidData = new BoidData {
                    velocity = HelperMethods.GetRandomVelocityFloat3(boidLivesIn3DSpace, initialSpeed),
                    accelerationForce = float3.zero,
                    maxSpeed = maxSpeed,
                    maxForce = maxForce,
                    separationScale = separationScale,
                    alignmentScale = alignmentScale,
                    cohesionScale = cohesionScale,
                    allowAdditionalBehaviour = allowAdditionalBehaviour,
                    perceptionRadiusSquared = perceptionRadius * perceptionRadius,
                    livesIn3DSpace = boidLivesIn3DSpace,
                    //collisionAvoidanceScale = collisionAvoidanceScale,
                    //castSphereRadius = castSphereRadius,
                    //collisionRayDistance = collisionRayDistance,
                    gravityFactor = gravityFactor,
                    xInterval = xInterval,
                    yInterval = yInterval,
                    zInterval = zInterval,
                    //secondsToRegeneratePheromon = secondsToRegeneratePheromon,
                    //pheromoneRayDistance = pheromoneRayDistance,
                    //pheromoneScale = pheromoneScale,
                    randomScale = randomScale,
                };

                entityManager.AddComponentData(myEntity, boidData);


             
                    RandomData randomData = new RandomData();
                    entityManager.AddComponentData(myEntity, randomData);
                
            }

        }


        //Json Stuff:
        //Setting class to load
        [System.Serializable]
        public class Settings
        {
            public int amountBoids;
            public string algorithm;

        }

        private void LoadJsonSettings()
        {

            string path = Application.dataPath + "/simsettings.json";
            string json = System.IO.File.ReadAllText(path);
            var configurableSettings = JsonUtility.FromJson<Settings>(json);
            string algorithmStr = configurableSettings.algorithm;
            BoidMatesAlgorithm algorithm;
            if (System.Enum.TryParse<BoidMatesAlgorithm>(algorithmStr, out algorithm))
            {
                amountBoids = configurableSettings.amountBoids;
                boidMatesAlgorithm = algorithm;
            }
            else
            {
                Application.Quit();
            }
        }

    }
}