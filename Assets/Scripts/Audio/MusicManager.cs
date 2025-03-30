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

    private void Awake()
    {
        Instance = this;

        musicSource = GetComponent<AudioSource>();
        maxVolume = musicSource.volume;

        relativeMasterVolume = PlayerPrefs.GetFloat(musicMasterVolumePlayerPrefsKey, relativeMasterVolume);
        realMasterVolume = maxVolume * relativeMasterVolume;
        musicSource.volume = realMasterVolume;

        musicSource.clip = ambienceMusic;
        musicSource.Play();
    }

    private void OnEnable()
    {
        GameManager.OnStressPhaseEntered += GameManager_OnStressPhaseEntered;
    }

    private void OnDisable()
    {
        GameManager.OnStressPhaseEntered -= GameManager_OnStressPhaseEntered;
    }

    private void GameManager_OnStressPhaseEntered(object sender, System.EventArgs e)
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade() {
        float timer = 0f;
        while (timer < fadeTime) {           
            float fadeProgress = timer / fadeTime;
            realMasterVolume = maxVolume * relativeMasterVolume * (1-fadeProgress);
            musicSource.volume = realMasterVolume;
            yield return null;

            timer += Time.deltaTime;
        }

        musicSource.clip = intenseMusic;
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
