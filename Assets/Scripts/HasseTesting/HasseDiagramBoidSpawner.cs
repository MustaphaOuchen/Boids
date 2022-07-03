using System.Collections.Generic;
using UnityEngine;
using Common;
using ConventionalBoid;
using UnityEditor;
using System.Linq;

namespace HasseDiagram
{
    public class HasseDiagramBoidSpawner : BoidsManagerBase//MonoBehaviour
    {

        //Graph
        //Lets define A Boid as a Node in a Graph
        [Header("Hasse diagram fields")]
        public int divisionNumber = 60;
        public int applyingForcesSteps = 100;
        public float xSpacingScaler = 10f;
        public float zSpacingScaler = 10f;
        public List<NodeBoid> allBoids;
        //Adjacency list
        Dictionary<int, List<int>> adjNodeBoids = new Dictionary<int, List<int>>();
        //List where transitive relations are not removed
        Dictionary<int, List<int>> adjNodeBoidsAll = new Dictionary<int, List<int>>();
        //Map set value to boid back
        Dictionary<int,NodeBoid> boidValueToBoid = new Dictionary<int, NodeBoid>();
        //For applying rules
        Dictionary<Boid, List<Boid>> cohesionAndAlignmentBoidList = new Dictionary<Boid, List<Boid>>();
        Dictionary<Boid, List<Boid>> separationBoidList = new Dictionary<Boid, List<Boid>>();

        // Start is called before the first frame update
        void Start()
        {
            
            //Lets assume we want to create a Hasse Diagram of by the division relation
            int[] divisionSet = GetDivisorsOfANumber(divisionNumber);
            perceptionRadius = float.PositiveInfinity;
            CreateNodeBoids(divisionSet);
            BuildAdjacenyGraphList();
            AssignPositionToNodeBoids();
            InitializeBehviourNeighbors();
            ApplyForces();
           
        }


        private void InitializeBehviourNeighbors()
        {
            //Cohesion on comparable Points
            foreach(var nodeBoid in allBoids)
            {
                cohesionAndAlignmentBoidList[nodeBoid] = new List<Boid>();
                separationBoidList[nodeBoid] = new List<Boid>();    
                foreach (var otherNodeBoid in allBoids)
                {
                    if (nodeBoid == otherNodeBoid)
                        continue;

                    if (IsInRelation(nodeBoid,otherNodeBoid))
                    {
                        //Apply cohesion and alignment
                        cohesionAndAlignmentBoidList[nodeBoid].Add(otherNodeBoid);
                       
                    }else
                    {
                        //Apply separation
                        separationBoidList[nodeBoid].Add(otherNodeBoid);
                    }
                }
            }
        }

        private void ApplyForces()
        {
            for (int i = 0;  i < applyingForcesSteps;i++)
            {
                foreach (NodeBoid boid in cohesionAndAlignmentBoidList.Keys)
                {
                    List<Boid> neighbors = cohesionAndAlignmentBoidList[boid];
                    boid.separationScale = 0f;
                    boid.alignmentScale =alignmentScale;
                    boid.cohesionScale = cohesionScale;
                    UpdateBoid(boid, neighbors);
                }

                foreach (NodeBoid boid in separationBoidList.Keys)
                {
                    List<Boid> neighbors = separationBoidList[boid];
                    boid.separationScale = separationScale;
                    boid.alignmentScale = 0f;
                    boid.cohesionScale = 0f;
                    UpdateBoid(boid, neighbors);
                }
            }
        }

        private void UpdateBoid(NodeBoid boid, List<Boid> neighbors)
        {
            //No limiting
            //boid.LimitBoidInScreen();
            boid.CalculateAllBehaviour(neighbors);
            boid.ApplyAndResetForce();    
            boid.HardTranslation(); 
        }
        private void AssignPositionToNodeBoids()
        {
            Dictionary<int, List<NodeBoid>> rankToBoids = new Dictionary<int, List<NodeBoid>>();           
            foreach (NodeBoid nodeBoid in allBoids)
            {
                int y = GetHeight(nodeBoid); //Assigh height
                if (!rankToBoids.ContainsKey(y))
                    rankToBoids[y] = new List<NodeBoid>();
                rankToBoids[y].Add(nodeBoid);               
                Vector3 pos = nodeBoid.transform.position;
                pos.y = y;
                nodeBoid.transform.position = pos;  
            }
            //Points with the same "rank/height" <-> are ranged around on a circle of the x - z plane
            // Spacing must be equal
            float goldenRatioTimesTwo = (1 + Mathf.Sqrt(5)); 
            foreach (int rank in rankToBoids.Keys)
            {
                List<NodeBoid> boidsOnSameCircle = rankToBoids[rank];
                //Calculate for each Boid a x, z Component evenly Distributed on the circle <-> spiral algorithm
                int numberOfPointsToGenerate = boidsOnSameCircle.Count;
                for (int i = 0; i < numberOfPointsToGenerate; i++)
                {
                    float indexNumber = (float)i + 0.5f;
                    float iterationFraction = Mathf.Sqrt(indexNumber)/ numberOfPointsToGenerate;
                    float theta = Mathf.PI * goldenRatioTimesTwo * indexNumber;
                    float x =  iterationFraction * Mathf.Cos(theta) * rank * xSpacingScaler;
                    float z = iterationFraction * Mathf.Sin(theta) * rank * zSpacingScaler;
                    Vector3 pos = boidsOnSameCircle[i].transform.position;
                    pos.x = x;
                    pos.z = z;
                    //Setting the position
                    boidsOnSameCircle[i].transform.position = pos;
                }
            }
        }

        private void BuildAdjacenyGraphList()
        {
            //One With removed transitive relations
            foreach (var nodeBoid in allBoids)
            {
                adjNodeBoidsAll[nodeBoid.SetValue] = new List<int>();
                adjNodeBoids[nodeBoid.SetValue] = new List<int>();
                //Structure to reverse back
                boidValueToBoid[nodeBoid.SetValue] = nodeBoid;
                //Search for possible relation (avoiding loops)
                foreach (var otherNodeBoid in allBoids)
                {
                    if (nodeBoid == otherNodeBoid)
                        continue;

                    if (IsInRelation(nodeBoid,otherNodeBoid))
                    {
                        adjNodeBoids[nodeBoid.SetValue].Add(otherNodeBoid.SetValue);
                        adjNodeBoidsAll[nodeBoid.SetValue].Add(otherNodeBoid.SetValue);
                    }
                }

            }

            //Remove only  transitive relation
           int[] possibleValues = adjNodeBoids.Keys.ToArray();
           foreach (int nodeBoidValue in possibleValues)
            {
                foreach (int nodeBoidValueToCompare in possibleValues)
                {
                    //Ignore self
                    if (nodeBoidValue == nodeBoidValueToCompare)
                        continue;
                    //Remove neighbors of neighbor
                    if (adjNodeBoids[nodeBoidValue].Contains(nodeBoidValueToCompare))
                        adjNodeBoids[nodeBoidValue].RemoveAll(r => adjNodeBoids[nodeBoidValueToCompare].Contains(r));
                }
            }
        }


        // <= relation
        private bool IsInRelation(NodeBoid nodeBoid, NodeBoid otherNodeBoid)
        {
            return nodeBoid.SetValue % otherNodeBoid.SetValue == 0;
        }


        private void CreateNodeBoids(int[] divisionSet)
        {
            int amount = divisionSet.Length;

            //Create boids
            for (int i = 0; i < amount; i++)
            {
                //Quaternion rot = Common.HelperMethods.GenerateRandomRotation(boidsLiveIn3DSpace);
                Quaternion rot = Quaternion.identity;
                GameObject gb = Instantiate(boidPrefab, transform.position, rot, transform);
                NodeBoid nodeBoid = gb.GetComponent<NodeBoid>();
                SetUpBoid(nodeBoid);
                nodeBoid.xInterval = xInterval;
                nodeBoid.yInterval = yInterval;
                nodeBoid.zInterval = zInterval;
                nodeBoid.boidLivesIn3DSpace = boidLivesIn3DSpace;
                allBoids.Add(nodeBoid);
                nodeBoid.SetValue = divisionSet[i];
                nodeBoid.velocity = Vector3.one;

            }
        }

     

        private void SetUpBoid(NodeBoid nodeBoid)
        {
            if (disableRealtimeConfiguration) return; //Avoid configuration if not wanted
            nodeBoid.separationScale = separationScale;
            nodeBoid.alignmentScale = alignmentScale;
            nodeBoid.cohesionScale = cohesionScale;
            nodeBoid.maxSpeed = maxSpeed;
            nodeBoid.initialSpeed = initialSpeed;
            nodeBoid.maxForce = maxForce;
            nodeBoid.perceptionRadiusSquared = perceptionRadius * perceptionRadius;
            nodeBoid.collisionAvoidanceScale = collisionAvoidanceScale;
            nodeBoid.castSphereRadius = castSphereRadius;
            nodeBoid.collisionRayDistance = collisionRayDistance;
            nodeBoid.gravityFactor = gravityFactor;
            nodeBoid.allowAdditionalBehaviour = allowAdditionalBehaviour;
            nodeBoid.boidLivesIn3DSpace = boidLivesIn3DSpace;
        }


        // method to get the divisors of an arbitrary number
        public int[] GetDivisorsOfANumber(int n)
        {
            List<int> divisors = new List<int>();
            for (int i = 1; i <= Mathf.Sqrt(n);
                                          i++)
            {
                if (n % i == 0)
                {
                    if (n / i == i)
                        divisors.Add(i);
                    else
                    {
                        divisors.Add(i);
                        divisors.Add(n / i);
                    }
                }
            }
            return divisors.ToArray();
        }

        private void OnDrawGizmos()
        {
        #if UNITY_EDITOR
            bool drawRelations = false;
            Gizmos.color = Color.yellow;
            //Handles.Label(transform.position, debugText);
            foreach (var nodeBoid in allBoids)
            {
                string debugText = $@"{nodeBoid.SetValue}";
                //string debugText = $@"{nodeBoid.SetValue} \n -> In relation with: ";
                if (drawRelations)
                {
                    if (adjNodeBoids.ContainsKey(nodeBoid.SetValue))
                    {
                        List<int> isInRelationWith = adjNodeBoids[nodeBoid.SetValue];
                        foreach (var relationNodeValue in isInRelationWith)
                        {
                            debugText = debugText + relationNodeValue.ToString() + "-> ";
                        }
                    }
                }
                Handles.Label(nodeBoid.transform.position, debugText);
            }
            //Drawing connection lines
            Gizmos.color = Color.red;
            foreach (NodeBoid nodeBoid in allBoids)
            {
                List<int> connectionMates = adjNodeBoids[nodeBoid.SetValue];
                foreach(var connectionNodeSetValue in connectionMates)
                {
                    NodeBoid connectedBoid = boidValueToBoid[connectionNodeSetValue];
                    Gizmos.DrawLine(nodeBoid.transform.position, connectedBoid.transform.position);
                }
            }
            #endif
        }



        //Methods Concerning Automated Lattice Drawing <-> Ralph Frese    
        //height(a) = length of the longest chain a to a minimal element
        //depth(a) = length of the longest chain from a to a maximal element
        //M <-> Longest chain
        //Simplification to amount in relation with

        private int GetHeight(NodeBoid nodeBoid)
        {
            //Simple return amount relations
           return adjNodeBoidsAll[nodeBoid.SetValue].Count;           
        }
    }
}