using System.Collections.Generic;
using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
    public int numFish = 10, environmentSize = 32, singleFoodSize = 40, sourcesFood = 40;
    private HashSet<Vector3> foodPositions = new HashSet<Vector3>();
    public GameObject fishPrefab, goalPrefab;
    public static List<GameObject> food;
    public GameObject[] foodPrefabs;
    public static GameObject[] fishes;
    public static Vector3 goalPos = Vector3.zero;
    public static int size;
    public Transform[] sources;
    public float sourcesRadius = 4;

    void Start()
    {
        for (int i = 0; i < singleFoodSize; i++)
        {
            food = new List<GameObject>(singleFoodSize);
            Vector3 tmp = new Vector3(
                Random.Range(-environmentSize, environmentSize),
                Random.Range(-environmentSize, environmentSize),
                Random.Range(-environmentSize, environmentSize));
            if (foodPositions.Add(tmp))
                food.Add(Instantiate(foodPrefabs[Mathf.RoundToInt(Random.Range(0, foodPrefabs.Length - 0.7f))],
                    tmp, Quaternion.identity));
        }
        for (int i = 0; i < sources.Length; i++)
        {
            for (int j = 0; j < sourcesFood; j++)
            {
                Vector3 tmp = Random.insideUnitSphere * sourcesRadius + sources[i].transform.position;
                if (foodPositions.Add(tmp))
                    food.Add(Instantiate(foodPrefabs[Mathf.RoundToInt(Random.Range(0, foodPrefabs.Length - 0.7f))],
                        tmp, Quaternion.identity));
            }
        }
        size = environmentSize;
        goalPrefab = Instantiate(goalPrefab, Vector3.zero, Quaternion.identity);
        fishes = new GameObject[numFish];
        for (int i = 0; i < numFish; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-environmentSize, environmentSize),
                Random.Range(-environmentSize, environmentSize),
                Random.Range(-environmentSize, environmentSize));
            fishes[i] = Instantiate(fishPrefab, pos, Quaternion.identity);
        }
    }

    void Update()
    {
        if (Random.Range(0, 10000) < 50)
        {
            goalPos = new Vector3(Random.Range(-environmentSize, environmentSize),
                Random.Range(-environmentSize, environmentSize),
                Random.Range(-environmentSize, environmentSize));
            goalPrefab.transform.position = goalPos;
        }
    }
}