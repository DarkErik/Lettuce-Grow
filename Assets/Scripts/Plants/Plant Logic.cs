using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLogic : MonoBehaviour {

    public enum PlantType {
        invalid = -1,
        carrot = 0,
        pumpkin,
        salad,
    }

    [SerializeField]
    private PlantType type = PlantType.invalid;

    [SerializeField]
    private Vector2 relativePositionInFlowerpot;

    [SerializeField]
    private ParticleSystem highlight;

    public Vector2 GetRelativePositionForFlowerpot() {
        return relativePositionInFlowerpot;
    }

    public PlantType GetPlantType() {
        return type;
    }


    public void SetHiglight(bool enabled) {
        if (enabled) { 
            highlight.Play(); 
        } else { 
            highlight.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); 
        }
    }

}
