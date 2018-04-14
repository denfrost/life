using UnityEngine;
using System.Collections;

public class InhibitorActivator : MonoBehaviour{
    
    [Range(2, 512)]
    public int resolution = 256;
    public Gradient coloring;

    public int steps = 100;
    public float alpha = -0.005f, beta = 10f, dx = 1, dt = 0.001f, da = 1, db = 100;
    
    private Texture2D texture;
    private float[,] activator, inhibitor;
    
    private void Awake () {
        texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
        texture.name = "InhivatorActivator";
        texture.wrapMode = TextureWrapMode.Clamp;        
        texture.filterMode = FilterMode.Trilinear;
        texture.anisoLevel = 9;
        GetComponent<MeshRenderer>().material.mainTexture = texture;
        FillTexture();
    }
    
    private float NormalDist(){
        float u1 = 1f - Random.value; //uniform(0,1] random doubles
        float u2 = 1f - Random.value;
        return 0 + 0.05f * Mathf.Sqrt(-2f * Mathf.Log(u1)) *
                               Mathf.Sin(2f * Mathf.PI * u2); //random normal(0,1)        
        //mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
    }
    
    private void InitalizeMorphogens(){
        activator = new float[resolution, resolution];
        inhibitor = new float[resolution, resolution];
        for (int i = 0; i < resolution; i++){  
            for (int j = 0; j < resolution; j++){
                //texture.SetPixel(x, y, Color.white * Random.value);
                activator[i,j] = NormalDist();
                inhibitor[i,j] = NormalDist();
            }
        }    
    }

    private int[] getRolledIndices(int i, int j, int h, int v){        
        int a = i + h;
        int b = j + v;
        if (a < 0) a = resolution - 1;
        else if (a >= resolution) a = 0;
        
        if (b < 0) b = resolution - 1;
        else if (b >= resolution) b = 0;
        return new []{a, b};
    }

    private float laplecian(int i, int j, float[,] morphogen)
    {
        float cnt = 0f;
        int[] tmp;
        tmp = getRolledIndices(i, j, 1, 0);
        cnt += morphogen[tmp[0], tmp[1]];
        tmp = getRolledIndices(i, j, -1, 0);
        cnt += morphogen[tmp[0], tmp[1]];
        tmp = getRolledIndices(i, j, 0, 1);
        cnt += morphogen[tmp[0], tmp[1]];
        tmp = getRolledIndices(i, j, 0, -1);
        cnt += morphogen[tmp[0], tmp[1]];
        return (cnt - 4 * morphogen[i, j]) / Mathf.Pow(dx, 2);
    }
    
    private void FillTexture(){
        InitalizeMorphogens();
        for (int c = 0; c < steps; c++)
        {
            float[,] tmpA = new float[resolution,resolution];
            float[,] tmpB = new float[resolution,resolution];
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    tmpA[i, j] = activator[i, j] +
                                 dt * (da * laplecian(i, j, activator) + fitzNagumoA(activator[i, j], inhibitor[i, j]));
                    tmpB[i, j] = inhibitor[i, j] +
                                 dt * (db * laplecian(i, j, inhibitor) + fitzNagumoB(activator[i, j], inhibitor[i, j]));
                }
            }

            activator = tmpA;
            inhibitor = tmpB;
        }
        for (int i = 0; i < resolution; i++){  
            for (int j = 0; j < resolution; j++){
                texture.SetPixel(j, i, coloring.Evaluate(activator[i,j]));                
            }
        }
        texture.Apply();
    }

    private float fitzNagumoA(float a, float b)
    {
        return a - Mathf.Pow(a, 3) - b + alpha;
    }
    
    private float fitzNagumoB(float a, float b)
    {
        return beta * (a - b);
    }
}