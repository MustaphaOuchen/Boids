using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
namespace ConventionalBoid
{
    //Original lisp code by Craig Reynolds: http://www.red3d.com/cwr/code/boids.lisp
    public class Boid : BoidSettingsWithBehaviourBase//MonoBehaviour
    {

        [Header("Dynamic boid variables")]
        public Vector3 velocity;
        public Vector3 accelerationForce;
        //uses squared perception Radius
        public float perceptionRadiusSquared = 9f;
        //pheromone and food
        private bool blockPheromone = false;
        public bool isDead = false;
        public float lifeTime = 100f;
 
        //Limits Boids in a "PacMan" style
        public void LimitBoidInScreen()
        {
            if (transform.position.x > xInterval)
            {
                transform.position = new Vector3(-xInterval, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < -xInterval)
            {
                transform.position = new Vector3(xInterval, transform.position.y, transform.position.z);
            }
            if (transform.position.y > yInterval)
            {
                transform.position = new Vector3(transform.position.x, -yInterval, transform.position.z);
            }
            else if (transform.position.y < -yInterval)
            {
                transform.position = new Vector3(transform.position.x, yInterval, transform.position.z);
            }
            if (boidLivesIn3DSpace)
            {
                if (transform.position.z > zInterval)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, -zInterval);
                }
                else if (transform.position.z < -zInterval)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, zInterval);
                }
            }
        }

        public void CalculateAllBehaviour(List<Boid> boids)
        {
            Vector3 alignmentVector = Vector3.zero;
            Vector3 cohesionVector = Vector3.zero;
            Vector3 separationVector = Vector3.zero;
            Vector3 averageAlignmentVector = Vector3.zero;
            Vector3 averageCohesionVector = Vector3.zero; //center of mass
            int total = 0;

            foreach (var boid in boids)
            {
                if (boid == this) continue;
                float squaredDistance = Vector3.SqrMagnitude(transform.position - boid.transform.position);
                if (squaredDistance < perceptionRadiusSquared)
                {
                    averageAlignmentVector += boid.velocity;
                    averageCohesionVector += boid.transform.position;
                    Vector3 difference = transform.position - boid.transform.position;
                    difference /= squaredDistance; //Closer Boid <-> greater Force
                    separationVector += difference;
                    
                    total++;
                }
            }
            if (total > 0)
            {
                //Alignment
                averageAlignmentVector /= total;
                averageAlignmentVector = (averageAlignmentVector / averageAlignmentVector.magnitude) * maxSpeed;
                alignmentVector = averageAlignmentVector - velocity;
                //No Limiting of alignment
                //Cohesion
                averageCohesionVector /= total;
                Vector3 vecToCom = averageCohesionVector - transform.position;
                cohesionVector = vecToCom - velocity;
                cohesionVector = Vector3.ClampMagnitude(cohesionVector, maxForce);
                //Separation
                separationVector = Vector3.ClampMagnitude(separationVector, maxForce);    
            }
            //Calculate everything up
            alignmentVector *= alignmentScale;
            cohesionVector *= cohesionScale;
            separationVector *= separationScale;
            accelerationForce = alignmentVector + cohesionVector + separationVector;
            if (allowAdditionalBehaviour)
            {
                //Gravity
                Vector3 gravityVector = Vector3.down  * Time.deltaTime;
                gravityVector = Vector3.ClampMagnitude(gravityVector, maxForce)* gravityFactor;
                accelerationForce += gravityVector;
                //Add some randomness
                Vector3 randomForce = Random.insideUnitSphere ;
                randomForce.z = boidLivesIn3DSpace ? randomForce.z : 0f;
                randomForce = Vector3.ClampMagnitude(randomForce, maxForce)* randomScale;
                accelerationForce += randomForce;
                //Collision
                if (BoidWillCollide())
                {      
                    Vector3 collisionAvoidancePoint = ObstacleRays();
                    Vector3 displacementVector = collisionAvoidancePoint.normalized * maxSpeed - velocity;
                    Vector3 collisionAvoidForce = Vector3.ClampMagnitude(displacementVector, maxForce) * collisionAvoidanceScale; 
                    accelerationForce += collisionAvoidForce;
                }
                //Pheromone and food part
                TryGoToPheromone();
                //Spawn self pheromone
                if (BoidHasAccessToFood())
                {
                    //Increase life points
                    lifeTime += lifePointSummand;
                    if (blockPheromone) return;
                    StartCoroutine(SpreadPheromone());
                }
            }
        }


        public void ApplyAndResetForce()
        {
            velocity += accelerationForce;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            accelerationForce = Vector3.zero;
        }

        public void MoveAndRotateBoid()
        {
            //Movement
            transform.position += velocity * Time.deltaTime;

            if (boidLivesIn3DSpace)
            {
                Quaternion lookRotation = Quaternion.LookRotation(velocity);
                //Smoothing out
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);

            }
            else
            {
                //2D has different rotation
                // rotate that vector by 90 degrees around the Z axis
                Vector3 rotatedVector = Quaternion.Euler(0, 0, 90) * velocity;
                Quaternion targetRotation =  Quaternion.LookRotation(forward: velocity, upwards: rotatedVector);
                //Smoothing out
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,Time.deltaTime);

            }

        }

        //Additional behavior
        //Method originally by SebLague https://github.com/SebLague/Boids/tree/master
        private bool BoidWillCollide()
        {
            RaycastHit hitInfo;
            //Casts a sphere along a ray
            bool boidColides =  Physics.SphereCast(transform.position, castSphereRadius, transform.forward, out hitInfo, collisionRayDistance, LayerMask.GetMask("AvoidObstacle"));
            return boidColides;
        }

        //Method originally by SebLague https://github.com/SebLague/Boids/tree/master
        private Vector3 ObstacleRays()
        {
            Vector3[] rayDirections = BoidCollisionHelper.unitSpherePoints;
            for (int i = 0; i < rayDirections.Length; i++)
            {
                Vector3 collisionAvoidancePoint = transform.TransformDirection(rayDirections[i]);

                if(!boidLivesIn3DSpace)
                {
                    collisionAvoidancePoint.z = 0f;
                }

                Ray ray = new Ray(transform.position, collisionAvoidancePoint);
                if (!Physics.SphereCast(ray, castSphereRadius, collisionRayDistance, LayerMask.GetMask("AvoidObstacle")))
                {
                    return collisionAvoidancePoint;
                }
            }
            return transform.forward;
        }

        private void TryGoToPheromone()
        {
            Vector3 position = new Vector3();
            if (BoidSensesPheromone(ref position))
            {
                Vector3 pheremoneDisplacementVector = (position - velocity).normalized * pheromoneScale;
                if (!boidLivesIn3DSpace)
                {
                    pheremoneDisplacementVector.z = 0f;
                }
                accelerationForce += pheremoneDisplacementVector;
            }
        }

        private bool BoidSensesPheromone(ref Vector3 pheromonePosition)
        {
            Vector3[] unitSpherePoints = BoidCollisionHelper.unitSpherePoints;
            for (int i = 0; i < unitSpherePoints.Length; i++)
            {
                Vector3 pointFromBoid = transform.TransformDirection(unitSpherePoints[i]);
                Ray ray = new Ray(transform.position, pointFromBoid);
                if (Physics.SphereCast(ray, castSphereRadius, pheromoneRayDistance, LayerMask.GetMask("Pheromone")))
                {
                    pheromonePosition = pointFromBoid;
                    return true;
                }
            }
            return false;
        }

        private IEnumerator SpreadPheromone()
        {

            blockPheromone = true;
            GameObject gb = Instantiate(pheromonePrefab, transform.position, transform.rotation);
            yield return new WaitForSeconds(secondsToRegeneratePheromone);
            blockPheromone = false;
        }


        private bool BoidHasAccessToFood()
        {
            Vector3[] unitSpherePoints = BoidCollisionHelper.unitSpherePoints;
            for (int i = 0; i < unitSpherePoints.Length; i++)
            {
                Vector3 pointFromBoid = transform.TransformDirection(unitSpherePoints[i]);
                Ray ray = new Ray(transform.position, pointFromBoid);
                if (Physics.SphereCast(ray, castSphereRadius, foodRayDistance, LayerMask.GetMask("Food")))
                {
                    return true;
                }
            }
            return false;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!allowAdditionalBehaviour)
                return;

            lifeTime -= Time.deltaTime;
            if (lifeTime < 0f)
            {
                isDead = true;
            }
        }

        void OnDrawGizmos()
        {
            if (GameObject.Find("BoidsManager") != null)
            {
                BoidsManager conventionalBoidsManager = BoidsManager.Instance as BoidsManager;
                if (conventionalBoidsManager != null)
                {
                    if (conventionalBoidsManager.drawBoidCollisionDirections)
                    {
                        Gizmos.color = Color.red;
                        //Drawing possible ways to avoid obstacles
                        Vector3[] rayDirections = BoidCollisionHelper.unitSpherePoints;
                        for (int i = 0; i < rayDirections.Length; i++)
                        {
                            Vector3 direction = transform.TransformDirection(rayDirections[i]);

                            Ray ray = new Ray(transform.position, direction * collisionRayDistance);
                            Gizmos.DrawRay(transform.position, direction);
                        }
                    }

                    if (conventionalBoidsManager.drawBoidCollisionSpheres)
                    {
                        //Drawing the collision sphere
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(transform.position, castSphereRadius);
                    }
                }
            }

        }
    }
}