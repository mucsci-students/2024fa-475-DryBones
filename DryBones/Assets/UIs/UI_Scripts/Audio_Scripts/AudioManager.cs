using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    //[SerializeField] private AudioMixer _masterVolumMixer; 
    //[SerializeField] private Slider _masterVolumSlider;

    public Sound[] _themeAudio, _ingameAudio, _sfxAudio;
    public AudioSource _themeAudioSource, _ingameAudioSource, _sfxAudioSource;

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


    private void Start()
    {
        PlayThemeMusic("AllMenuAudio");
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

    public void PlayIngameMusic(string audioName)
    {
        Sound sounds = Array.Find(_ingameAudio, x => x._audioName == audioName);
        if (sounds == null)
        {
            Debug.Log("Ingame Audio Not Found!");
        }
        else
        {
            _ingameAudioSource.clip = sounds._audioClip;
            _ingameAudioSource.Play();
        }
    }
}
