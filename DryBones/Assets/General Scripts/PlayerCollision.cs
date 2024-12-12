using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public static int _coinAmount = 0;

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
            AudioManager.Instance.PlaySFX("CoinSFX");
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

            if(other.name == "ShrinkOrbLevel1 (2)")
                PlayerPrefs.SetInt("shrink", 1);

            if(other.name == "ShrinkOrb(Clone)")
                PlayerPrefs.SetInt("shrink", 2);
            if(other.name == "ShrinkOrb(Clone) (2)")
                PlayerPrefs.SetInt("shrink",3);
        }else if(other.CompareTag("FinalOrb")){
            float currentMinScale = _playerShrinkData.GetMinScale();
            _playerShrinkData.SetMinScale(--currentMinScale);
            Destroy(other.gameObject);

            PlayerPrefs.SetInt("shrink", 4);
        }else if(other.CompareTag("TutorialOrb")){
            float currentMinScale = _playerShrinkData.GetMinScale();
            _playerShrinkData.SetMinScale(--currentMinScale);
            Destroy(other.gameObject);

            PlayerPrefs.SetInt("shrink", 0);
        }
    }
}
