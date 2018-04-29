using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public class Flock : MonoBehaviour
    {
        public int NumFishes = 20, SingleFoodSize = 310, SourcesFood = 53;
        public static List<GameObject> FishFoods = new List<GameObject>();
        public GameObject FishPrefab, FoodPrefab;
        public Transform[] Sources;
        public float SourcesRadius = 1.8f, FoodDistance = 0.8f, EnvironmentSize = 6;
        public static List<GameObject> Fishes;

        private bool IsInRadious(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b) <= FoodDistance;
        }

        private void Start()
        {
            Fishes = new List<GameObject>(NumFishes);
            for (int i = 0; i < NumFishes; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-EnvironmentSize, EnvironmentSize),
                    Random.Range(-EnvironmentSize, EnvironmentSize),
                    Random.Range(-EnvironmentSize, EnvironmentSize));
                Fishes.Add(Instantiate(FishPrefab, pos, Quaternion.identity));
            }

            foreach (Transform source in Sources)
            {
                for (int j = 0; j < SourcesFood; j++)
                {
                    Vector3 tmp = Random.insideUnitSphere * SourcesRadius + source.transform.position;
                    bool alone = true;
                    foreach (GameObject v in FishFoods)
                        if (IsInRadious(v.transform.position, tmp))
                        {
                            alone = false;
                            break;
                        }

                    if (!alone) continue;
                    FishFoods.Add(Instantiate(FoodPrefab, tmp, Quaternion.identity));
                }
            }

            float aux = EnvironmentSize - 0.5f;
            for (int i = 0; i < SingleFoodSize; i++)
            {
                
                Vector3 tmp = new Vector3(                    
                    Random.Range(-aux, aux),
                    Random.Range(-aux, aux),
                    Random.Range(-aux, aux));
                bool alone = true;
                foreach (GameObject v in FishFoods)
                    if (IsInRadious(v.transform.position, tmp))
                    {
                        alone = false;
                        break;
                    }

                if (!alone) continue;
                Instantiate(FoodPrefab, tmp, Quaternion.identity);
                //FishFoods.Add(Instantiate(FoodPrefab, tmp, Quaternion.identity));
            }
        }
    }
}