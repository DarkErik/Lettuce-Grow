using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerpotBaseLogic : MonoBehaviour {


    [SerializeField]
    private Collider2D interactionHitbox;

    [SerializeField]
    private GameObject plant;
    private PlantLogic plantLogic;

    private bool isPlanted = false;

    
    /// <summary>
    /// Returns true when the given object is a plant and this pot is empty
    /// </summary>
    /// <param name="plant"></param>
    /// <returns></returns>
    public bool CanPlant(GameObject plant) {
        return !isPlanted && plant.GetComponent<PlantLogic>() != null;
    }


    public void Plant(GameObject plant) {

        PlantLogic pl = plant.GetComponent<PlantLogic>();
        if (pl == null) { throw new System.Exception("Oject " + plant + " is not plantable"); }

        this.plant = plant;
        plantLogic = pl;
        isPlanted = true;

        Vector2 relativePosition = plantLogic.GetRelativePositionForFlowerpot();

        plant.transform.parent = this.transform;
        plant.transform.position = new Vector3(this.transform.position.x + relativePosition.x, this.transform.position.y + relativePosition.y, plant.transform.position.z);

    }

}
