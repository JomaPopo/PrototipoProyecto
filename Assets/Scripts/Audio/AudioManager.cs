using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SingletonPersistent<AudioManager>
{
    [Header("Audio Mixer y Settings")]
    [SerializeField] public AudioMixer myMixer;
    [SerializeField] private AudioSettings audioSettings;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips (Librer�a)")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSound;

    // El Start ahora es perfecto para cargar y aplicar los vol�menes guardados.
    private void Start()
    {
        // Cargamos los valores guardados en el ScriptableObject y los aplicamos al Mixer.
        SetVolumeMaster(audioSettings.masterVolume);
        SetVolumeMusic(audioSettings.musicVolume);
        SetVolumeSFX(audioSettings.sfxVolume);

        PlayBackgroundMusic();
    }

    // --- M�todos de Reproducci�n (sin cambios) ---
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickSound != null)
        {
            sfxSource.PlayOneShot(buttonClickSound);
        }
    }

    // --- M�todos de Volumen (�MEJORADOS!) ---
    // Ahora aceptan un valor 'volume' como par�metro.
    public void SetVolumeMaster(float volume)
    {
        // Aseguramos que el volumen est� en un rango seguro para el Log10
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        audioSettings.masterVolume = volume; // Guardamos el valor
    }

    public void SetVolumeMusic(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        audioSettings.musicVolume = volume;
    }

    public void SetVolumeSFX(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        myMixer.SetFloat("Sfx", Mathf.Log10(volume) * 20);
        audioSettings.sfxVolume = volume;
    }
}