using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData
{

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

    public static readonly int[,] triangles = new int[6,6]
    {
        {0, 3, 1, 1, 3, 2}, // back face
        {5, 6, 4, 4, 6, 7}, // front face
        {3, 7, 2, 2, 7, 6}, // top face
        {1, 5, 0, 0, 5, 4}, // bottom face
        {4, 7, 0, 0, 7, 3}, // left face
        {1, 2, 5, 5, 2, 6}  // right face
    };

    public static readonly Vector2[] uvs= new Vector2[6]
    {
        new Vector2 (0f, 0f),
        new Vector2 (0f, 1f),
        new Vector2 (1f, 0f),
        new Vector2 (1f, 0f),
        new Vector2 (0f, 1f),
        new Vector2 (1f, 1f)
    };

}
