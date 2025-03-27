using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrieable : MonoBehaviour {


    /// <summary>
    /// The distance relative to the player at which this object should be positioned
    /// </summary>
    [SerializeField]
    private float carryDistance = 1.0f;


    [SerializeField]
    private Vector2 carryOffset;

    [SerializeField]
    private bool allowMirror = true;


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

            this.transform.localPosition = new Vector3(direction.x * carryDistance + carryOffset.x, direction.y * carryDistance + carryOffset.y);
            
            if (allowMirror) {

                float adjustedAngle = Mathf.Atan2(direction.y, direction.x);
                Debug.Log(adjustedAngle + " " + (Mathf.Rad2Deg * adjustedAngle));

                if (adjustedAngle < -Mathf.PI / 2 - 0.1 || adjustedAngle > Mathf.PI / 2 + 0.1) {
                    this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
                } else if (adjustedAngle > -Mathf.PI / 2 + 0.1 && adjustedAngle < Mathf.PI / 2 - 0.1) {
                    this.transform.localScale = new Vector3(-Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
                }

            }
        }
    }



}
