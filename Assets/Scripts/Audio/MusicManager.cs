using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    private const string musicMasterVolumePlayerPrefsKey = "musicMasterVolume";
    private float relativeMasterVolume = 1f;
    private float realMasterVolume = 1f;
    private float maxVolume;

    private AudioSource musicSource;

    private void Awake()
    {
        Instance = this;

        musicSource = GetComponent<AudioSource>();
        maxVolume = musicSource.volume;

        relativeMasterVolume = PlayerPrefs.GetFloat(musicMasterVolumePlayerPrefsKey, relativeMasterVolume);
        realMasterVolume = maxVolume * relativeMasterVolume;
        musicSource.volume = realMasterVolume;
    }

    public float GetRelativeMasterVolume() {
        return relativeMasterVolume;
    }

    public void ChangeRelativeMasterVolume(float newRelativeMasterVolume) {
        relativeMasterVolume = newRelativeMasterVolume;
        PlayerPrefs.SetFloat(musicMasterVolumePlayerPrefsKey, newRelativeMasterVolume);
        realMasterVolume = maxVolume * relativeMasterVolume;
        musicSource.volume = realMasterVolume;
    }
}
