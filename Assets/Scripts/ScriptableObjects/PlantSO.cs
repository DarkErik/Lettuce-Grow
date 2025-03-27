using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PlantSO : ScriptableObject
{
    public enum PlantType { PUMPKIN, CARROT, SALAT };

    public PlantType plantType;


    [Header("Progression")]
    public float neededGrothTime;
    public float regressionSpeedFactor;   

    [System.Serializable]
    public class NeedWrapper
    {
        public float pickChance;
        public Player.PlayerController.CarriableItemTypes neededCarriableItem;
    }

    [Header("Needs")]
    public float initalNeedDelayAfterPlanting; 
    public float minNeedTime;
    public float maxNeedTime;
    public EnumDataMapping<NeedWrapper, PlantNeed> plantNeedMapping;
}
