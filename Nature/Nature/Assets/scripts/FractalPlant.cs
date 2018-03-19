using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalPlant : MonoBehaviour {

	public Mesh mesh;
	public Material material;
	private int depth = 0;
	public int maxDepth;

	private static Vector3[] childDirections = {
		Vector3.up*2,
		(Vector3.up * 2) + Vector3.right,
		(Vector3.up * 2) - Vector3.right,
		(Vector3.up * 1) + Vector3.right,
		(Vector3.up * 1) - Vector3.right		
	};

	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -25f),
		Quaternion.Euler(0f, 0f, 25f),
		Quaternion.Euler(0f, 0f, -25f),
		Quaternion.Euler(0f, 0f, 25f),
	};
	
	void Start () {
		if (depth == 0){
			FractalPlant tree = new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(this, 0, depth+1);
			createZero(tree, depth+1, 0);
		}
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = material;
	}

	private FractalPlant createZero(FractalPlant parent, int form, int depth){
		maxDepth = parent.maxDepth;
		this.depth = depth;				
		if (depth < maxDepth){
			FractalPlant tree1 = new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(parent, 0+form, depth+1);
			createZero(tree1, 1, depth+1);
			createZero(tree1, 2, depth+1);
			FractalPlant tree2 = new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(tree1, 0+form, depth+1);
			createZero(tree2, 1, depth+1);
			FractalPlant tree3 = new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(tree2, 1+form, depth+1);
			createZero(tree3, 1, depth+1);
		}
		else if (depth == maxDepth){
			new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(parent, 0, depth+1);
			new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(parent, 0, depth+1);
			new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(parent, 2, depth+1);
		}
		return this;
	}
	
	private FractalPlant createOne(FractalPlant parent, int form, int depth){
		maxDepth = parent.maxDepth;
		this.depth = depth;		
		mesh = parent.mesh;
		material = parent.material;
		transform.parent = parent.transform;		
		transform.localPosition = childDirections[form];
		transform.localRotation = childOrientations[form];
		if (depth < maxDepth){
			FractalPlant tree = new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(this, 0, depth+1);
			return new GameObject("Fractal Child").AddComponent<FractalPlant>().createOne(tree, 0, depth+2);
		}
		return this;
	}
}
