using UnityEngine;

public class AffineTransform : MonoBehaviour
{

    public float[] xs = {1, 1, 1};
    public float[] ys = {1, 1, 1};
    public float[] zs = {1, 1, 1};
    public float rate = 0.5f;
    private float nextTime;
    
    private void Update(){
        if (Time.time >= nextTime)
        {
            nextTime = Time.time + rate;
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            Vector3 a;
            for (int i = 0; i < vertices.Length; i++)
            {
                a = vertices[i];
                vertices[i] = new Vector3(
                    a.x + Random.Range(-0.16f, 0.16f)*xs[0] + xs[1]*Random.Range(-1,1)*a.y + xs[2]*Random.Range(-1,1)*a.z,
                    a.y + Random.Range(-0.16f, 0.16f)*ys[0] + ys[1]*Random.Range(-1,1)*a.x + ys[2]*Random.Range(-1,1)*a.z,
                    a.z + Random.Range(-0.16f, 0.16f)*zs[0] + zs[1]*Random.Range(-1,1)*a.x + zs[2]*Random.Range(-1,1)*a.y);
            }
            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }
    }
}