using ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCastTest : MonoBehaviour
{
    BoidData boidData;

    public float castSphereRadius = 3f;
    public float collisionRayDistance = 3f; 


    // Start is called before the first frame update
    void Start()
    {
        boidData = new BoidData();

    }

    // Update is called once per frame
    void Update()
    {


        /*


        boidData.castSphereRadius = castSphereRadius;
        boidData.collisionRayDistance = collisionRayDistance;

        LayerMask avoidObstacle = LayerMask.GetMask("AvoidObstacle");

        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, boidData.castSphereRadius, transform.forward, out hitInfo, boidData.collisionRayDistance, avoidObstacle))
        {

            //Avoid the Obstacle
            UnityEngine.Debug.Log("I have to avoid something");
        }
        else
        {
            // UnityEngine.Debug.Log("No");
        }*/
    }

    void OnDrawGizmos()
    {
        float r = transform.localScale.x;
        float v = 0.75f * Mathf.PI * r * r * r; //volume of one cube/sphere

        float newV = 7 * v;


        float newRadius = Mathf.Pow(newV/(0.75f*Mathf.PI), (1f / 3f));

        Gizmos.color = Color.green;
        //Gizmos.DrawSphere(transform.position, boidData.castSphereRadius);
       // Gizmos.DrawSphere(transform.position, transform.localScale.x);
        Gizmos.DrawSphere(transform.position, newRadius);


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.right *4f, transform.localScale.x);

    }
}


