using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public static int _coinAmount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            // Access the static field directly using the class name
            ++_coinAmount;
            //Debug.Log($"Coin amount: {GameManager._coinAmount}");
            Destroy(other.gameObject);
        }
    }
}
