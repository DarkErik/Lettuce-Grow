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
    public AudioClip[] itemPlaced;

    [Header("Plant Sounds")]
    public AudioClip[] plantPlanted;
    public AudioClip[] harvest;
    public AudioClip[] plantFinishedGrowing;
    public AudioClip[] plantDeath;
    public AudioClip[] plantSold;
    public AudioClip[] plantNeedArises;


    [Header("Minigame Sounds")]
    public AudioClip[] platsch; //water game
    public AudioClip[] klatsch; //fly game
    public AudioClip[] banjoHit;
    public AudioClip[] banjoMissed;
}
