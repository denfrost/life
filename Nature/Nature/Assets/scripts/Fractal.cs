using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour {

	public Mesh mesh;
	public Material material;
	private int depth = 0;
	public int maxDepth;
	public float childScale;
	private static Material[] materials;
	private static Vector3[] childDirections = {
		Vector3.up, 
		Vector3.right, 
		Vector3.left,
		Vector3.forward,
		Vector3.back, 
		Vector3.down 
	};

	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f),
		Quaternion.identity,
	};
	
	void Start () {
		if (materials == null)
			initializaMaterials();

		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = materials[depth];
		if (depth < maxDepth)
			StartCoroutine(createChildren());
	}
	
	private IEnumerator createChildren(){
		for (int i = 0; i < childDirections.Length; i++) {
			yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
			new GameObject("Fractal Child").AddComponent<Fractal>().initialize(this, i);
		}
	}

	private void initializaMaterials(){
		materials = new Material[maxDepth+1];
		for (int i = 0; i <= maxDepth; i++){
			materials[i] = new Material(material);
			materials[i].color = Color.Lerp(Color.white, Color.yellow, i*1.0f/maxDepth);
		}		
	}

	private void initialize(Fractal parent, int i){
		mesh = parent.mesh;
		//material = parent.material;
		//materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition = childDirections[i] * (0.5f + 0.5f * childScale);
		transform.localRotation = childOrientations[i];
	}

	private void Update () {
		transform.Rotate(0f, 30 * Time.deltaTime, 0f);
	}

}
