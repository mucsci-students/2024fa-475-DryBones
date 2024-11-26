/*
    The Chunk class epresents one chunk (a subdivided block)
    It instantiates a GameObject into the scene when the constructor is called
    The ChunkCoord class is a way of storing location in a fractal world
    The coordinates should never be negative
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;
    // TODO: maybe don't call UpdateActiveChunk() on every child...
    //public Dictionary<ChunkCoord, Chunk> children;

    BlockType type;
    GameObject obj;
    MeshRenderer rend;
    MeshFilter filter;
    MeshCollider coll;

    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3> ();
    List<int> triangles = new List<int> ();
    List<Vector2> uvs = new List<Vector2> ();
    bool isSolid;

    byte[,,] blockMap = new byte[BlockData.chunkWidth, BlockData.chunkWidth, BlockData.chunkWidth];

    World world;

    List<Chunk> subChunks;
    public bool subdivided = false; // whether the chunk has a mesh or not

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
        obj.transform.position = world.transform.position + coord.ToVector3 () * Mathf.Pow (BlockData.chunkWidth, -(world.GetCurrentSize () - coord.size - 1f));
        obj.transform.localScale = new Vector3 (1f / BlockData.chunkWidth, 1f / BlockData.chunkWidth, 1f / BlockData.chunkWidth);
        obj.name = coord.ToString ();
        isSolid = type.isSolid;

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
        for (int y = 0; y < BlockData.chunkWidth; ++y)
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
        if (x < 0 || x >= BlockData.chunkWidth || y < 0 || y >= BlockData.chunkWidth || z < 0 || z >= BlockData.chunkWidth)
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
        if (isSolid)
            coll.sharedMesh = filter.mesh;
    }

    // TODO: divide this chunk if it is the active chunk, or call this method on the child that is
    public void UpdateActiveChunks (List<ChunkCoord> activeCoords)
    {
        bool subdivide = false;
        bool reunite = false;
        foreach (ChunkCoord c in activeCoords)
        {
            if (c.size >= coord.size)
            {
                reunite = true;
            }
            if (coord.Contains (c))
            {
                subdivide = true;
            }
        }
        if (subdivide)
        {
            Subdivide ();
            foreach (Chunk c in subChunks)
            {
                c.UpdateActiveChunks (activeCoords);
            }
        }
        else if (reunite)
        {
            Reunite ();
            if (subChunks != null)
            {
                foreach (Chunk c in subChunks)
                {
                    c.UpdateActiveChunks (activeCoords);
                }
            }
        }
        /*
        if (activeCoord.size > coord.size)
        {
            Reunite ();
            if (subChunks != null)
            {
                foreach (Chunk c in subChunks)
                {
                    c.UpdateActiveChunk (activeCoord);
                }
            }
        }
        else if (activeCoord.size < coord.size && coord.Contains (activeCoord))
        {
            Subdivide ();
            foreach (Chunk c in subChunks)
            {
                c.UpdateActiveChunk (activeCoord);
            }
        }
        else if (activeCoord.size == coord.size)
        {
            if (activeCoord.x == coord.x && activeCoord.y == coord.y && activeCoord.z == coord.z)
            {
                Subdivide ();
                foreach (Chunk c in subChunks)
                {
                    c.UpdateActiveChunk (activeCoord);
                }
            }
            else
            {
                Reunite ();
                if (subChunks != null)
                {
                    foreach (Chunk c in subChunks)
                    {
                        c.UpdateActiveChunk (activeCoord);
                    }
                }
            }
            
        }
        */
    }

    // divde this block into many smaller blocks
    public void Subdivide ()
    {
        subdivided = true;
        if (subChunks == null)
        {
            subChunks = new List<Chunk> ();
            for (int x = 0; x < BlockData.chunkWidth; ++x)
            {
                for (int y = 0; y < BlockData.chunkWidth; ++y)
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

    // disable the mesh
    public void Hide ()
    {
        rend.enabled = false;
        coll.enabled = false;
    }

    // enable the mesh
    public void Show ()
    {
        rend.enabled = true;
        coll.enabled = true;
    }

    // disable the smaller blocks inside this block, and show itself again
    public void Reunite ()
    {
        subdivided = false;
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
    public int size;

    // creates a coordinate given the x, y, and z at a certain size
    // the x, y, and z represent this chunk's position relative to its size
    public ChunkCoord (int x, int y, int z, int size)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.size = size;
    }

    // creates a coordinate given a parent coord and the x, y, and z to offset it by
    // the offset x, y, and z are relative to this chunk coord's new size
    public ChunkCoord (ChunkCoord parentCoord, int offsetX, int offsetY, int offsetZ)
    {
        x = parentCoord.x * BlockData.chunkWidth + offsetX;
        y = parentCoord.y * BlockData.chunkWidth + offsetY;
        z = parentCoord.z * BlockData.chunkWidth + offsetZ;
        this.size = parentCoord.size - 1; // the size should be one less that the parent's size
    }

    // checks if this ChunkCoord contains the other ChunkCoord
    public bool Contains (ChunkCoord other)
    {
        if (other.size < size)
        {
            int relativeScale = (int) Mathf.Pow (BlockData.chunkWidth, size - other.size);
            if (x == other.x / relativeScale && y == other.y / relativeScale && z == other.z / relativeScale)
                return true;
        }
        return Equals (other);
    }

    public bool Equals (ChunkCoord other)
    {
        if (other.x == x && other.y == y && other.z == z && other.size == size)
        {
            return true;
        }
        return false;
    }

    // returns the absolute position of coord
    public Vector3 ToVector3 ()
    {
        return new Vector3 (x, y, z);
    }

    public override string ToString ()
    {
        return "Chunk " + "(" + x + ", " + y + ", " + z + ") " + size;
    }
}