using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animals : MonoBehaviour {
	public float x;

	private void Awake(){
		Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
		Vector3 a;
        for (int i = 0; i < vertices.Length; i++) {
			a = vertices[i];
			//vertices[i] = new Vector3(a.x+(x*a.z), a.y, a.z);
			//vertices[i] = new Vector3(a.x + x*a.z, a.y, a.z);
			vertices[i] = new Vector3(Mathf.Pow(a.x,2)-Mathf.Pow(a.z,2), a.y, 2*a.x*a.z);
		}
        mesh.vertices = vertices;
		mesh.RecalculateBounds();
	}
}
