using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : SingletonPersistent<AudioManager>
{
    [Header("Componentes Principales")]
    [SerializeField] public AudioMixer myMixer;
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private AudioSource musicSource; 
    [SerializeField] private AudioSource sfxSource;  
    [SerializeField] private AudioSource voiceSource; 

    [Header("--- LIBRERÍA DE AUDIO CLIPS ---")]

    [Header("Música")]
    public AudioClip backgroundMusic;

    [Header("Efectos de Sonido (SFX)")]
    public AudioClip buttonClickSound;
    public AudioClip radioSquelchSound;

    [Header("Voces del Jugador")]
    public AudioClip playerCheckingConsciousness; 
    public AudioClip playerReportVoice;          

    [Header("Voces del Instructor")]
    public AudioClip instructor_BriefingInicial;
    public AudioClip instructor_AlertaEmergencia;
    public AudioClip instructor_ConfirmacionRescate;
    public AudioClip instructor_ZonaSegura;
    public AudioClip instructor_ComprobarConciencia;
    public AudioClip instructor_FeedbackIncorrectoConciencia;
    public AudioClip instructor_LlamarRadio;
    public AudioClip instructor_AbrirViasAereas;
    public AudioClip instructor_FeedbackIncorrectoViasAereas;

    [Header("Voz de la Central")]
    public AudioClip dispatcherResponseVoice;

    private void Start()
    {
        SetVolumeMaster(audioSettings.masterVolume);
        SetVolumeMusic(audioSettings.musicVolume);
        SetVolumeSFX(audioSettings.sfxVolume);

        PlayBackgroundMusic();
    }
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayVoice(AudioClip clip)
    {
        if (voiceSource != null && clip != null)
        {
            voiceSource.Stop();
            voiceSource.PlayOneShot(clip);
        }
    }
    public void PlayRadioCallSequence()
    {
        StartCoroutine(RadioCallCoroutine());
    }
    private IEnumerator RadioCallCoroutine()
    {
        PlaySFX(radioSquelchSound);
        yield return new WaitForSeconds(radioSquelchSound != null ? radioSquelchSound.length : 0.2f);

        PlaySFX(playerReportVoice);
        yield return new WaitForSeconds(playerReportVoice != null ? playerReportVoice.length : 1f);

        PlaySFX(dispatcherResponseVoice);
    }
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

    public void SetVolumeMaster(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        myMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        audioSettings.masterVolume = volume; 
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