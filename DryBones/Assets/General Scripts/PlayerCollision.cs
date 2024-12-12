using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public static int _coinAmount = 1000;

    PlayerShrink _playerShrinkData;

    public static bool _isLifting = false;

    private void Start()
    {
        _playerShrinkData = gameObject.GetComponent<PlayerShrink>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            // Access the static field directly using the class name
            ++_coinAmount;
            //Debug.Log($"Coin amount: {GameManager._coinAmount}");
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("ShrinkOrb"))
        {
            float currentMinScale = _playerShrinkData.GetMinScale();
            _playerShrinkData.SetMinScale(--currentMinScale);
            Destroy(other.gameObject);
        }
    }
}
