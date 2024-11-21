using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshRenderer rend;
    public MeshFilter filter;

    void Start()
    {
        int vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3> ();
        List<int> triangles = new List<int> ();
        List<Vector2> uvs = new List<Vector2> ();

        for (int i = 0; i < 6; ++i)
        {
            for (int j = 0; j < 6; ++j)
            {
                int triangleIndex = BlockData.triangles [i,j];
                vertices.Add (BlockData.vertices [triangleIndex]);
                triangles.Add (vertexIndex);

                uvs.Add (BlockData.uvs[j]);

                ++vertexIndex;
            }
        }

        Mesh mesh = new Mesh ();
        mesh.vertices = vertices.ToArray ();
        mesh.triangles = triangles.ToArray ();
        mesh.uv = uvs.ToArray ();

        mesh.RecalculateNormals ();

        filter.mesh = mesh;
    }
}
