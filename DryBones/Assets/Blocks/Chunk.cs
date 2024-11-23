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
    MeshCollider coll;

    float scale;
    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3> ();
    List<int> triangles = new List<int> ();
    List<Vector2> uvs = new List<Vector2> ();

    byte[,,] blockMap = new byte[BlockData.chunkWidth, BlockData.chunkHeight, BlockData.chunkWidth];

    World world;

    List<Chunk> subChunks;

    public Chunk (ChunkCoord coord, World world, BlockType type, Transform parent)
    {
        // get a reference to each of these components
        this.coord = coord;
        this.world = world;
        this.type = type;
        obj = new GameObject ();
        filter = obj.AddComponent<MeshFilter> ();
        rend = obj.AddComponent<MeshRenderer> ();
        coll = obj.AddComponent<MeshCollider> ();

        // set material, parent, location, scale, name
        rend.material = world.material;
        obj.transform.SetParent (parent);
        obj.transform.position = coord.ToAbsolutePos ();
        obj.transform.localScale = new Vector3 (0.125f, 0.125f, 0.125f);
        obj.name = coord.ToString ();

        // get the placements of sub-blocks based on what type of block this is
        blockMap = type.blockMap;
    }

    // run all of the computation intensive set-up
    public void Init ()
    {
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

    // add one block to this chunk at a position (relative to the chunk)
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

    // check if there is a block at a position (relative to the chunk), using blockMap
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

    // check if a position (relative to the chunk) is within the chunk
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
        coll.sharedMesh = filter.mesh;
    }

    // divde this block into many smaller blocks
    public void Subdivide ()
    {
        if (subChunks == null)
        {
            subChunks = new List<Chunk> ();
            for (int x = 0; x < BlockData.chunkWidth; ++x)
            {
                for (int y = 0; y < BlockData.chunkHeight; ++y)
                {
                    for (int z = 0; z < BlockData.chunkWidth; ++z)
                    {
                        Chunk subChunk = new Chunk (new ChunkCoord (coord, x, y, z), world, world.blockTypes[blockMap[x, y, z]], obj.transform);
                        subChunks.Add (subChunk);
                    }
                }
            }
            world.InitChunks (subChunks, this);
        }
        else
        {
            foreach (Chunk c in subChunks)
            {
                c.SetActive (true);
            }
            Hide ();
        }
    }

    // disable the renderer
    public void Hide ()
    {
        rend.enabled = false;
    }

    // enable the renderer
    public void Show ()
    {
        rend.enabled = true;
    }

    // disable the smaller blocks inside this block, and show itself again
    public void Reunite ()
    {
        if (subChunks != null)
        {
            foreach (Chunk c in subChunks)
            {
                c.SetActive (false);
            }
        }
       Show ();
    }

    public void SetActive (bool b)
    {
        obj.SetActive (b);
    }

}

public class ChunkCoord
{
    public int x;
    public int y;
    public int z;
    public float scale;

    // creates a coordinate given the x, y, and z at a certain scale
    // the x, y, and z represent this chunk's position relative to its scale
    public ChunkCoord (int x, int y, int z, float scale)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.scale = scale;
    }

    // creates a coordinate given a parent coord and the x, y, and z to offset it by
    // the offset x, y, and z are relative to this chunk coord's new scale
    public ChunkCoord (ChunkCoord parentCoord, int offsetX, int offsetY, int offsetZ)
    {
        x = parentCoord.x * BlockData.chunkWidth + offsetX;
        y = parentCoord.y * BlockData.chunkHeight + offsetY;
        z = parentCoord.z * BlockData.chunkWidth + offsetZ;
        this.scale = parentCoord.scale / BlockData.chunkWidth; // the scale should be reduced from the parent's scale
    }

    // returns the absolute position of coord in world space
    public Vector3 ToAbsolutePos ()
    {
        return new Vector3 (x * BlockData.chunkWidth * scale, y * BlockData.chunkHeight * scale, z * BlockData.chunkWidth * scale);
    }

    public override string ToString ()
    {
        return "Chunk " + ToAbsolutePos ();
    }
}