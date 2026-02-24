using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Wird direkt auf das Settings-Panel GameObject gelegt.
/// Funktioniert sowohl im MainMenu als auch im PauseMenu.
/// </summary>
public class SettingsMenuController : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Dieses Panel öffnet sich "über" dem aufrufenden Menü.
    // Der Caller (z.B. PauseMenuController) übergibt sich selbst,
    // damit wir ihn beim Schließen wieder einblenden können.
    private GameObject _callerPanel;

    // -------------------------------------------------------
    // Öffnen / Schließen
    // -------------------------------------------------------

    /// <summary>
    /// Öffnet das Settings-Panel und merkt sich welches Menü zuvor aktiv war.
    /// </summary>
    public void Open(GameObject callerPanel = null)
    {
        _callerPanel = callerPanel;

        // Slider auf gespeicherte Werte setzen (ohne Events auszulösen)
        if (AudioManager.Instance != null)
        {
            SetSliderSilently(masterSlider, AudioManager.Instance.GetMasterVolume());
            SetSliderSilently(musicSlider, AudioManager.Instance.GetMusicVolume());
            SetSliderSilently(sfxSlider, AudioManager.Instance.GetSFXVolume());
        }

        gameObject.SetActive(true);
    }

    /// <summary>
    /// Schließt das Settings-Panel und kehrt zum aufrufenden Menü zurück.
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);

        if (_callerPanel != null)
            _callerPanel.SetActive(true);

        _callerPanel = null;
    }

    // -------------------------------------------------------
    // Slider Events (werden im Inspector mit OnValueChanged verknüpft)
    // -------------------------------------------------------

    public void OnMasterVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMasterVolume(value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        AudioManager.Instance?.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        AudioManager.Instance?.SetSFXVolume(value);
    }

    // -------------------------------------------------------
    // Hilfsmethoden
    // -------------------------------------------------------

    /// <summary>Setzt den Slider-Wert ohne das OnValueChanged-Event auszulösen.</summary>
    private void SetSliderSilently(Slider slider, float value)
    {
        if (slider == null) return;

        slider.onValueChanged.RemoveAllListeners();
        slider.value = value;

        // Listener wieder hinzufügen – passend zum jeweiligen Slider
        if (slider == masterSlider) slider.onValueChanged.AddListener(OnMasterVolumeChanged);
        else if (slider == musicSlider) slider.onValueChanged.AddListener(OnMusicVolumeChanged);
        else if (slider == sfxSlider) slider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
}