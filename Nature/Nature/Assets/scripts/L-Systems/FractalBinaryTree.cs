using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalBinaryTree : MonoBehaviour {

	public Mesh mesh;
	public Material material;
	private int depth = 0;
	public int maxDepth;

	private static Vector3[] childDirections = {
		Vector3.up*2,
		(Vector3.up * 2f) + Vector3.right,
		(Vector3.up * 2f) - Vector3.right,
		(Vector3.up * 1) + Vector3.right,
		(Vector3.up * 1) - Vector3.right		
	};

	private static Quaternion[] childOrientations = {		
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -45f),
		Quaternion.Euler(0f, 0f, 45f),
		Quaternion.Euler(0f, 0f, -45f),
		Quaternion.Euler(0f, 0f, 45f)
	};
	
	void Start () {
		if (depth == 0){
			FractalBinaryTree tree = new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().createOne(this, 0, depth+1);
			createZero(tree, depth+1);
		}
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = material;
	}

	private FractalBinaryTree createZero(FractalBinaryTree parent, int depth){
		maxDepth = parent.maxDepth;
		this.depth = depth;
		if (depth < maxDepth){
			FractalBinaryTree tree = new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().createOne(parent, 3, depth+1);
			createZero(tree, depth+1);
			FractalBinaryTree tree2 = new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().createOne(parent, 4, depth+1);
			createZero(tree2, depth+1);
		}
		else if (depth == maxDepth){
			new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().createOne(parent, 0, depth+1);
			new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().createOne(parent, 1, depth+1);
			new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().createOne(parent, 2, depth+1);
		}
		return this;
	}

	private FractalBinaryTree createOne(FractalBinaryTree parent, int form, int depth){
		maxDepth = parent.maxDepth;
		this.depth = depth;		
		mesh = parent.mesh;
		material = parent.material;
		transform.parent = parent.transform;		
		transform.localPosition = childDirections[form];
		transform.localRotation = childOrientations[form];
		if (depth < maxDepth){
			FractalBinaryTree tree = new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().createOne(this, 0, depth+1);
			return new GameObject("Fractal Child").AddComponent<FractalBinaryTree>().createOne(tree, 0, depth+2);
		}
		return this;
	}
}
