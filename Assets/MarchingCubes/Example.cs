using ProceduralNoiseProject;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MarchingCubesProject
{
    public enum MARCHING_MODE { CUBES, TETRAHEDRON };

    public class Example : MonoBehaviour
    {
        public Material m_material;

        public MARCHING_MODE mode = MARCHING_MODE.CUBES;

        public int seed = 0;

        private List<GameObject> meshes = new List<GameObject>();

        void Start()
        {
            INoise perlin = new PerlinNoise(seed, 2.0f);
            FractalNoise fractal = new FractalNoise(perlin, 3, 1.0f);

            //Set the mode used to create the mesh.
            //Cubes is faster and creates less verts, tetrahedrons is slower and creates more verts but better represents the mesh surface.
            Marching marching = null;
            if (mode == MARCHING_MODE.TETRAHEDRON)
                marching = new MarchingTertrahedron();
            else
                marching = new MarchingCubes();

            //Surface is the value that represents the surface of mesh
            //For example the perlin noise has a range of -1 to 1 so the mid point is where we want the surface to cut through.
            //The target value does not have to be the mid point it can be any value with in the range.
            marching.Surface = 0.0f;

            //The size of voxel array.
            int width = 32;
            int height = 32;
            int length = 32;

            float[] voxels = new float[width * height * length];

            //Fill voxels with values. Im using perlin noise but any method to create voxels will work.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < length; z++)
                    {
                        float fx = x / (width - 1.0f);
                        float fy = y / (height - 1.0f);
                        float fz = z / (length - 1.0f);

                        int idx = x + y * width + z * width * height;

                        voxels[idx] = fractal.Sample3D(fx, fy, fz);
                    }
                }
            }

            List<Vector3> verts = new List<Vector3>();
            List<int> indices   = new List<int>();

            System.Diagnostics.Stopwatch measure = new System.Diagnostics.Stopwatch();
            measure.Start();

            marching.Generate(voxels, width, height, length, verts, indices);

            verts = MeshUtils.WeldVertices(verts, indices);

            measure.Stop();

            Debug.Log(string.Format("Time elapsed: {0}", measure.Elapsed));

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(verts);
            mesh.SetTriangles(indices, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = transform;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().material = m_material;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.transform.localPosition = new Vector3(-width / 2, -height / 2, -length / 2);
        }

        void Update()
        {
            transform.Rotate(Vector3.up, 10.0f * Time.deltaTime);
        }
    }
}