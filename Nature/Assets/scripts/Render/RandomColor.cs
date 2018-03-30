using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour {

	// Use this for initialization
	void Awake()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
		Color[] colors = new Color[mesh.vertices.Length];
		int aux;
        for (int i = 0; i < mesh.vertices.Length; i++) {
			aux = Random.Range(0.0f, 1f) > 0.5 ? 0 : 0;
			colors[i] = new Color(aux, aux, aux);
		}
		mesh.colors = colors;
    }
}
