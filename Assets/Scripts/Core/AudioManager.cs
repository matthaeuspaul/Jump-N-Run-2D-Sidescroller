using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    // Exposed Parameter Namen im Audio Mixer (müssen exakt übereinstimmen!)
    private const string MASTER_PARAM = "MasterVolume";
    private const string MUSIC_PARAM = "MusicVolume";
    private const string SFX_PARAM = "SFXVolume";

    // PlayerPrefs Keys
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

    // -------------------------------------------------------
    // Singleton & Lifecycle
    // -------------------------------------------------------

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

    // -------------------------------------------------------
    // Volume – Public API (Werte 0.0001 – 1, von Sliders)
    // -------------------------------------------------------

    /// <summary>Wird vom SettingsMenuController via Slider aufgerufen (0–1).</summary>
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

    /// <summary>Gibt den gespeicherten Wert zurück (0–1) – Default 1.</summary>
    public float GetMasterVolume() => PlayerPrefs.GetFloat(PREF_MASTER, 1f);
    public float GetMusicVolume() => PlayerPrefs.GetFloat(PREF_MUSIC, 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(PREF_SFX, 1f);

    // -------------------------------------------------------
    // Internes
    // -------------------------------------------------------

    private void LoadAndApplyVolumes()
    {
        ApplyVolume(MASTER_PARAM, GetMasterVolume());
        ApplyVolume(MUSIC_PARAM, GetMusicVolume());
        ApplyVolume(SFX_PARAM, GetSFXVolume());
    }

    /// <summary>Konvertiert linearen Wert (0–1) in Dezibel und setzt den Mixer-Parameter.</summary>
    private void ApplyVolume(string parameter, float linearValue)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("[AudioManager] Kein Audio Mixer zugewiesen!");
            return;
        }

        // Clamp verhindert log(0) → -Infinity
        float clamped = Mathf.Clamp(linearValue, 0.0001f, 1f);
        float db = Mathf.Log10(clamped) * 20f;
        audioMixer.SetFloat(parameter, db);
    }

    // -------------------------------------------------------
    // SFX & Music Playback
    // -------------------------------------------------------

    public void PlaySFX(string sfxName)
    {
        if (_sfxDictionary != null && _sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            if (sfxSource && clip)
                sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] SFX '{sfxName}' nicht gefunden!");
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