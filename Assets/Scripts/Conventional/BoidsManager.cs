using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEditor;
//kd
using DataStructures.ViliWonka.KDTree;
//Octree
using UnityOctree;
//Profiling
using UnityEngine.Profiling;

namespace ConventionalBoid
{
    /// <summary>
    /// Manager class which creates all Boids an initiates values.
    /// </summary>
    public class BoidsManager :BoidsManagerBase //MonoBehaviour
    {
        public List<Boid> allBoids;
        [Header("KD-Tree")]
        KDTree kdtree;
        KDQuery query;
        //Points of boids must be cached here
        public Vector3[] pointCloud;
        //UnityOctree
        BoundsOctree<Boid> boundsOctree;
        //Spatial Hashing
        Dictionary<int, List<Boid>> boidToNeighbors;
        [Header("Enemies in the life of a Boid")]
        //public EnemyController enemy;
        [SerializeField] float enemyScaleFactor = 2f;
        public int amountEnemies = 10;
        public List<Boid> enemyBoids;
        [Header("OnDrawGizmosSelected Settings")]
        public bool drawShadowRealm = true;
        public bool drawBoidCollisionDirections = true;
        public bool drawBoidCollisionSpheres = true;

        // Start is called before the first frame update
        void Start()
        {
            CreateBoids(amountBoids,allBoids);
            CreateEnemyBoids();  
            //Initializing KD-Tree 
            if (boidMatesAlgorithm == BoidMatesAlgorithm.KD_Tree_Bounds)
            {
                pointCloud = new Vector3[amountBoids];
                
                for (int i = 0; i < amountBoids; i++)
                {

                    pointCloud[i] = allBoids[i].transform.position;
                }
                //Creating KD Tree
                kdtree = new KDTree(pointCloud, 7); //calls rebuild
                query = new KDQuery();
            }
            if (boidMatesAlgorithm == BoidMatesAlgorithm.SpatialHashing)
            {
                boidToNeighbors = new Dictionary<int, List<Boid>>();
                //Using the perceptionRadius to assign the cell
                conversionFactor = 1 / (2 * perceptionRadius);

            }
        }


        private void CreateBoids(int amount,List<Boid> listToAddBoids)
        {
            //Create boids
            for (int i = 0; i < amount; i++)
            {
                Vector3 randomPos = HelperMethods.GenerateRandomPositionVector3(xInterval, yInterval, zInterval, boidLivesIn3DSpace);      
                Quaternion rot = HelperMethods.GenerateRandomRotation(boidLivesIn3DSpace);
                GameObject gb = Instantiate(boidPrefab, randomPos, rot, transform);
                Boid boid = gb.GetComponent<Boid>();
                SetUpBoid(boid);
                boid.xInterval = xInterval;
                boid.yInterval = yInterval;
                boid.zInterval = zInterval;
                boid.boidLivesIn3DSpace = boidLivesIn3DSpace;
                listToAddBoids.Add(boid);
                boid.pheromonePrefab = pheromonePrefab;
                boid.velocity = HelperMethods.GetRandomVelocityVector3(boidLivesIn3DSpace,initialSpeed);
            }
        }

        private void CreateEnemyBoids()
        {
            CreateBoids(amountEnemies, enemyBoids);
            foreach (Boid enemyBoid in enemyBoids)
            {
                //Adding a sphere collider
                SphereCollider sphereCollider = enemyBoid.gameObject.AddComponent<SphereCollider>();
                sphereCollider.radius = perceptionRadius;//* enemyScaleFactor;
                //Set tag of collider
                enemyBoid.gameObject.layer = LayerMask.NameToLayer("AvoidObstacle");
                MeshRenderer meshRenderer = enemyBoid.gameObject.GetComponentInChildren<MeshRenderer>();
                meshRenderer.material.color = Color.red; //Make enemies red
                //Some scale
                enemyBoid.transform.localScale *= enemyScaleFactor;
                //Do not Seperate from target Boids
                enemyBoid.separationScale = 0f;
            }
        }

        private void BuildKdTree()
        {
            Profiler.BeginSample("KdTreeConstruction");
            for (int i = 0; i < amountBoids; i++)
            {
                kdtree.Points[i] = allBoids[i].transform.position; //rebuilding will be with zero GC impact
            }
            //Resets tree structure
            kdtree.Rebuild();
            Profiler.EndSample();
        }
     


        private void CreateUnityOctreeBounds()
        {
            Profiler.BeginSample("UnityOctreeBounds");
            float boxSize = Mathf.Max(xInterval, yInterval, zInterval) * 2; //Dat sollte passen
            boundsOctree = new UnityOctree.BoundsOctree<Boid>(boxSize,transform.position, 2 * perceptionRadius,1.25f);
            //Adding positions
            foreach (var boid in allBoids)
            {
                //Create bounds
                Vector3 center = boid.transform.position;          
                Vector3 extents = new Vector3(perceptionRadius, perceptionRadius, perceptionRadius);
                Bounds bounds = new Bounds(center,2 * extents );
                boundsOctree.Add(boid, bounds);
            }
            Profiler.EndSample();
        }

        private void CreateSpatialHashMap()
        {
            Profiler.BeginSample("SpatialHashing");
            boidToNeighbors.Clear();
            foreach (Boid boid in allBoids)
            {
                int key = SpatialHasher.HashPositionVector3(boid.transform.position,amountBoids,conversionFactor);
                if (!boidToNeighbors.ContainsKey(key))
                {
                    boidToNeighbors[key] = new List<Boid>();
                }
                boidToNeighbors[key].Add(boid);            
            }
            Profiler.EndSample();
        }

        // Update is called once per frame
        void Update()
        {
            if (allowAdditionalBehaviour)
                DestroyDeadBoids();
            if (boidMatesAlgorithm == BoidMatesAlgorithm.Bruteforce)
            {
                BruteForce(allBoids);
                if (amountEnemies > 0)
                    BruteForce(enemyBoids);
            }
 
            else if (boidMatesAlgorithm == BoidMatesAlgorithm.KD_Tree_Bounds)
            {
                BuildKdTree();
                UseKdTreeBounds(allBoids);
                if (amountEnemies > 0)
                    UseKdTreeBounds(enemyBoids);
            }

            else if (boidMatesAlgorithm == BoidMatesAlgorithm.UnityOctreeBounds)
            {
                CreateUnityOctreeBounds();
                UseUnityOCTreeBounds(allBoids);
                if (amountEnemies > 0)
                    UseUnityOCTreeBounds(enemyBoids);
            }
             else if (boidMatesAlgorithm == BoidMatesAlgorithm.SpatialHashing)
            {
                CreateSpatialHashMap();
                UseSpatialHashing(allBoids);
                if (amountEnemies > 0)
                    UseSpatialHashing(enemyBoids);
            }
            //Debug Break
            if(activateDebugBreak)
            {
                secondsUntilDebugBreak -= Time.unscaledDeltaTime;
                if(secondsUntilDebugBreak < 0)
                {
                    Debug.Break();
                }
            }
        }

        private void BruteForce(List<Boid> boidsToUpdate)
        {
            foreach (Boid boid in boidsToUpdate)
            {
                SetUpBoid(boid);
                UpdateBoid(boid, allBoids);
            }
        }


        private void UseKdTreeBounds(List<Boid> boidsToUpdate)
        {

            for (int i = 0; i < boidsToUpdate.Count; i++)
            {
                Boid boid = boidsToUpdate[i];
                SetUpBoid(boid);
                var resultIndices = new List<int>();
                //Create bounds
                Vector3 center = transform.position;
                //Using addition
                Vector3 extents = new Vector3(perceptionRadius, perceptionRadius, perceptionRadius);              
                Vector3 min = center - extents;
                Vector3 max = center + extents;
                Profiler.BeginSample("KDBoundsQuery");
                query.Interval(kdtree, min, max, resultIndices);
                Profiler.EndSample();
                List<Boid> neighbors = new List<Boid>(); //clearinglist faster??? -> only possible if the behaviour is calculated by the manager
                foreach (int index in resultIndices)
                {
                    neighbors.Add(allBoids[index]);
                }
                UpdateBoid(boid, neighbors);
            }           
        }
       
        private void UseUnityOCTreeBounds(List<Boid> boidsToUpdate)
        {
            foreach (Boid boid in boidsToUpdate)
            {
                SetUpBoid(boid);
                //Create bounds
                Vector3 center = boid.transform.position;
                Vector3 extents = new Vector3(perceptionRadius,perceptionRadius,perceptionRadius);             
                Bounds bounds = new Bounds(center, 2 * extents);
                //perception radius individual
                List<Boid> neighbors = new List<Boid>();
                Profiler.BeginSample("UnityOctreeBoundsQuery");
                boundsOctree.GetColliding(neighbors, bounds);
                Profiler.EndSample();
                UpdateBoid(boid, neighbors);
            }
        }

        private void UseSpatialHashing(List<Boid> boidsToUpdate)
        {
            foreach (Boid boid in boidsToUpdate)
            {
                SetUpBoid(boid);
                int key = SpatialHasher.HashPositionVector3(boid.transform.position, amountBoids, conversionFactor);
                if (boidToNeighbors.ContainsKey(key))
                {
                    List<Boid> neighbors = boidToNeighbors[key];
                    UpdateBoid(boid, neighbors);
                }
            }
        }

        private void UpdateBoid(Boid boid, List<Boid> neighbors)
        {
            boid.LimitBoidInScreen();
            boid.CalculateAllBehaviour(neighbors);
            boid.ApplyAndResetForce();
            boid.MoveAndRotateBoid();
        }

        //Sets the boid with the editor setting
        private void SetUpBoid(Boid boid)
        {
            if (disableRealtimeConfiguration) return; //Avoid configuration if not wanted         
            boid.separationScale = separationScale;
            boid.alignmentScale = alignmentScale;
            boid.cohesionScale = cohesionScale;     
            boid.maxSpeed = maxSpeed;
            boid.initialSpeed = initialSpeed;
            boid.maxForce = maxForce;
            boid.perceptionRadiusSquared = perceptionRadius * perceptionRadius;
            boid.collisionAvoidanceScale = collisionAvoidanceScale;
            boid.castSphereRadius = castSphereRadius;
            boid.collisionRayDistance = collisionRayDistance;
            boid.allowAdditionalBehaviour = allowAdditionalBehaviour;
            if (allowAdditionalBehaviour)
            {
                //Pheremone
                boid.secondsToRegeneratePheromone = secondsToRegeneratePheromone;
                boid.pheromoneRayDistance = pheromoneRayDistance;
                boid.pheromoneScale = pheromoneScale;
                //Food
                boid.foodRayDistance = foodRayDistance;
                //Randomness
                boid.randomScale = randomScale;
                boid.gravityFactor = gravityFactor;
            }
        }


        private void DestroyDeadBoids()
        {
            //Create copy
            foreach (Boid boid in allBoids.ToArray())
            {
                if(boid.isDead)
                {
                    allBoids.Remove(boid);
                    Destroy(boid.gameObject);
                    amountBoids--;
                }
            }
        }

        //Gizmos for debug reasons
        void OnDrawGizmos()
        {
#if UNITY_EDITOR

            bool drawHash =  false;

            //Drawing boid keys

            if (drawHash)
            {
                foreach (var boid in allBoids)
                {
                    Vector3 position = boid.transform.position;
                    string hashStr = SpatialHasher.HashPositionVector3(position, amountBoids, conversionFactor).ToString();
                    Gizmos.color = Color.yellow;
                    Handles.Label(position, hashStr);
                }
            }
            if (drawShadowRealm)
            {

                // Draw a semitransparent blue cube at the transforms position
                Gizmos.color = new Color(0, 0.3f, 1, 0.2f);

                if (boidLivesIn3DSpace)
                {
                    Gizmos.DrawCube(transform.position, new Vector3(xInterval * 2, yInterval * 2, zInterval * 2));
                }
                else
                {
                    Gizmos.DrawCube(transform.position, new Vector3(xInterval * 2, yInterval * 2, 1));
                }
            }
#endif
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