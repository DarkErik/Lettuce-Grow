using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowingPlant : MonoBehaviour
{


    [SerializeField, Tooltip("The min/max amount of water droplets needed for this plant to grow a stage")]
    private Vector2Int requiredWater;

    [SerializeField]
    private Animator animator;

    private int waterNeeded;
    private uint stage = 0;

    public void Start() {

        waterNeeded = Random.Range(requiredWater.x, requiredWater.y);
    }



    public void AddWater() {
        waterNeeded--;

        if (waterNeeded == 0) {
            stage++;
            animator.SetInteger("growthStage", (int)stage);

            if (stage < 2) { waterNeeded = Random.Range(requiredWater.x, requiredWater.y); }
            else { transform.parent.GetComponent<WateringMinigame>().PlantFullyGrownCallback(); }
        }
    }
    

}
