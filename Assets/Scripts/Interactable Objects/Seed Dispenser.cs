using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedDispenser : MonoBehaviour {

    [SerializeField]
    private GameObject seedPrefab;


    [SerializeField]
    private Color seedColor;


    [SerializeField]
    private GameObject plantPrefab;

    // Create a new seed when one is picked up
    private GameObject dispensedSeed;
    private Carrieable dispensedSeedCarriableComponent;

    [SerializeField]
    private Collider2D interactionHitbox;

    [SerializeField]
    private uint count;


    [SerializeField, Tooltip("The gameobject containing the seed sprite - Used to display an empty/filled box")]
    private GameObject contentVisual;




    public bool HasSeeds() { return count > 0; }

    /// <summary>
    /// Checks whether the given GameObject is assigned to this depot
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool AcceptObject(GameObject obj) { return obj == dispensedSeed; }

    public GameObject GetPlantPrefab() { return plantPrefab; }


    public Carrieable PickUp(PlayerController parent) {

        if (!HasSeeds()) { throw new System.Exception("Seed dispenser is empty"); }


        dispensedSeed = GameObject.Instantiate(seedPrefab);
        dispensedSeed.transform.position = new Vector3(100, 100, 0);        // Spawn out of bounds so it isn't visible until the first fixed update frame
        dispensedSeed.GetComponent<SpriteRenderer>().color = seedColor;
        dispensedSeedCarriableComponent = dispensedSeed.GetComponent<Carrieable>();

        dispensedSeedCarriableComponent.PickUp(parent);

        count--;
        if (count == 0) { contentVisual.SetActive(false); }

        return dispensedSeedCarriableComponent;
    }


    public void Drop() {
        count++;
        dispensedSeedCarriableComponent.Drop();

        GameObject.Destroy(dispensedSeed);
        dispensedSeed = null;
        dispensedSeedCarriableComponent = null;


        if (count > 0 && !contentVisual.activeInHierarchy) { contentVisual.SetActive(true); }
    }


}
