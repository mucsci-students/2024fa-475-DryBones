using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            // Access the static field directly using the class name
            GameManager._coinAmount += 1;
            //Debug.Log($"Coin amount: {GameManager._coinAmount}");
            Destroy(other.gameObject);
        }
    }
}
