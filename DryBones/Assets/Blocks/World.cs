/*
    The World class stores all of the possible blocks, as well as the starting block
    It also instantiates new chunks in a coroutine (to avoid lag)
    The BlockType class represents a type of block (default block, Menger sponge, etc)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;
    public int startingBlockID = 1;
    public Chunk startingChunk;
    public List<List<Chunk>> chunksToInit = new List<List<Chunk>> ();
    public List<Chunk> chunksToHide = new List<Chunk> ();
    bool runningCoroutine = false;

    [SerializeField] private PlayerShrink playerShrink;
    [SerializeField] private int subdivisionSpeed; // how many chunks are subdivided per frame (probably 10-100)

    void Start ()
    {
        foreach (BlockType b in blockTypes)
        {
            if (b.subBlockLocs.Count == 0 && b.subBlockIDs.Count == 1)
            {
                byte id = b.subBlockIDs[0];
                for (int x = 0; x < BlockData.chunkWidth; ++x)
                {
                    for (int y = 0; y < BlockData.chunkWidth; ++y)
                    {
                        for (int z = 0; z < BlockData.chunkWidth; ++z)
                        {
                            b.blockMap[x, y, z] = id;
                        }
                    }
                }
            }
            else
            {
                bool oneID = b.subBlockIDs.Count == 1;
                for (int i = 0; i < b.subBlockLocs.Count; ++i)
                {
                    Vector3 v = b.subBlockLocs[i];
                    if (oneID)
                        b.blockMap[(int) v.x, (int) v.y, (int) v.z] = b.subBlockIDs[0];
                    else
                        b.blockMap[(int) v.x, (int) v.y, (int) v.z] = b.subBlockIDs[i];
                }
            }
        }

        startingChunk = new Chunk (new ChunkCoord (0, 0, 0, 0), this, blockTypes[startingBlockID], transform);
        List<Chunk> temp = new List<Chunk> ();
        temp.Add (startingChunk);
        InitChunks (temp);
    }

    // call the Init() function on a bunch of chunks, in a coroutine
    public void InitChunks (List<Chunk> chunks, Chunk parentChunk)
    {
        chunksToInit.Add (new List<Chunk> (chunks));
        chunksToHide.Add (parentChunk);
        if (!runningCoroutine)
        {
            StartCoroutine ("InitCoroutine");
        }
    }

    public void InitChunks (List<Chunk> chunks)
    {
        InitChunks (chunks, null);
    }

    // a coroutine that calls Init() on each chunk gradually to avoid lag
    IEnumerator InitCoroutine ()
    {
        runningCoroutine = true;
        int t = 0;
        while (chunksToInit.Count > 0)
        {
            Chunk parent = null;
            if (chunksToHide.Count > 0)
                parent = chunksToHide[0];
            while ((parent == null || parent.subdivided) && chunksToInit[0].Count > 0)
            {
                if (chunksToInit[0][0].coord != null)
                {
                    chunksToInit[0][0].Init ();
                    if (t == subdivisionSpeed)
                    {
                        t = 0;
                        yield return null;
                    }
                    ++t;
                }
                chunksToInit[0].RemoveAt (0);
                if (parent != null)
                    ++parent.initialized;
            }
            chunksToInit.RemoveAt (0);
            if (parent != null && parent.subdivided) // may be null, ie. no parent
                parent.Hide ();
            if (chunksToHide.Count > 0)
                chunksToHide.RemoveAt (0);
                //print ("What??" + toStringList<List<Chunk>>(chunksToInit) + "\n" + toStringList<Chunk>(chunksToHide));
            else
                chunksToHide.Clear ();
        }
        runningCoroutine = false;
    }

    public void UpdateActiveChunks (List<ChunkCoord> activeCoords)
    {
        startingChunk.UpdateActiveChunks (activeCoords);
    }

    public int GetCurrentSize ()
    {
        return playerShrink.size;
    }

    public string toStringList<T> (List<T> things)
    {
        string str = "[";
        foreach (T t in things)
        {
            if (t == null)
            {
                str += "null";
            }
            if (t is IEnumerable<object> nestedList)
            {
                str += toStringList<object> (nestedList.ToList ());
            }
            else
            {
                str += t + ", ";
            }
        }
        return str + "](" + things.Count + ")";
    }
} 

[System.Serializable]
public class BlockType 
{
    public string blockName;
    public bool isSolid;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    /*
        There are 3 ways to define where blocks go in a chunk:

        1) One subBlockID, empty subBlockLocs
        The chunk gets completely filled with the block in subBlockIDs

        2) One subBlockID, filled subBlockLocs
        The block in subBlockIDs is placed at every location in subBlockLocs

        3) Filled subBlockIDs & subBlockLocs
        Each block in subBlockIDs is placed at its corresponding location in subBlockLocs
    */
    public List<byte> subBlockIDs;
    public List<Vector3> subBlockLocs;

    // the placement of sub-blocks in this block
    public byte[,,] blockMap = new byte[BlockData.chunkWidth, BlockData.chunkWidth, BlockData.chunkWidth];

    public int GetTextureID (int faceIndex)
    {
        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log ("Invalid faceIndex in GetTextureID().");
                return 0;
        }
    }
}

