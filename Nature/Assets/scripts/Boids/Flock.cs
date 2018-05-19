using System.Collections.Generic;
using UnityEngine;

namespace Boids
{
    public class Flock : MonoBehaviour
    {
        public int NumFishes = 30, SingleFoodSize = 44, SourcesFood = 44, PredatorsNum = 2;
        public static List<GameObject> FishFoods, Fishes, Predators, Trash;
        public GameObject FishPrefab, FoodPrefab, PredatorPrefab;
        public Transform[] SourcesTransforms;
        public float SourcesRadius = 1.2f, FoodDistance = 0.28f, EnvironmentSize = 6;
        public float SeasonRate = 10f, SourceChance = 0.4f;
        private List<List<GameObject>> _soureces;
        private float _nextSeason;


        private bool IsInRadious(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b) <= FoodDistance;
        }

        private void Awake()
        {
            _soureces = new List<List<GameObject>>(SourcesTransforms.Length);
            FishFoods = new List<GameObject>(SourcesFood * SourcesTransforms.Length);
            Fishes = new List<GameObject>(NumFishes);
            Trash = new List<GameObject>(SingleFoodSize);
            for (int i = 0; i < NumFishes; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-EnvironmentSize, EnvironmentSize),
                    Random.Range(-EnvironmentSize, EnvironmentSize),
                    Random.Range(-EnvironmentSize, EnvironmentSize));
                Fishes.Add(Instantiate(FishPrefab, pos, Quaternion.identity));
            }

            int c = 0;
            foreach (Transform source in SourcesTransforms)
            {
                _soureces.Add(new List<GameObject>(SourcesFood));
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

                    if (alone)
                    {
                        GameObject plant = Instantiate(FoodPrefab, tmp, Quaternion.identity);
                        plant.SetActive(false);
                        _soureces[c].Add(plant);
                        FishFoods.Add(plant);
                    }
                }

                c++;
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
                //Instantiate(FoodPrefab, tmp, Quaternion.identity);
                Trash.Add(Instantiate(FoodPrefab, tmp, Quaternion.identity));
            }

            Predators = new List<GameObject>(PredatorsNum);
            for (int i = 0; i < PredatorsNum; i++)
            {
                Vector3 tmp = new Vector3(
                    Random.Range(-aux, aux),
                    Random.Range(-aux, aux),
                    Random.Range(-aux, aux));
                Predators.Add(Instantiate(PredatorPrefab, tmp, Quaternion.identity));
            }
        }

        private void Update()
        {
            if (Time.time >= _nextSeason)
            {
                _nextSeason = Time.time + SeasonRate;
                foreach (List<GameObject> l in _soureces)
                {
                    bool active = Random.value <= SourceChance;
                    foreach (GameObject plant in l)
                        plant.SetActive(active);
                }
            }
        }
    }
}