using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource;
    public AudioSource menuMusicSource;
    public AudioSource sfxSource;
    public AudioSource menuSfxSource;
    public AudioSource environmentSfxSource;
    public AudioSource characterSfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlayMenuMusic(AudioClip clip)
    {
        menuMusicSource.clip = clip;
        menuMusicSource.loop = true;
        menuMusicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMenuSFX(AudioClip clip)
    {
        menuSfxSource.PlayOneShot(clip);
    }

    public void PlayEnvironmentSFX(AudioClip clip)
    {
        environmentSfxSource.PlayOneShot(clip);
    }

    public void PlayCharacterSFX(AudioClip clip)
    {
        characterSfxSource.PlayOneShot(clip);
    }
}