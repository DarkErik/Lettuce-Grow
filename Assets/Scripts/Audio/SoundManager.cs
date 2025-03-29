using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private float masterVolume = 1f;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayFliegenklatscheSound(Vector3 position, float volume = 1f) {
        PlaySound(audioClipRefsSO.klatsch, position, volume);
    }

    public void PlayPlantNeedArisesSound(Vector3 position, float volume = 1f) {
        PlaySound(audioClipRefsSO.plantNeedArises, position, volume);
    }

    public void PlayPlantSellSound(Vector3 position, float volume = 1f) {
        PlaySound(audioClipRefsSO.plantSold, position, volume);
    }

    public void PlayWaterHitPlantSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.platsch, position, volume);
    }

    public void PlayBanjoHitSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.banjoHit, position, volume);
    }

    public void PlayBanjoMissedSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.banjoMissed, position, volume);
    }

    public void PlayPlantPlantedSound(Vector3 position, float volume = 1f) {
        PlaySound(audioClipRefsSO.plantPlanted, position, volume);
    }
    public void PlayHarvestSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.harvest, position, volume);
    }
    
    public void PlayPlantDeathSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.plantDeath, position, volume);
    }
    

    public void PlayPlantFinishedGrowingSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.plantFinishedGrowing, position, volume);
    }

    public void PlayItemGrabbedSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.itemGrabbed, position, volume);
    }
    public void PlayItemPlacedSound(Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.itemPlaced, position, volume);
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
}
