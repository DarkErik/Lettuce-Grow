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
            interactButton.onButtonUp += OnInteractionCancelled;
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
            interactionHitbox.enabled = false;
            interactionTimer = 0;

            if (currentlyCarried != null) {
                currentlyCarried.Drop();
                currentlyCarried = null;
            }
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


            if (interactionHitbox.enabled && currentlyCarried == null) {
                // Check it the other object can be carried
                Carrieable carrieable = other.gameObject.GetComponent<Carrieable>();

                if (carrieable != null && carrieable.GetIsCarrieable()) {
                    carrieable.PickUp(this);
                    currentlyCarried = carrieable;
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
