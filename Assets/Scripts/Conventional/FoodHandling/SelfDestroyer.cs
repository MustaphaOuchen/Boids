using UnityEngine;

public class SelfDestroyer : MonoBehaviour
{
    //Amount of Seconds until the GameObject is destroyed
    [SerializeField] private float maxTimeUntilDestructionInSeconds = 8f;
    private float timeUntilDestructionInSeconds;
    // Start is called before the first frame update
    void Start()
    {
       timeUntilDestructionInSeconds = Random.Range(0, maxTimeUntilDestructionInSeconds);
    }
    // Update is called once per frame
    void Update()
    {
        timeUntilDestructionInSeconds -= Time.deltaTime;
        if (timeUntilDestructionInSeconds < 0)
        {
            Destroy(gameObject);
        }
    }
}
