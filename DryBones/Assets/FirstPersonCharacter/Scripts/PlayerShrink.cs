using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShrink : MonoBehaviour
{
    // positive size increases the player's scale, negative size decreases it
    public int startingSize = 0;
    int size;
    float startingScale;
    public GameObject world;
    public GameObject worldParent;

    bool scrolling = false;
    float timeOfLastScroll = 0f;
    float scrollingCooldown = 0.1f;
    float sizeChange = 0f;


    void Start()
    {
        startingScale = worldParent.transform.localScale.x;
        size = startingSize;
        worldParent.transform.localScale = CurrentScale (0f);
    }

    void Update()
    {
        float scrollAmt = Input.GetAxis ("Mouse ScrollWheel") / 10f;
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
            worldParent.transform.localScale *=  Mathf.Pow (8f, scrollAmt);

            // adjust the size as the player scrolls
            if (sizeChange > 0.5f) 
            {
                ++size;
                --sizeChange;
                print (size);
            }
            else if (sizeChange < -0.5f)
            {
                --size;
                ++sizeChange;
                print (size);
            }
        }
        else if (scrolling && timeOfLastScroll + scrollingCooldown < Time.unscaledTime)
        {
            scrolling = false;
            StartCoroutine ("InterpolateToCurrentScale");
            print (size);
        }
    }

    // get the player's current scale based on their size
    // increases or decreases the return value based on the offset
    Vector3 CurrentScale (float offset)
    {
        return new Vector3 (1f, 1f, 1f) * startingScale * Mathf.Pow (8f, size + offset);
    }

    IEnumerator InterpolateToCurrentScale ()
    {
        float currScale = worldParent.transform.localScale.x;
        float newScale = CurrentScale (0f).x;
        float oldSizeChange = sizeChange;
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float step = Mathf.SmoothStep (0f, 1f, elapsed / duration);
            worldParent.transform.localScale = new Vector3 (1f, 1f, 1f) * Mathf.Lerp (currScale, newScale, step);
            sizeChange = Mathf.Lerp (oldSizeChange, 0f, step);
            yield return null;
        }
        worldParent.transform.localScale = CurrentScale (0f);
        Time.timeScale = 1f;
        world.transform.parent = null;
        // shrinking ends, time resumes
    }
}
