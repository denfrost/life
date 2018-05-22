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
            float aux = EnvironmentSize - 1.5f;
            for (int i = 0; i < NumFishes; i++)
            {
                Vector3 pos = new Vector3(
                    Random.Range(-aux, aux),
                    Random.Range(-aux, aux),
                    Random.Range(-aux, aux));
                Fishes.Add(Instantiate(FishPrefab, pos, Quaternion.identity));
            }
            
            Predators = new List<GameObject>(PredatorsNum);
            float aux1 = EnvironmentSize - 1f;
            for (int i = 0; i < PredatorsNum; i++)
            {
                Vector3 tmp = new Vector3(
                    Random.Range(-aux1, aux1),
                    Random.Range(-aux1, aux1),
                    Random.Range(-aux1, aux1));
                Predators.Add(Instantiate(PredatorPrefab, tmp, Quaternion.identity));
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

            aux1 = EnvironmentSize - 0.5f;
            for (int i = 0; i < SingleFoodSize; i++)
            {
                Vector3 tmp = new Vector3(
                    Random.Range(-aux1, aux1),
                    Random.Range(-aux1, aux1),
                    Random.Range(-aux1, aux1));
                bool alone = true;
                foreach (GameObject v in FishFoods)
                    if (IsInRadious(v.transform.position, tmp))
                    {
                        alone = false;
                        break;
                    }

                if (alone)
                    Trash.Add(Instantiate(FoodPrefab, tmp, Quaternion.identity));
            }
        }

        private void Update()
        {
            if (Time.time >= _nextSeason)
            {
                _nextSeason = Time.time + SeasonRate;
                int a = Mathf.RoundToInt(Random.Range(0, _soureces.Count + 0.49f));
                int i = 0;
                foreach (List<GameObject> l in _soureces)
                {
                    bool active = Random.value <= SourceChance || a == i;
                    foreach (GameObject plant in l)
                        plant.SetActive(active);
                    i++;
                }
            }
        }
    }
}