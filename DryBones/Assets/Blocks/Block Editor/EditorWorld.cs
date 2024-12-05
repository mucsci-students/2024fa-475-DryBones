using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorWorld : MonoBehaviour
{
    public BlockType block;

    public void AddBlock (Vector3 loc, byte id)
    {
        block.subBlockLocs.Add (new Vector3 ( Mathf.RoundToInt (loc.x), Mathf.RoundToInt (loc.y), Mathf.RoundToInt (loc.z)));
        block.subBlockIDs.Add (id);
    }
}
