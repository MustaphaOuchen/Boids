using UnityEngine;

public class DestroyOnObstacleCollision : MonoBehaviour
{
    //food will be destroyed if it collides with an "AvoidObstacle" and the collider is a "trigger"
    private void OnTriggerEnter(Collider other)
    {
        int layer = LayerMask.NameToLayer("AvoidObstacle");

        if(other.gameObject.layer == layer)
        {
            Destroy(gameObject);
        }
    }
}
