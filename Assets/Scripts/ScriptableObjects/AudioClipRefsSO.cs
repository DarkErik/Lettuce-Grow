using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
    [Header("Player Sounds")]
    public AudioClip[] footstep;

    [Header("Item Sounds")]
    public AudioClip[] itemGrabbed;
    public AudioClip[] itemGrabbedHomeBrew;
    public AudioClip[] itemPlaced;
    public AudioClip[] itemPlacedHomeBrew;

    [Header("Plant Sounds")]
    public AudioClip[] plantPlanted;
    public AudioClip[] plantPlantedHomeBrew;
    public AudioClip[] harvest;
    public AudioClip[] harvestHomeBrew;
    public AudioClip[] plantFinishedGrowing;
    public AudioClip[] plantFinishedGrowingHomeBrew;
    public AudioClip[] plantDeath;
    public AudioClip[] plantDeathHomeBrew;
    public AudioClip[] plantSold;
    public AudioClip[] plantSoldHomeBrew;
    public AudioClip[] plantNeedArises;
    public AudioClip[] plantNeedArisesHomeBrew;


    [Header("Minigame Sounds")]
    public AudioClip[] platsch; //water game
    public AudioClip[] platschHomeBrew; //water game
    public AudioClip[] klatsch; //fly game
    public AudioClip[] klatschHomeBrew; //fly game
    public AudioClip[] banjoHit;
    public AudioClip[] banjoHitHomeBrew;
    public AudioClip[] banjoMissed;
    public AudioClip[] banjoMissedHomeBrew;
}
