using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShrink : MonoBehaviour
{

    //debug
    [SerializeField] bool usingRecenter = true;
    [SerializeField] bool printingCurrChunk = true;
    [SerializeField] bool printingWorldPos = true;
    private Vector3 oldWorldPosition = new Vector3 (0f,0f,0f);

    // Define constants for level limits
    public float minScale = 1;
    public float maxScale = 1;

    // positive size increases the player's scale, negative size decreases it
    public int startingSize = 0;
    public int size;
    float startingScale;
    public GameObject world;
    public World worldComponent;
    public GameObject worldParent;
    ChunkCoord currChunkCoord;
    ChunkCoord oldChunkCoord;
    Vector3 startingPos;

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
        startingPos = transform.position;
    }

    void Update()
    {
        // player scrolls:
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
           scrollAmt = Mathf.Clamp (scrollAmt, -0.05f - sizeChange - (size - minScale), 0.05f - sizeChange - size + maxScale);
           sizeChange += scrollAmt;
           worldParent.transform.localScale *=  Mathf.Pow (8f, -scrollAmt);

           // adjust the size as the player scrolls
           if (sizeChange > 0.5f) 
           {
               ++size;
               --sizeChange;
           }
           else if (sizeChange < -0.5f)
           {
               --size;
               ++sizeChange;
           }
        }
        else if (scrolling && timeOfLastScroll + scrollingCooldown < Time.unscaledTime)
        {
           scrolling = false;
           StartCoroutine ("InterpolateToCurrentScale");
        }

        // player presses G or H:
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
        Vector3 rCube = RelativeCube (transform.position, currChunkCoord.ToVector3 (), BlockData.chunkWidth);
        if (rCube != Vector3.zero)
            UpdateChunkCoord ((int) rCube.x, (int) rCube.y, (int) rCube.z, size);
    }

    // returns the how the cube needs to change in order to contains a point
    public Vector3 RelativeCube (Vector3 pos1, Vector3 cubePos, float cubeLength)
    {
        Vector3 diff = pos1 - cubePos;
        return new Vector3 (Mathf.Floor(diff.x / cubeLength), Mathf.Floor(diff.y / 8f), Mathf.Floor(diff.z / 8f));
    }

    public float GetPlayerSize()
    {
        return size;
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

    void UpdateChunkCoord (int dX, int dY, int dZ, int newSize)
    {
        //currChunkCoord = new ChunkCoord ((int) (posRelativeToWorld.x / BlockData.chunkWidth), (int) (posRelativeToWorld.y / BlockData.chunkWidth), (int) (posRelativeToWorld.z / BlockData.chunkWidth), size);
        if (dX == 0 && dY == 0 && dZ == 0 && newSize == currChunkCoord.size) Debug.Log ("AAAAHHHH cant all be 0"); //debug
        if (newSize == currChunkCoord.size)
            currChunkCoord = new ChunkCoord (currChunkCoord.x + dX, currChunkCoord.y + dY, currChunkCoord.z + dZ, size);
        else if (newSize < currChunkCoord.size)
        {
            if (diff.x != 0 || diff.y != 0 || diff.z != 0) printingCurrChunk ("hmmm this one may be a little suspicious, we are changing size while moving");
            Vector3 diff = transform.position - currChunkCoord.ToVector3 ();
            currChunkCoord = new ChunkCoord (currChunkCoord, (int) diff.x, (int) diff.y, (int) diff.z);
        }
        else
        {
            if (dX != 0 || dY != 0 || dZ != 0) printingCurrChunk ("AHHH all of these should be 0 when we grow");
            float divisor = Mathf.Pow (BlockData.chunkWidth, size - currChunkCoord.size);
            currChunkCoord = new ChunkCoord (currChunkCoord.x / divisor, currChunkCoord.x / divisor, currChunkCoord.x / divisor, size);
        }

        if (printingCurrChunk && !currChunkCoord.Equals(oldChunkCoord)) print ("current chunk " + currChunkCoord);
        if (currChunkCoord.Equals(oldChunkCoord)) print ("AAAAAHHHH what is wrong with meeee");
        if (printingWorldPos && worldComponent.position != oldWorldPosition) print (worldComponent.position);
        oldWorldPosition = worldComponent.position;
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
        if (usingRecenter)
            worldComponent.Recenter();
        UpdateChunkCoord (0, 0, 0, size);
        // shrinking ends, time resumes
    }
}
