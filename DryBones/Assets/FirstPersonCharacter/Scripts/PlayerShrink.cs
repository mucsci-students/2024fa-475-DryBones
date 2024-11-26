using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShrink : MonoBehaviour
{
    // positive size increases the player's scale, negative size decreases it
    public int startingSize = 0;
    public int size { get; private set; }
    float startingScale;
    public GameObject world;
    public GameObject worldParent;
    ChunkCoord currChunkCoord;
    ChunkCoord oldChunkCoord;

    bool scrolling = false;
    float timeOfLastScroll = 0f;
    float scrollingCooldown = 0.1f;
    float sizeChange = 0f;


    void Start()
    {
        startingScale = worldParent.transform.localScale.x;
        size = startingSize;
        currChunkCoord = new ChunkCoord ((int) transform.position.x, (int) transform.position.y, (int) transform.position.z, size);
        oldChunkCoord = new ChunkCoord ((int) transform.position.x, (int) transform.position.y, (int) transform.position.z, size);
        world.transform.parent = worldParent.transform;
        worldParent.transform.localScale = CurrentScale ();
        world.transform.parent = null;
    }

    void Update()
    {
        float scrollAmt = Input.GetAxis ("Mouse ScrollWheel") / 25f;
        if (scrollAmt != 0)
        {
            // shrinking begins, time freezes
            if (!scrolling)
            {
                StopCoroutine ("InterpolateToCurrentScale");
                world.transform.parent = worldParent.transform;
                scrolling = true;
            }
            timeOfLastScroll = Time.unscaledTime;
            Time.timeScale = 0f;
            sizeChange += scrollAmt;
            worldParent.transform.localScale *=  Mathf.Pow (8f, -scrollAmt);

            // adjust the size as the player scrolls
            if (sizeChange > 0.5f) 
            {
                size = size + 1;
                --sizeChange;
                //print (size);
            }
            else if (sizeChange < -0.5f)
            {
                size = size - 1;
                ++sizeChange;
                //print (size);
            }
        }
        else if (scrolling && timeOfLastScroll + scrollingCooldown < Time.unscaledTime)
        {
            scrolling = false;
            StartCoroutine ("InterpolateToCurrentScale");
            //print (size);
        }
        UpdateChunkCoord ();
    }

    // TODO: fix this hideousness
    void UpdateChunkCoord ()
    {
        Vector3 posRelativeToWorld = transform.position - world.transform.position;
        currChunkCoord = new ChunkCoord ((int) (posRelativeToWorld.x / BlockData.chunkWidth), (int) (posRelativeToWorld.y / BlockData.chunkWidth), (int) (posRelativeToWorld.z / BlockData.chunkWidth), size);
        if (Time.timeScale == 1f && (currChunkCoord.size != oldChunkCoord.size || currChunkCoord.x != oldChunkCoord.x || currChunkCoord.y != oldChunkCoord.y || currChunkCoord.z != oldChunkCoord.z))
        {
            List<ChunkCoord> coords = new List<ChunkCoord> ();
            coords.Add (new ChunkCoord ((int) (posRelativeToWorld.x / BlockData.chunkWidth), (int) (posRelativeToWorld.y / BlockData.chunkWidth) - 1, (int) (posRelativeToWorld.z / BlockData.chunkWidth), size));
            coords.Add (new ChunkCoord ((int) (posRelativeToWorld.x / BlockData.chunkWidth), (int) (posRelativeToWorld.y / BlockData.chunkWidth), (int) (posRelativeToWorld.z / BlockData.chunkWidth), size));
            world.GetComponent<World> ().UpdateActiveChunks (coords);
            oldChunkCoord = currChunkCoord;
        }
    }

    // get the player's current scale based on their size
    // increases or decreases the return value based on the offset
    Vector3 CurrentScale ()
    {
        return new Vector3 (1f, 1f, 1f) * startingScale * Mathf.Pow (8f, -size);
    }

    IEnumerator InterpolateToCurrentScale ()
    {
        float currScale = worldParent.transform.localScale.x;
        float newScale = CurrentScale ().x;
        float oldSizeChange = sizeChange;
        float duration = 0.35f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float step = Mathf.SmoothStep (0f, 1f, elapsed / duration);
            worldParent.transform.localScale = new Vector3 (1f, 1f, 1f) * Mathf.Lerp (currScale, newScale, step);
            sizeChange = Mathf.Lerp (oldSizeChange, 0f, step);
            yield return null;
        }
        worldParent.transform.localScale = CurrentScale ();
        Time.timeScale = 1f;
        world.transform.parent = null;
        // shrinking ends, time resumes
    }
}
