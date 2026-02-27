using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private const string MASTER_PARAM = "MasterVolume";
    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SFXVolume";

    private const string PREF_MASTER = "Vol_Master";
    private const string PREF_MUSIC = "Vol_Music";
    private const string PREF_SFX = "Vol_SFX";

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip coinPickup;
    [SerializeField] private AudioClip heartContainerPickup;
    [SerializeField] private AudioClip playerDeath;
    [SerializeField] private AudioClip jump;

    private Dictionary<string, AudioClip> _sfxDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    public void Initialize()
    {
        _sfxDictionary = new Dictionary<string, AudioClip>
        {
            { "CoinPickup",  coinPickup },
            { "LifePickup",  heartContainerPickup },
            { "PlayerDeath", playerDeath },
            { "Jump",        jump }
        };

        LoadAndApplyVolumes();
    }

    public void SetMasterVolume(float value)
    {
        ApplyVolume(MASTER_PARAM, value);
        PlayerPrefs.SetFloat(PREF_MASTER, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        ApplyVolume(MUSIC_PARAM, value);
        PlayerPrefs.SetFloat(PREF_MUSIC, value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        ApplyVolume(SFX_PARAM, value);
        PlayerPrefs.SetFloat(PREF_SFX, value);
        PlayerPrefs.Save();
    }

    public float GetMasterVolume() => PlayerPrefs.GetFloat(PREF_MASTER, 1f);
    public float GetMusicVolume() => PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(PREF_SFX, 1f);

    private void LoadAndApplyVolumes()
    {
        ApplyVolume(MASTER_PARAM, GetMasterVolume());
        ApplyVolume(MUSIC_PARAM, GetMusicVolume());
        ApplyVolume(SFX_PARAM, GetSFXVolume());
    }

    private void ApplyVolume(string parameter, float linearValue)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("[AudioManager] No Audio Mixer assigned!");
            return;
        }

        float clamped = Mathf.Clamp(linearValue, 0.0001f, 1f);
        audioMixer.SetFloat(parameter, Mathf.Log10(clamped) * 20f);
    }

    public void PlaySFX(string sfxName)
    {
        if (_sfxDictionary != null && _sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            if (sfxSource && clip)
                sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SFX '{sfxName}' not found!");
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic() => musicSource?.Stop();
}