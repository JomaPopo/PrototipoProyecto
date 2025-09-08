using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Sliders de Volumen")]
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;

    [Header("Configuración de Botones +/-")]
    [SerializeField] private float volumeStep = 0.1f;

    [Header("Datos Guardados")]
    [SerializeField] private AudioSettings audioSettings;

    // Start se usa para inicializar la UI con los valores guardados
    void Start()
    {
        if (audioSettings != null)
        {
            sliderMaster.value = audioSettings.masterVolume;
            sliderMusic.value = audioSettings.musicVolume;
            sliderSFX.value = audioSettings.sfxVolume;
        }

        // Conectamos los sliders para que llamen a las funciones cuando cambien de valor
        sliderMaster.onValueChanged.AddListener(OnMasterSliderChanged);
        sliderMusic.onValueChanged.AddListener(OnMusicSliderChanged);
        sliderSFX.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    // --- Funciones que se llaman desde los Sliders (¡esto responde a tu pregunta!) ---
    private void OnMasterSliderChanged(float value)
    {
        AudioManager.Instance.SetVolumeMaster(value);
    }
    private void OnMusicSliderChanged(float value)
    {
        AudioManager.Instance.SetVolumeMusic(value);
    }
    private void OnSFXSliderChanged(float value)
    {
        AudioManager.Instance.SetVolumeSFX(value);
    }

    // --- Funciones para los BOTONES (+/-) ---
    // Fíjate que los botones solo cambian el valor del slider.
    // Al cambiar el valor del slider, la función de arriba se dispara automáticamente.
    public void IncreaseMasterVolume() { sliderMaster.value += volumeStep; }
    public void DecreaseMasterVolume() { sliderMaster.value -= volumeStep; }
    public void IncreaseMusicVolume() { sliderMusic.value += volumeStep; }
    public void DecreaseMusicVolume() { sliderMusic.value -= volumeStep; }
    public void IncreaseSFXVolume() { sliderSFX.value += volumeStep; }
    public void DecreaseSFXVolume() { sliderSFX.value -= volumeStep; }
}