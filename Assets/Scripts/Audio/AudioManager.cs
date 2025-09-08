using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

// 1. Cambiamos "MonoBehaviour" por nuestro molde "SingletonPersistent<AudioManager>"
public class AudioManager : SingletonPersistent<AudioManager>
{
    // 2. Borramos todo el c�digo del Singleton que ten�as en Awake() y la variable "Instance".
    //    �El molde ya se encarga de todo eso!

    [SerializeField] public AudioMixer myMixer;
    [SerializeField] private Slider sliderMaster;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSFX;
    [SerializeField] private AudioSettings audioSettings;

    // Dejamos el resto de tu l�gica intacta
    private void Start()
    {
        // Es buena pr�ctica comprobar si los sliders est�n asignados
        // para evitar errores si se usa en una escena sin el men� de opciones.
        if (sliderMaster != null && sliderMusic != null && sliderSFX != null)
        {
            LoadVolumeSettings();
        }
    }

    private void LoadVolumeSettings()
    {
        sliderMaster.value = audioSettings.masterVolume;
        sliderMusic.value = audioSettings.musicVolume;
        sliderSFX.value = audioSettings.sfxVolume;
        SetVolumeMaster();
        SetVolumeMusic();
        SetVolumeSFX();
    }

    public void SetVolumeMaster()
    {
        float volume = sliderMaster.value;
        audioSettings.masterVolume = volume;
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeMusic()
    {
        float volume = sliderMusic.value;
        audioSettings.musicVolume = volume;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    public void SetVolumeSFX()
    {
        float volume = sliderSFX.value;
        audioSettings.sfxVolume = volume;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }
}