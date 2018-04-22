using UnityEngine;

public class GlobalFlock : MonoBehaviour
{
	public int numFish = 10;
	public int tankSize = 5;
	public GameObject fishPrefab, goalPrefab;
	public static GameObject[] fishes;
	public static Vector3 goalPos = Vector3.zero;
	public static int size;
		
	void Start ()
	{
		size = tankSize;
		fishes = new GameObject[numFish];
		for (int i = 0; i < numFish; i++)
		{
			Vector3 pos = new Vector3(
				Random.Range(-tankSize, tankSize), 
				Random.Range(-tankSize, tankSize),
				Random.Range(-tankSize, tankSize));
			fishes[i] = Instantiate(fishPrefab, pos, Quaternion.identity);
		}
	}
		
	void Update () {
		if (Random.Range(0, 10000) < 50)
		{
			goalPos = new Vector3(Random.Range(-tankSize, tankSize), 
				Random.Range(-tankSize, tankSize),
				Random.Range(-tankSize, tankSize));
			goalPrefab.transform.position = goalPos;
		}
	}
}
