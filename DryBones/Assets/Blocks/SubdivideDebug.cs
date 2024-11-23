using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubdivideDebug : MonoBehaviour
{

    public List<Chunk> chunks;
    public bool reunite = false;

    void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Space))
        {
            if (chunks == null)
            {
                chunks = GameObject.Find ("World").GetComponent<World> ().startingChunks;
                if (chunks == null)
                {
                    print ("Could not find starting chunk.");
                }
                else
                {
                    print ("Found these chunks: " + chunks);
                }
            }
            else
            {
                if (reunite)
                {
                    foreach (Chunk c in chunks)
                        c.Reunite ();
                    print ("reunited chunks");
                }
                else
                {
                    foreach (Chunk c in chunks)
                        c.Subdivide ();
                    print ("subdivided chunks");
                }
                reunite = !reunite;
            }
        }
    }
}
