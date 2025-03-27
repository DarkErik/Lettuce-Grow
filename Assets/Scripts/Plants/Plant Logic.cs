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

    public Vector2 GetRelativePositionForFlowerpot() {
        return relativePositionInFlowerpot;
    }

    public PlantType GetPlantType() {
        return type;
    }

}
