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

    [SerializeField] private AudioClip ambienceMusic;
    [SerializeField] private AudioClip intenseMusic;
    [SerializeField] private float fadeTime;

    [SerializeField] private bool isNotInLevel;

    private void Awake()
    {
        if (Instance != null) {
            if (Instance.musicSource.clip == intenseMusic) {
                Instance.FadeToAmbient();
            }


            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);


        musicSource = GetComponent<AudioSource>();
        maxVolume = musicSource.volume;

        relativeMasterVolume = PlayerPrefs.GetFloat(musicMasterVolumePlayerPrefsKey, relativeMasterVolume);
        realMasterVolume = maxVolume * relativeMasterVolume;
        musicSource.volume = realMasterVolume;

        musicSource.clip = ambienceMusic;
        musicSource.Play();
    }


    public void FadeToAmbient() {
        StartCoroutine(Fade(ambienceMusic));
    }
    public void GameManager_OnStressPhaseEntered()
    {
        StartCoroutine(Fade(intenseMusic));
    }

    private IEnumerator Fade(AudioClip fade2) {
        float timer = 0f;
        while (timer < fadeTime) {           
            float fadeProgress = timer / fadeTime;
            realMasterVolume = maxVolume * relativeMasterVolume * (1-fadeProgress);
            musicSource.volume = realMasterVolume;
            yield return null;

            timer += Time.deltaTime;
        }

        musicSource.clip = fade2;
        musicSource.Play();
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
