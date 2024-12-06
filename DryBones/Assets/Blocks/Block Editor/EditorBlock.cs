using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBlock : MonoBehaviour
{
    [SerializeField] EditorWorld world;
    [SerializeField] MeshRenderer rend;
    public byte id;

    // Start is called before the first frame update
    void Start()
    {
        world.AddBlock (transform.position, id);
        Random.InitState (id);
        Material materialColored = new Material(rend.material);
		materialColored.color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f));
        rend.material = materialColored;
    }


}
