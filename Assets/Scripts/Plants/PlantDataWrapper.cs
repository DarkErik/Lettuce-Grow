using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantDataWrapper : MonoBehaviour
{
    [SerializeField] private PlantSO plantData;
    public PlantSO PlantData { get { return plantData; } private set { plantData = value; } }
}
