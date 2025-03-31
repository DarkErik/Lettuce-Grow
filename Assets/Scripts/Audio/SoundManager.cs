using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private const string homeBrewPlayerPrefsKey = "isHomeBrewActive";
    private bool isHomeBrewActive;

    private const string sfxMasterVolumePlayerPrefsKey = "sfxMasterVolume";
    private float masterVolume = 1f;

    private void Awake()
    {
        Instance = this;
        isHomeBrewActive = (PlayerPrefs.GetInt(homeBrewPlayerPrefsKey) != 0);
        masterVolume = PlayerPrefs.GetFloat(sfxMasterVolumePlayerPrefsKey, masterVolume);
    }

    public void ChangeHomeBrewMode(bool isHomeBrewActive) {
        this.isHomeBrewActive = isHomeBrewActive;
        PlayerPrefs.SetInt(homeBrewPlayerPrefsKey, (this.isHomeBrewActive ? 1 : 0));
    }

    public void ChangeMasterVolume(float newVolume) { 
        masterVolume = newVolume;
        PlayerPrefs.SetFloat(sfxMasterVolumePlayerPrefsKey, newVolume);
        Debug.Log($"Set volume to {newVolume}");
    }

    public float GetMasterVolume() { 
        return masterVolume;
    }

    public bool GetIsHomeBrewActive() {
        return isHomeBrewActive;
    }

    public void PlayFliegenklatscheSound(Vector3 position, float volume = 1f) {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.klatschHomeBrew : audioClipRefsSO.klatsch;
        PlaySound(audioClips, position, volume);
    }

    public void PlayPlantNeedArisesSound(Vector3 position, float volume = 1f) {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.plantNeedArisesHomeBrew : audioClipRefsSO.plantNeedArises;
        PlaySound(audioClips, position, volume);
    }

    public void PlayPlantSellSound(Vector3 position, float volume = 1f) {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.plantSoldHomeBrew : audioClipRefsSO.plantSold;
        PlaySound(audioClips, position, volume);
    }

    public void PlayWaterHitPlantSound(Vector3 position, float volume = 1f)
    {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.platschHomeBrew : audioClipRefsSO.platsch;
        PlaySound(audioClips, position, volume);
    }

    public void PlayBanjoHitSound(Vector3 position, float volume = 1f, float pitch = 1f)
    {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.banjoHitHomeBrew : audioClipRefsSO.banjoHit;
        PlaySoundEx(audioClips[Random.Range(0, audioClips.Length)], position, volume, 1f, pitch);
    }

    public void PlayBanjoMissedSound(Vector3 position, float volume = 1f)
    {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.banjoMissedHomeBrew : audioClipRefsSO.banjoMissed;
        PlaySound(audioClips, position, volume);
    }

    public void PlayPlantPlantedSound(Vector3 position, float volume = 1f) {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.plantPlantedHomeBrew : audioClipRefsSO.plantPlanted;
        PlaySound(audioClips, position, volume);
    }
    public void PlayHarvestSound(Vector3 position, float volume = 1f)
    {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.harvestHomeBrew : audioClipRefsSO.harvest;
        PlaySound(audioClips, position, volume);
    }
    
    public void PlayPlantDeathSound(Vector3 position, float volume = 1f)
    {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.plantDeathHomeBrew : audioClipRefsSO.plantDeath;
        PlaySound(audioClips, position, volume);
    }
    

    public void PlayPlantFinishedGrowingSound(Vector3 position, float volume = 1f)
    {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.plantFinishedGrowingHomeBrew : audioClipRefsSO.plantFinishedGrowing;
        PlaySound(audioClips, position, volume);
    }

    public void PlayItemGrabbedSound(Vector3 position, float volume = 1f)
    {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.itemGrabbedHomeBrew : audioClipRefsSO.itemGrabbed;
        PlaySound(audioClips, position, volume);
    }
    public void PlayItemPlacedSound(Vector3 position, float volume = 1f)
    {
        AudioClip[] audioClips = isHomeBrewActive ? audioClipRefsSO.itemPlacedHomeBrew : audioClipRefsSO.itemPlaced;
        PlaySound(audioClips, position, volume);
    }
    public void PlayFootstepSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.footstep, position, volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * masterVolume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }


    /// <summary>
    /// Does pretty much the same as AudioSource.PlayClipAtPoint, but with more parameters exposed
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="position"></param>
    /// <param name="volume"></param>
    /// <param name="spatialBlend"></param>
    /// <param name="pitch"></param>
    private void PlaySoundEx(AudioClip audioClip, Vector3 position, float volume = 1f, float spatialBlend = 1f, float pitch = 1f) {
        
        GameObject gameObject = new GameObject("One shot audio Ex");
        gameObject.transform.position = position;
        
        AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        audioSource.clip = audioClip;
        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();
        
        Object.Destroy(gameObject, audioClip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
    }

}
