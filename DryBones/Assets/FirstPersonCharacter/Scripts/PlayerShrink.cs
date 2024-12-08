using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShrink : MonoBehaviour
{
    // Define constants for level limits
    public float minScale = 1;
    public float maxScale = 1;

    // positive size increases the player's scale, negative size decreases it
    public int startingSize = 0;
    public int size { get; private set; }
    float startingScale;
    public GameObject world;
    public World worldComponent;
    public GameObject worldParent;
    ChunkCoord currChunkCoord;
    ChunkCoord oldChunkCoord;

    bool scrolling = false;
    float timeOfLastScroll = 0f;
    float scrollingCooldown = 0.1f;
    float sizeChange = 0f;
    public float scrollSensitivity = 0.009f;
    public int chunkDepth = 2; //how far the chunks get subdivided around the player, between 2 and 5


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
        float scrollAmt = Input.GetAxis ("Mouse ScrollWheel") * scrollSensitivity;
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
           scrollAmt = Mathf.Clamp (scrollAmt, -0.05f - sizeChange - (size + minScale), 0.05f - sizeChange - size + maxScale);
           sizeChange += scrollAmt;
           worldParent.transform.localScale *=  Mathf.Pow (8f, -scrollAmt);

           // adjust the size as the player scrolls
           if (sizeChange > 0.5f) 
           {
               ++size;
               --sizeChange;
               //print (size);
           }
           else if (sizeChange < -0.5f)
           {
               --size;
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
        //ShrinkTest();
        if (Input.GetKeyDown (KeyCode.G))
        {
            if (size > minScale)
            {
                Debug.Log("Shrinking! Current size " + size);
                Debug.Log("Current min scale " + (-minScale));
                StopCoroutine("InterpolateToCurrentScale");
                world.transform.parent = worldParent.transform;
                Time.timeScale = 0f;
                --size;
                ++sizeChange;
                scrolling = false;
                StartCoroutine ("InterpolateToCurrentScale");
            }
            else
            {
                Debug.Log("Can't shrink! Current size " + size);
            }
        }
        else if (Input.GetKeyDown (KeyCode.H))
        {
            if (size < maxScale)
            {
                Debug.Log("Growing! Current size " + size);
                StopCoroutine("InterpolateToCurrentScale");
                world.transform.parent = worldParent.transform;
                Time.timeScale = 0f;
                ++size;
                --sizeChange;
                scrolling = false;
                StartCoroutine ("InterpolateToCurrentScale");
            }
        }
        UpdateChunkCoord ();
    }

    public float GetMinScale()
    {
        return minScale;
    }

    public float GetMaxScale() 
    { 
        return maxScale; 
    }

    public void SetMinScale(float newScale)
    {
        minScale = newScale;
    }

    public void SetMaxScale(float newScale)
    {
        maxScale = newScale;
    }

/*
    void ShrinkTest()
    {
        float zoomAmount = 0f;

        // Check for shrink (G key) or grow (H key)
        if (Input.GetKey(KeyCode.G))
        {
            zoomAmount = -scrollSensitivity; // Shrink
        }
        else if (Input.GetKey(KeyCode.H))
        {
            zoomAmount = scrollSensitivity; // Grow
        }

        if (zoomAmount != 0)
        {
            // Shrinking begins, time freezes
            if (!scrolling)
            {
                StopCoroutine("InterpolateToCurrentScale");
                world.transform.parent = worldParent.transform;
                scrolling = true;
            }
            timeOfLastScroll = Time.unscaledTime;
            Time.timeScale = 0f;

            // Calculate the new scale and clamp it
            float currentScale = worldParent.transform.localScale.x;
            float targetScale = Mathf.Clamp(
                currentScale * Mathf.Pow(8f, -zoomAmount),
                startingScale * Mathf.Pow(8f, -minScale),
                startingScale * Mathf.Pow(8f, maxScale)
            );

            // Apply the clamped scale
            worldParent.transform.localScale = new Vector3(targetScale, targetScale, targetScale);

            // Adjust sizeChange and update size
            sizeChange += zoomAmount;
            if (sizeChange > 0.5f && size < maxScale)
            {
                size++;
                sizeChange -= 1f;
            }
            else if (sizeChange < -0.5f && size > -minScale)
            {
                size--;
                sizeChange += 1f;
            }
        }
        else if (scrolling && timeOfLastScroll + scrollingCooldown < Time.unscaledTime)
        {
            // End zooming process
            scrolling = false;
            StartCoroutine("InterpolateToCurrentScale");
        }
    }
*/

    void UpdateChunkCoord ()
    {
        Vector3 posRelativeToWorld = transform.position - world.transform.position;
        currChunkCoord = new ChunkCoord ((int) (posRelativeToWorld.x / BlockData.chunkWidth), (int) (posRelativeToWorld.y / BlockData.chunkWidth), (int) (posRelativeToWorld.z / BlockData.chunkWidth), size);
        if (Time.timeScale == 1f && (!currChunkCoord.Equals (oldChunkCoord)))
        {
            List<ChunkCoord> coords = new List<ChunkCoord> ();
            if (chunkDepth % 2 == 0)
            {
                int r = chunkDepth / 2;
                int xOffset = currChunkCoord.x % BlockData.chunkWidth < 4 ? 0 : 1;
                int yOffset = currChunkCoord.y % BlockData.chunkWidth < 4 ? 0 : 1;
                int zOffset = currChunkCoord.z % BlockData.chunkWidth < 4 ? 0 : 1;
                for (int x = xOffset - r; x <= xOffset + r - 1; ++x)
                    for (int y = yOffset - r; y <= yOffset + r - 1; ++y)
                        for (int z = zOffset - r; z <= zOffset + r - 1; ++z)
                            coords.Add (new ChunkCoord ((int) (currChunkCoord.x / BlockData.chunkWidth) + x, (int) (currChunkCoord.y / BlockData.chunkWidth) + y, (int) (currChunkCoord.z / BlockData.chunkWidth) + z, size + 1));
            }
            else
            {
                int r = chunkDepth / 2;
                for (int x = -r; x <= r; ++x)
                    for (int y = -r; y <= r; ++y)
                        for (int z = -r; z <= r; ++z)
                            coords.Add (new ChunkCoord ((int) (currChunkCoord.x / BlockData.chunkWidth) + x, (int) (currChunkCoord.y / BlockData.chunkWidth) + y, (int) (currChunkCoord.z / BlockData.chunkWidth) + z, size + 1));
            }
            worldComponent.UpdateActiveChunks (coords);
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
