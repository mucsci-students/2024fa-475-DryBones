/*
    Stores general data that applies to all blocks and chunks
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData
{

    public static readonly int chunkWidth = 8;
    
    // anything involving faces of the cube is done in this order:
    // Back, Front, Top, Bottom, Left, Right
    /*
        3 - - - 2
        | \     | \
        |   7 - - - 6
        |   |   |   |
        0 - | - 1   |
          \ |     \ |
            4 - - - 5
    */

    public static readonly Vector3[] vertices = new Vector3[8]
    {
        new Vector3 (0f, 0f, 0f), new Vector3 (1f, 0f, 0f), new Vector3 (1f, 1f, 0f),
        new Vector3 (0f, 1f, 0f), new Vector3 (0f, 0f, 1f), new Vector3 (1f, 0f, 1f),
        new Vector3 (1f, 1f, 1f), new Vector3 (0f, 1f, 1f)
    };

    public static readonly Vector3[] faces = new Vector3[6]
    {
        Vector3.back,
        Vector3.forward,
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right
    };

    public static readonly int[,] triangles = new int[6,4]
    {
        // 0, 1, 2, 2, 1, 3
        {0, 3, 1, 2}, // back face
        {5, 6, 4, 7}, // front face
        {3, 7, 2, 6}, // top face
        {1, 5, 0, 4}, // bottom face
        {4, 7, 0, 3}, // left face
        {1, 2, 5, 6}  // right face
    };


    public static int atlasSize = 16; // the atlas is made of 16 x 16 textures
    public static float normalizedTextureSize { // size of texture in atlas, relative to size of the atlas
        get { return 1f / (float) atlasSize; }
    }

    public static readonly Vector2[] uvs= new Vector2[4]
    {
        new Vector2 (0f, 0f),
        new Vector2 (0f, 1f),
        new Vector2 (1f, 0f),
        new Vector2 (1f, 1f)
    };

}
