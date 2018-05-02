using UnityEngine;

namespace TuringPattern
{
    public class InhibitorActivator : MonoBehaviour
    {
        [Range(2, 512)] public int resolution = 256;
        public Gradient coloring;

        public int steps = 100;
        public float alpha = -0.005f, beta = 10f, dx = 1, dt = 0.001f, da = 1, db = 100;
        public bool IsAnimated = true;

        private Texture2D _texture;
        private float[,] _activator, _inhibitor;
        private int _c;
        public GameObject inhibitorObject;

        private void Awake()
        {
            _texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
            _texture.name = "InhivatorActivator";
            //texture.wrapMode = TextureWrapMode.Clamp;
            //texture.filterMode = FilterMode.Trilinear;
            //texture.anisoLevel = 9;
            //GetComponent<MeshRenderer>().material.mainTexture = texture;
            if (IsAnimated)
                GetComponent<SkinnedMeshRenderer>().material.mainTexture = _texture;
            else
                GetComponent<MeshRenderer>().material.mainTexture = _texture;
            //FillTexture();
            InitalizeMorphogens();
            if (IsAnimated)
                inhibitorObject.GetComponent<SkinnedMeshRenderer>().material.mainTexture =
                    new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
            else
                inhibitorObject.GetComponent<MeshRenderer>().material.mainTexture =
                    new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
        }

        private void InitalizeMorphogens()
        {
            _activator = new float[resolution, resolution];
            _inhibitor = new float[resolution, resolution];
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    //texture.SetPixel(x, y, Color.white * Random.value);
                    _activator[i, j] = Random.value;
                    _inhibitor[i, j] = Random.value;
                    //Debug.Log(activator[i,j]);
                }
            }
        }

        private int[] GetRolledIndices(int i, int j, int h, int v)
        {
            int a = i + h;
            int b = j + v;
            if (a < 0) a = resolution - 1;
            else if (a >= resolution) a = 0;

            if (b < 0) b = resolution - 1;
            else if (b >= resolution) b = 0;
            return new[] {a, b};
        }

        private float Laplacian(int i, int j, float[,] morphogen)
        {
            float cnt = 0f;
            int[] tmp;
            tmp = GetRolledIndices(i, j, 1, 0);
            cnt += morphogen[tmp[0], tmp[1]];
            tmp = GetRolledIndices(i, j, -1, 0);
            cnt += morphogen[tmp[0], tmp[1]];
            tmp = GetRolledIndices(i, j, 0, 1);
            cnt += morphogen[tmp[0], tmp[1]];
            tmp = GetRolledIndices(i, j, 0, -1);
            cnt += morphogen[tmp[0], tmp[1]];
            return (cnt - 4 * morphogen[i, j]) / Mathf.Pow(dx, 2);
        }

        private void Update()
        {
            if (_c >= steps) return;
            float[,] tmpA = new float[resolution, resolution];
            float[,] tmpB = new float[resolution, resolution];
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    tmpA[i, j] = _activator[i, j] +
                                 dt * (da * Laplacian(i, j, _activator) +
                                       FitzNagumoA(_activator[i, j], _inhibitor[i, j]));
                    tmpB[i, j] = _inhibitor[i, j] +
                                 dt * (db * Laplacian(i, j, _inhibitor) +
                                       FitzNagumoB(_activator[i, j], _inhibitor[i, j]));
                }
            }

            _activator = tmpA;
            _inhibitor = tmpB;
            Texture2D texture2D;
            if(IsAnimated)
                texture2D = (Texture2D) inhibitorObject.GetComponent<SkinnedMeshRenderer>().material.mainTexture;
            else
                texture2D = (Texture2D) inhibitorObject.GetComponent<MeshRenderer>().material.mainTexture;
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    //texture.SetPixel(j, i, coloring.Evaluate(activator[i,j]));
                    //texture.SetPixel(j, i, Color.white * activator[i,j]);
                    _texture.SetPixel(j, i, coloring.Evaluate(_activator[i, j]));
                    texture2D.SetPixel(j, i, coloring.Evaluate(_inhibitor[i, j]));
                }
            }

            _texture.Apply();
            texture2D.Apply();
            _c++;
        }

        private float FitzNagumoA(float a, float b)
        {
            return a - Mathf.Pow(a, 3) - b + alpha;
        }

        private float FitzNagumoB(float a, float b)
        {
            return beta * (a - b);
        }
    }
}