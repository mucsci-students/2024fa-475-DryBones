/*
    The World class stores all of the possible blocks, as well as the starting block
    It also instantiates new chunks in a coroutine (to avoid lag)
    The BlockType class represents a type of block (default block, Menger sponge, etc)
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;
    public int startingBlockID = 1;
    public List<Chunk> startingChunks = new List<Chunk> ();
    public List<List<Chunk>> chunksToInit = new List<List<Chunk>> ();
    public List<Chunk> chunksToHide = new List<Chunk> ();
    bool runningCoroutine = false;
    public Vector3 position = new Vector3 (0f, 0f, 0f);

    private float scale;

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

        scale = transform.localScale.x;
        for (int i = -1; i <= 1; ++i)
        {
            for (int j = -1; j <= 1; ++j)
            {
                for (int k = -1; k <= 1; ++k)
                {
                    startingChunks.Add (new Chunk (new ChunkCoord (i, j, k, scale / 8f), this, blockTypes[startingBlockID], transform));
                }
            }
        }
        InitChunks (startingChunks);
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
        while (chunksToInit.Count > 0)
        {
            while (chunksToInit[0].Count > 0)
            {
                chunksToInit[0][0].Init ();
                chunksToInit[0].RemoveAt (0);
                yield return null;
            }
            chunksToInit.RemoveAt (0);
            if (chunksToHide[0] != null) // may be null, ie. no parent
                chunksToHide[0].Hide ();
            chunksToHide.RemoveAt (0);
        }
        runningCoroutine = false;
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
    public byte[,,] blockMap = new byte[BlockData.chunkWidth, BlockData.chunkHeight, BlockData.chunkWidth];

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

