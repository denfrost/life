using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sierpinski : MonoBehaviour {

	public Mesh mesh;
	public Material material;
	private int depth = 0;
	public int maxDepth;

	private static Vector3[] childDirections = {
		Vector3.up*2,
		(Vector3.up * 2) - Vector3.right,
		(Vector3.up * 2) + Vector3.right			
	};

	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, 60f),
		Quaternion.Euler(0f, 0f, -60f)			
	};
	
	void Start () {
		if (depth == 0){
			createA(this, 0, depth+1);			
		}
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = material;
	}

	private Sierpinski createA(Sierpinski parent, int form, int depth){
		maxDepth = parent.maxDepth;
		this.depth = depth;
		if (depth < maxDepth){
			Sierpinski tree = createB(parent, form, depth+1);
			Sierpinski tree2 = createA(tree, 2, depth+1);
			Sierpinski tree3 = createB(tree2, 2, depth+1);
			return tree3;
		} 
		return new GameObject("A"+depth).AddComponent<Sierpinski>().create(parent, form, depth);
	}

	private Sierpinski createB(Sierpinski parent, int form, int depth){
		maxDepth = parent.maxDepth;
		this.depth = depth;
		if (depth < maxDepth){
			Sierpinski tree = createA(parent, form, depth+1);
			Sierpinski tree2 = createB(tree, 1, depth+1);
			Sierpinski tree3 = createA(tree2, 1, depth+1);
			return tree3;
		}		 
		return new GameObject("B"+depth).AddComponent<Sierpinski>().create(parent, form, depth);
	}

	private Sierpinski create(Sierpinski parent, int form, int depth){
		maxDepth = parent.maxDepth;
		this.depth = depth;		
		mesh = parent.mesh;
		material = parent.material;
		transform.parent = parent.transform;		
		transform.localPosition = childDirections[form];
		transform.localRotation = childOrientations[form];
		transform.localScale = Vector3.one;
		return this;
	}
}
