using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip coinPickup;
    [SerializeField] private AudioClip heartContainerPickup;
    [SerializeField] private AudioClip playerDeath;
    [SerializeField] private AudioClip jump;

    private Dictionary<string, AudioClip> _sfxDictionary;

    public void Initialize()
    {
        _sfxDictionary = new Dictionary<string, AudioClip>
        {
            { "CoinPickup", coinPickup },
            { "LifePickup", heartContainerPickup },
            { "PlayerDeath", playerDeath },
            { "Jump", jump }
        };
    }

    public void PlaySFX(string sfxName)
    {
        if (_sfxDictionary != null && _sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            if (SFXSource && clip)
                SFXSource.PlayOneShot(clip);
        }
    }
}