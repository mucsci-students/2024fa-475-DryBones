/*
    Represents one chunk (a subdivided block)
    Instantiates a GameObject into the scene when the constructor is called
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;

    BlockType type;
    GameObject obj;
    MeshRenderer rend;
    MeshFilter filter;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3> ();
    List<int> triangles = new List<int> ();
    List<Vector2> uvs = new List<Vector2> ();

    byte[,,] blockMap = new byte[BlockData.chunkWidth, BlockData.chunkHeight, BlockData.chunkWidth];

    World world;

    public Chunk (ChunkCoord coord, World world, BlockType type)
    {
        // get a reference to each of these components
        this.coord = coord;
        this.world = world;
        this.type = type;
        obj = new GameObject ();
        filter = obj.AddComponent<MeshFilter> ();
        rend = obj.AddComponent<MeshRenderer> ();

        // set material, parent, location, name
        rend.material = world.material;
        obj.transform.SetParent (world.transform);
        obj.transform.position = coord.ToVector3 ();
        obj.name = coord.ToString ();

        // get the placements of sub-blocks based on what type of block this is
        blockMap = type.blockMap;

        // create the data necessary to create a mesh
        CreateBlockData ();

        // create the mesh
        CreateMesh ();
    }

    // generate mesh data for all blocks in this chunk
    void CreateBlockData ()
    {
        for (int y = 0; y < BlockData.chunkHeight; ++y)
        {
            for (int x = 0; x < BlockData.chunkWidth; ++x)
            {
                for (int z = 0; z < BlockData.chunkWidth; ++z)
                {
                    AddBlockData (new Vector3 (x, y, z));
                }
            }
        }
    }

    // add one block to this chunk at a position
    void AddBlockData (Vector3 pos)
    {
        byte blockID = blockMap[(int) pos.x, (int) pos.y, (int) pos.z];

        // air blocks do not need to be added
        if (blockID == 0)
            return;

        for (int i = 0; i < 6; ++i)
        {
            if (!HasBlock (pos + BlockData.faces[i]))
            {
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

        if (InChunk (x, y, z))
            return world.blockTypes[blockMap[x, y, z]].isSolid;

        return false;
    }

    // check if a position is within the chunk
    bool InChunk (int x, int y, int z)
    {
        if (x < 0 || x >= BlockData.chunkWidth || y < 0 || y >= BlockData.chunkHeight || z < 0 || z >= BlockData.chunkWidth)
            return false;
        return true;
    }

    // adds the uvs of a certain texture for one face
    // should be called in the order: Back, Front, Top, Bottom, Left, Right
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

public class ChunkCoord
{
    public int x;
    public int y;
    public int z;

    public ChunkCoord (int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 ToVector3 ()
    {
        return new Vector3 (x * BlockData.chunkWidth, y * BlockData.chunkHeight, z * BlockData.chunkWidth);
    }

    public override string ToString ()
    {
        return "Chunk " + ToVector3 ();
    }
}