using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrieable : MonoBehaviour {


    /// <summary>
    /// The distance relative to the player at which this object should be positioned
    /// </summary>
    [SerializeField]
    private float carryDistance = 1.0f;


    private PlayerController parent = null;


    public void PickUp(PlayerController parent) {
        this.parent = parent;
        this.transform.parent = parent.transform;

        Debug.Log("Picked up " + this.gameObject.name);
    }

    public void Drop() {
        this.transform.parent = null;
        this.parent = null;

        Debug.Log("Dropped " + this.gameObject.name);
    }



    public void FixedUpdate() {
        
        if (parent != null) {
            Vector2 direction = parent.GetMovementDirection();
            if (direction == Vector2.zero) { direction = parent.GetLastMovementDirection(); }

            this.transform.localPosition = new Vector3(direction.x * carryDistance, direction.y * carryDistance);
        }
    }



}
