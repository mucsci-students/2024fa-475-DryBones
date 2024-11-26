using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubdivideDebug : MonoBehaviour
{

    public Chunk chunk;
    public bool reunite = false;

    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            if (chunk == null)
            {
                chunk = GameObject.Find ("World").GetComponent<World> ().startingChunk;
                if (chunk == null)
                {
                    print ("Could not find starting chunk.");
                }
                else
                {
                    print ("Found this chunk: " + chunk);
                }
            }
            else
            {
                if (reunite)
                {
                    chunk.Reunite ();
                    print ("reunited chunks");
                }
                else
                {
                    chunk.Subdivide ();
                    print ("subdivided chunks");
                }
                reunite = !reunite;
            }
        }
    }
}
