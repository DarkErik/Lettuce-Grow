using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
    public AudioClip[] footstep;

    public AudioClip[] itemGrabbed;
    public AudioClip[] itemPlaced;
    public AudioClip[] plantPlanted;
    public AudioClip[] plantFinishedGrowing;
    public AudioClip[] plantSold;

    public AudioClip[] platsch;
}
