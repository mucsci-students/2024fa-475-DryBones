using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public MeshRenderer rend;
    public MeshFilter filter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3> ();
    List<int> triangles = new List<int> ();
    List<Vector2> uvs = new List<Vector2> ();

    bool[,,] blockMap = new bool[BlockData.width, BlockData.height, BlockData.width];

    void Start()
    {
        // decide where there will be blocks in this chunk
        PopulateBlockMap ();

        // create the data necessary to create a mesh
        CreateBlockData ();

        // create the mesh
        CreateMesh ();
    }

    // set blockMap to true where there should be a block
    void PopulateBlockMap ()
    {
        for (int y = 0; y < BlockData.height; ++y)
        {
            for (int x = 0; x < BlockData.width; ++x)
            {
                for (int z = 0; z < BlockData.width; ++z)
                {
                    blockMap[x, y, z] = true;
                }
            }
        }
    }

    // generate mesh data for all blocks in this chunk
    void CreateBlockData ()
    {
        for (int y = 0; y < BlockData.height; ++y)
        {
            for (int x = 0; x < BlockData.width; ++x)
            {
                for (int z = 0; z < BlockData.width; ++z)
                {
                    AddBlockData (new Vector3 (x, y, z));
                }
            }
        }
    }

    // add one block to this chunk at a position
    void AddBlockData (Vector3 pos)
    {
        for (int i = 0; i < 6; ++i)
        {
            if (!HasBlock (pos + BlockData.faces[i]))
            {
                // draw 2 triangles, using only 4 vertices
                for (int j = 0; j < 4; ++j)
                {
                    int triangleIndex = BlockData.triangles [i, j];
                    vertices.Add (BlockData.vertices [triangleIndex] + pos);

                    uvs.Add (BlockData.uvs[j]);
                }

                triangles.Add (vertexIndex + 0);
                triangles.Add (vertexIndex + 1);
                triangles.Add (vertexIndex + 2);
                triangles.Add (vertexIndex + 2);
                triangles.Add (vertexIndex + 1);
                triangles.Add (vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }

    // check if there is a block at a position, using blockMap
    // can accept positions that are out of bounds
    bool HasBlock (Vector3 pos)
    {
        int x = (int) pos.x;
        int y = (int) pos.y;
        int z = (int) pos.z;

        if (x < 0 || x >= BlockData.width || y < 0 || y >= BlockData.height || z < 0 || z >= BlockData.width)
            return false;

        return blockMap[x, y, z];
    }

    // create a mesh for the chunk, using data generated from CreateBlockData()
    void CreateMesh ()
    {
        Mesh mesh = new Mesh ();
        mesh.vertices = vertices.ToArray ();
        mesh.triangles = triangles.ToArray ();
        mesh.uv = uvs.ToArray ();

        mesh.RecalculateNormals ();

        filter.mesh = mesh;
    }

}
