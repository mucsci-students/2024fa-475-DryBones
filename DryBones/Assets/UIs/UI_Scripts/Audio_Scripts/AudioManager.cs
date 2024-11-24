using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    //[SerializeField] private AudioMixer _masterVolumMixer; 
    //[SerializeField] private Slider _masterVolumSlider;

    public Sound[] _themeAudio, _sfxAudio;
    public AudioSource _themeAudioSource, _sfxAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayThemeMusic(string audioName)
    {
        Sound sounds = Array.Find(_themeAudio, x => x._audioName == audioName);
        if (sounds == null)
        {
            Debug.Log("Theme Audio Not Found!");
        }
        else
        {
            _themeAudioSource.clip = sounds._audioClip;
            _themeAudioSource.Play();
        }
    }
}
