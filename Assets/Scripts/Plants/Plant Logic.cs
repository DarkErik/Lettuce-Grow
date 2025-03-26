using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantLogic : MonoBehaviour
{

    [SerializeField]
    private Vector2 relativePositionInFlowerpot;

    public Vector2 GetRelativePositionForFlowerpot() {
        return relativePositionInFlowerpot;
    }

}
