using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MyGrid : MonoBehaviour {

	public int xSize, ySize;
	private Vector3[] vertices;
	private Mesh mesh;

	private void Generate(){
		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();		
		mesh.name = "Procedural Grid";
		for (int i = 0, y = 0; y <= ySize; y++)
			for (int x = 0; x <= xSize; x++, i++){
				//vertices[i] = new Vector3(x, y);
				//vertices[i] = new Vector3((x*x)-(y*y), 2*x*y);
				vertices[i] = new Vector3(x*(x/1.5f), Mathf.Abs(y - (x/2.5f)));
				uv[i] = new Vector2(x*1.0f / xSize, y*1.0f / ySize);
				tangents[i] = tangent;
			}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;
		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	/*private void OnDrawGizmos () {
		if (vertices == null) return;

		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++)
			Gizmos.DrawSphere(vertices[i], 0.1f);
	}*/

	private void Awake () {
		Generate();
	}
}
