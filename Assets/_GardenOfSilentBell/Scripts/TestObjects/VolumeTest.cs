using UnityEngine;
using UnityEngine.Audio;

public class VolumeTest : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetMusicVolume(float volume)
    {
        mixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }
}