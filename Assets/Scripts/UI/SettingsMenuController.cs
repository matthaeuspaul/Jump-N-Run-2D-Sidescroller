using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private GameObject _callerPanel;

    public void Open(GameObject callerPanel = null)
    {
        _callerPanel = callerPanel;

        if (AudioManager.Instance != null)
        {
            SetSliderSilently(masterSlider, AudioManager.Instance.GetMasterVolume());
            SetSliderSilently(musicSlider, AudioManager.Instance.GetMusicVolume());
            SetSliderSilently(sfxSlider, AudioManager.Instance.GetSFXVolume());
        }

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);

        if (_callerPanel != null)
            _callerPanel.SetActive(true);

        _callerPanel = null;
    }

    public void OnMasterVolumeChanged(float value) => AudioManager.Instance?.SetMasterVolume(value);
    public void OnMusicVolumeChanged(float value) => AudioManager.Instance?.SetMusicVolume(value);
    public void OnSFXVolumeChanged(float value) => AudioManager.Instance?.SetSFXVolume(value);

    private void SetSliderSilently(Slider slider, float value)
    {
        if (slider == null) return;

        slider.onValueChanged.RemoveAllListeners();
        slider.value = value;

        if (slider == masterSlider) slider.onValueChanged.AddListener(OnMasterVolumeChanged);
        else if (slider == musicSlider) slider.onValueChanged.AddListener(OnMusicVolumeChanged);
        else if (slider == sfxSlider) slider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
}