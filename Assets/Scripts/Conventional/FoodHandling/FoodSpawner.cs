using System.Collections;
using UnityEngine;
using Common;

namespace ConventionalBoid {
    public class FoodSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject foodPrefab;
        [SerializeField] private float spawnRateInSeconds = 5f; 
        [SerializeField] private int amountToSpawn;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(SpawnFood());    
        }

        private IEnumerator SpawnFood()
        {
            BoidsManager boidsManager = BoidsManager.Instance as BoidsManager;

            for (; ; )
            {
          
                for (int i = 0; i < amountToSpawn; i++)
                {
                    Vector3 pos = HelperMethods.GenerateRandomPositionVector3(boidsManager.xInterval, boidsManager.yInterval, boidsManager.zInterval, boidsManager.boidLivesIn3DSpace);
                    Instantiate(foodPrefab, pos, foodPrefab.transform.rotation, transform);
                }
                yield return new WaitForSeconds(spawnRateInSeconds);
            }
        }
    }
}