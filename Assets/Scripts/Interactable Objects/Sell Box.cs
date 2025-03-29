using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellBox : MonoBehaviour {


    private Dictionary<PlantLogic.PlantType, System.Action> plantLookupTable;


    [SerializeField]
    private Collider2D interactionHitbox;



    private void Start() {
        
        // Has to be assigned at runtime - GameManager.Instance is null at compile time
        plantLookupTable = new Dictionary<PlantLogic.PlantType, System.Action>() {
            { PlantLogic.PlantType.carrot, (System.Action)GameManager.Instance.AddCarrot },
            { PlantLogic.PlantType.pumpkin, (System.Action)GameManager.Instance.AddPumpkin },
            { PlantLogic.PlantType.salad, (System.Action)GameManager.Instance.AddSalad },
        };

    }

    public bool CanBeSold(GameObject gameObject) { return gameObject.GetComponent<PlantLogic>() != null; }


    public void Sell(GameObject gameObject) {

        PlantLogic pl = gameObject.GetComponent<PlantLogic>();
        if (pl == null) { throw new System.Exception("Object " + gameObject + " can not be sold"); }

        SoundManager.Instance.PlayPlantSellSound(this.transform.position);

        plantLookupTable[pl.GetPlantType()]();
        GameObject.Destroy(gameObject);
    }

    
}
