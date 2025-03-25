using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDepot : MonoBehaviour {

    [SerializeField]
    private GameObject childObject;

    private Carrieable childCarriableComponent;

    [SerializeField]
    private Collider2D interactionHitbox;

    [SerializeField]
    private Vector2 childObjectPosition;

    // True if the item is currently inside the depot
    private bool isInPossession = true;


    public void Start() {
        childCarriableComponent = childObject.GetComponent<Carrieable>();

        childObject.transform.parent = this.transform;
        childObject.transform.position = new Vector3(this.transform.position.x + childObjectPosition.x, this.transform.position.y + childObjectPosition.y, childObject.transform.position.z);
    }


    public bool HasPossessionOfChildObject() { return isInPossession; }

    /// <summary>
    /// Checks whether the given GameObject is assigned to this depot
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool AcceptObject(GameObject obj) { return obj == childObject; }


    public Carrieable PickUp(PlayerController parent) {
        
        if (!isInPossession) { throw new System.Exception("Item depot does not currently possess any items"); }

        isInPossession = false;
        childCarriableComponent.PickUp(parent);

        return childCarriableComponent;
    }

    public void Drop() {
        isInPossession = true;
        childCarriableComponent.Drop();

        childObject.transform.parent = this.transform;
        childObject.transform.position = new Vector3(this.transform.position.x + childObjectPosition.x, this.transform.position.y + childObjectPosition.y, childObject.transform.position.z);
    }

}
