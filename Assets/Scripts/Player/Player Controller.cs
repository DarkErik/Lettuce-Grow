using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {

    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        public enum CarriableItemTypes {
            invalid = -1,
            none = 0,
            unspecifiedObject = 1,
            seed,
        }


        private PlayerInputActions inputWrapper;
        private PlayerInputActions.PlayerActions controller;
        private InputButtonWrapper interactButton;

        [SerializeField]
        private new Rigidbody2D rigidbody;

        [SerializeField]
        private float movementSpeed;
        private bool canMove = true;

        /// <summary>
        /// Vector containing the current direction of movement - normalized
        /// </summary>
        private Vector2 directionVector;
        private Vector2 lastValidDirectionVector;


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
        private CarriableItemTypes currentlyCarriedType = CarriableItemTypes.none;

        [SerializeField]
        private PlayerAnimation playerAnimation;

        #region Input setup logic
        private void Awake() {
            Instance = this;

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




        private void FixedUpdate()
        {

            // Movement
            if (canMove)
            {
                if (directionVector != Vector2.zero) { lastValidDirectionVector = directionVector; }
                directionVector = controller.Move.ReadValue<Vector2>().normalized;

                // Animation stuff
                playerAnimation.ChangeRunning(directionVector != Vector2.zero);

                if (interactionTimer > 0)
                {
                    interactionTimer--;

                    // Adjust the position of the interaction hitbox according to the direction
                    if (directionVector == Vector2.zero) { directionVector = lastValidDirectionVector; }
                    interactionHitbox.offset = directionVector * new Vector2(interactionDistance, interactionDistance);

                    if (interactionTimer == 0) { interactionHitbox.enabled = false; }
                }
            }
            else
            {
                playerAnimation.ChangeRunning(false);
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
                        currentlyCarriedType = CarriableItemTypes.unspecifiedObject;

                        interactionHitbox.enabled = false;
                        interactionTimer = 0;
                    }

                    // Check it the other object can be picked up from the depot
                    SeedDispenser seedDispenser = other.gameObject.GetComponent<SeedDispenser>();

                    if (seedDispenser != null && seedDispenser.HasSeeds()) {
                        currentlyCarried = seedDispenser.PickUp(this);
                        currentlyCarriedType = CarriableItemTypes.seed;

                        interactionHitbox.enabled = false;
                        interactionTimer = 0;
                    }


                } else {

                    switch (currentlyCarriedType) {

                        case CarriableItemTypes.unspecifiedObject: {
                                ItemDepot depot = other.gameObject.GetComponent<ItemDepot>();

                                if (depot != null && depot.AcceptObject(currentlyCarried.gameObject) && !depot.HasPossessionOfChildObject()) {
                                    depot.Drop();

                                    ResetCarrying();
                                }

                                break;
                            }


                        case CarriableItemTypes.seed: {
                                FlowerpotBaseLogic pot = other.gameObject.GetComponent<FlowerpotBaseLogic>();

                                if (pot != null && pot.CanPlant(currentlyCarried.gameObject)) {
                                    pot.Plant(currentlyCarried.gameObject);
                                    currentlyCarried.Drop();

                                    ResetCarrying();

                                }

                                SeedDispenser seedDispenser = other.gameObject.GetComponent<SeedDispenser>();

                                if (seedDispenser != null && seedDispenser.AcceptObject(currentlyCarried.gameObject)) {
                                    seedDispenser.Drop();

                                    ResetCarrying();
                                }

                                break;
                            }



                        default: {
                                Debug.Log("Unspecified action for " + currentlyCarried);
                                break;
                            }

                    }

                    
                    

                }
            }


        }


        private void ResetCarrying() {
            currentlyCarried = null;
            currentlyCarriedType = CarriableItemTypes.none;

            interactionHitbox.enabled = false;
            interactionTimer = 0;
        }


        /// <summary>
        /// Returns the normalized vector containing the current direction for movement. If there is no movement, a zero-vector is returned.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMovementDirection() { return directionVector; }

        /// <summary>
        /// Returns the normalized vector containing the last valid/non-zero directional input.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetLastMovementDirection() { return lastValidDirectionVector; }

        /// <summary>
        /// Disables or enables player movement 
        /// </summary>
        /// <param name="flag">new State</param>
        public void SetCanMove(bool flag) {
            canMove = flag;
        }
    }

}
