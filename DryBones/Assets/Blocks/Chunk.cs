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

    byte[,,] blockMap = new byte[BlockData.width, BlockData.height, BlockData.width];

    World world;

    void Start()
    {
        // get a reference to the World object
        world = GameObject.Find ("World").GetComponent<World> ();

        // decide what blocks there will be in this chunk
        PopulateBlockMap ();

        // create the data necessary to create a mesh
        CreateBlockData ();

        // create the mesh
        CreateMesh ();
    }

    // update blockMap with the correct block ids
    void PopulateBlockMap ()
    {
        for (int y = 0; y < BlockData.height; ++y)
        {
            for (int x = 0; x < BlockData.width; ++x)
            {
                for (int z = 0; z < BlockData.width; ++z)
                {
                    blockMap[x, y, z] = 4;
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
                byte blockID = blockMap[(int) pos.x, (int) pos.y, (int) pos.z];

                // draw 2 triangles, using only 4 vertices
                for (int j = 0; j < 4; ++j)
                {
                    int triangleIndex = BlockData.triangles [i, j];
                    vertices.Add (BlockData.vertices [triangleIndex] + pos);

                }

                triangles.Add (vertexIndex + 0);
                triangles.Add (vertexIndex + 1);
                triangles.Add (vertexIndex + 2);
                triangles.Add (vertexIndex + 2);
                triangles.Add (vertexIndex + 1);
                triangles.Add (vertexIndex + 3);
                vertexIndex += 4;

                AddTexture (world.blockTypes[blockID].GetTextureID (i));
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

    return world.blockTypes[blockMap[x, y, z]].isSolid;
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

    // adds the uvs of a certain texture for one face
    void AddTexture (int id)
    {
        float y = id / BlockData.atlasSize;
        float x = id - (y * BlockData.atlasSize);

        x *= BlockData.normalizedTextureSize;
        y *= BlockData.normalizedTextureSize;

        y = 1f - y - BlockData.normalizedTextureSize;

        uvs.Add (new Vector2 (x, y));
        uvs.Add (new Vector2 (x, y + BlockData.normalizedTextureSize));
        uvs.Add (new Vector2 (x + BlockData.normalizedTextureSize, y));
        uvs.Add (new Vector2 (x + BlockData.normalizedTextureSize, y + BlockData.normalizedTextureSize));
    }

}
