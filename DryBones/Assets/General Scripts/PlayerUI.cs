using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Slider _stamina;
    [SerializeField] private TMP_Text _coinText;

    private ThirdPersonController _playerController;
    

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "Main")
        {
            UpdatePlayerInfo();
        }
    }

    private void UpdatePlayerInfo()
    {
        _playerController = GameObject.Find("Player").GetComponent<ThirdPersonController>();    
        if (_playerController != null)
        {
            _coinText.text = PlayerCollision._coinAmount.ToString();
            _stamina.maxValue = _playerController.GetMaxStamina();
            _stamina.value = _playerController.GetCurrentStamina();
        }
    }
}
