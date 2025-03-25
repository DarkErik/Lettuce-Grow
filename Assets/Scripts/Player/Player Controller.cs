using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {

    public class PlayerController : MonoBehaviour
    {


        private PlayerInputActions inputWrapper;
        private PlayerInputActions.PlayerActions controller;
        private InputButtonWrapper interactButton;

        [SerializeField]
        private new Rigidbody2D rigidbody;

        [SerializeField]
        private float movementSpeed;

        /// <summary>
        /// Vector containing the current direction of movement - normalized
        /// </summary>
        private Vector2 directionVector;


        // Carrying logic
        [SerializeField]
        private Collider2D interactionHitbox;

        [SerializeField]
        private float interactionDistance = 1f;

        /// <summary>
        /// The amount of fixed updates where the interaction trigger stays active after pressing the interaction button
        /// </summary>
        [SerializeField]
        private uint interactionTimeWindow = 1;
        private uint interactionTimer = 0;

        private Carrieable currentlyCarried = null;



        #region Input setup logic
        private void Awake() {
            inputWrapper = new PlayerInputActions();
            controller = inputWrapper.Player;

            //controller.Interact.performed += OnInteractPerformed;
            interactButton = new InputButtonWrapper(controller.Interact);
            interactButton.onButtonDown += OnInteractPerformed;
        }

        private void OnEnable() {
            controller.Enable();
        }

        private void OnDisable() {
            controller.Disable();
        }

        #endregion

        private void OnInteractPerformed(InputAction.CallbackContext ctx) {

            interactionHitbox.enabled = true;
            interactionTimer = interactionTimeWindow;
        }

        private void OnInteractionCancelled(InputAction.CallbackContext ctx) {
            
        }



        private void FixedUpdate() {

            // Movement
            directionVector = controller.Move.ReadValue<Vector2>().normalized;

            rigidbody.velocity = directionVector * new Vector2(movementSpeed, movementSpeed);



            if (interactionTimer > 0) {
                interactionTimer--;

                // Adjust the position of the interaction hitbox according to the direction
                interactionHitbox.offset = directionVector * new Vector2(interactionDistance, interactionDistance);

                if (interactionTimer == 0) { interactionHitbox.enabled = false; }
            }


        }


        private void OnTriggerEnter2D(Collider2D other) {


            if (interactionHitbox.enabled) {

                // Check if the player is already carrying something
                if (currentlyCarried == null) {

                    // Check it the other object can be picked up from the depot
                    ItemDepot depot = other.gameObject.GetComponent<ItemDepot>();

                    if (depot != null && depot.HasPossessionOfChildObject()) {
                        currentlyCarried = depot.PickUp(this);

                        interactionHitbox.enabled = false;
                        interactionTimer = 0;
                    }

                } else {

                    ItemDepot depot = other.gameObject.GetComponent<ItemDepot>();

                    if (depot != null && depot.AcceptObject(currentlyCarried.gameObject) && !depot.HasPossessionOfChildObject()) {
                        depot.Drop();
                        currentlyCarried = null;

                        interactionHitbox.enabled = false;
                        interactionTimer = 0;
                    }
                    

                }
            }


        }


        /// <summary>
        /// Returns the normalized vector containing the current direction for movement. If there is no movement, a zero-vector is returned.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMovementDirection() { return directionVector; }


    }

}
