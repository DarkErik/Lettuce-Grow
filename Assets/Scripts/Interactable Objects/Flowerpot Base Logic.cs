using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlowerpotBaseLogic : MonoBehaviour {

    [SerializeField]
    private PlantProgressionManager progressionManager;

    [SerializeField]
    private Collider2D interactionHitbox;

    [SerializeField]
    private GameObject plant;
    private PlantLogic plantLogic;

    private bool isPlanted = false;
    private bool isDeposited = false;

    public class OnPlantedEventArgs : EventArgs {
        public PlantSO plantData;
    }
    public event EventHandler<OnPlantedEventArgs> OnPlanted;

    
    /// <summary>
    /// Returns true when the given object is a plant and this pot is empty
    /// </summary>
    /// <param name="plant"></param>
    /// <returns></returns>
    public bool CanPlant(GameObject plant) {
        return !isPlanted && !isDeposited && plant.GetComponent<PlantLogic>() != null;
    }


    public void Plant(GameObject plant) {

        AddPlantToPot(plant);
        SoundManager.Instance.PlayPlantPlantedSound(this.transform.position);
        isPlanted = true;

        OnPlanted?.Invoke(this, new OnPlantedEventArgs { plantData = plant.GetComponent<PlantDataWrapper>().PlantData});

    }


    public bool CanBeHarvested() {
        return isPlanted && progressionManager.IsPlantReadyToHarvest;
    }


    public GameObject Harvest() {
        if (!CanBeHarvested()) { throw new Exception("Plant " + plant + " can not be harvested"); }
        progressionManager.Harvest();
        SoundManager.Instance.PlayHarvestSound(this.transform.position);
        isPlanted = false;

        return plant;
    }



    #region Putting down/picking up/swapping already grown plants

    /// <summary>
    /// Checks whether the flower pot is currently empty
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty() { return !isPlanted && !isDeposited; }

    /// <summary>
    /// Checks whether there is a plant currently deposited in this flowerpot
    /// </summary>
    /// <returns></returns>
    public bool IsDeposited() {  return isDeposited; }

    /// <summary>
    /// Returns true if the given object and the plant inside the flower pot can be swapped
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool CanBeSwapped(GameObject other) { return (isDeposited || CanBeHarvested()) && other.GetComponents<PlantLogic>() != null; }


    /// <summary>
    /// Deposits a fully grown plant back into the pot
    /// </summary>
    /// <param name="plant"></param>
    public void Deposit(GameObject plant) {

        AddPlantToPot(plant);
        isDeposited = true;
    }


    /// <summary>
    /// Picks up a fully grown plant from the pot
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public GameObject PickUp() {

        if (!isDeposited) { throw new Exception("There is no plant to pick up"); }
        isDeposited = false;

        return plant;
    }


    /// <summary>
    /// Swaps the currently held plant with the one in the pot
    /// </summary>
    /// <param name="plant"></param>
    /// <returns></returns>
    public GameObject Swap(GameObject plant) {

        GameObject old = CanBeHarvested() ? Harvest() : PickUp();
        Deposit(plant);

        return old;
    }

    #endregion


    /// <summary>
    /// Helper funtion to automatically set up references and position the plant
    /// </summary>
    /// <param name="plant"></param>
    /// <exception cref="System.Exception"></exception>
    private void AddPlantToPot(GameObject plant) {
        PlantLogic pl = plant.GetComponent<PlantLogic>();
        if (pl == null) { throw new System.Exception("Oject " + plant + " can not be deposited here"); }

        this.plant = plant;
        plantLogic = pl;

        Vector2 relativePosition = plantLogic.GetRelativePositionForFlowerpot();
        plant.GetComponent<Carrieable>().Drop();

        plant.transform.parent = this.transform;
        plant.transform.position = new Vector3(this.transform.position.x + relativePosition.x, this.transform.position.y + relativePosition.y, plant.transform.position.z);
    }




    public void DestroyPlant() {
        Destroy(this.plant);

        plant = null;
        plantLogic = null;
        isPlanted = false;
    }

    /// <summary>
    /// Changes the Mode of the Animator of the plant
    /// </summary>
    public void ChangePannicMode() {
        PlantAnimation plantAnimation = plant.GetComponent<PlantAnimation>();
        plantAnimation.ChangePanic();
    }

}
