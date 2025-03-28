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
            finishedPlant,
            wateringCan,
            banjo,
            flySwatter,
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
        /// <summary>
        /// The source where this object has been picked up (Depot, Dispenser, etc.)
        /// </summary>
        private GameObject pickupSource;


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

                rigidbody.velocity = directionVector * new Vector2(movementSpeed, movementSpeed);


                // Animation stuff
                if (playerAnimation != null) {
                    playerAnimation.ChangeRunning(directionVector != Vector2.zero);
                }
                

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
                if (playerAnimation != null)
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
                        currentlyCarriedType = depot.GetItemType();
                        pickupSource = depot.gameObject;

                        ResetInteraction();
                        return;
                    }

                    // Check it the other object can be picked up from the depot
                    SeedDispenser seedDispenser = other.gameObject.GetComponent<SeedDispenser>();

                    if (seedDispenser != null && seedDispenser.HasSeeds()) {
                        currentlyCarried = seedDispenser.PickUp(this);
                        currentlyCarriedType = CarriableItemTypes.seed;
                        pickupSource = seedDispenser.gameObject;

                        ResetInteraction();
                        return;
                    }

                    // Check it the other object can be picked up from a flower pot
                    FlowerpotBaseLogic flowerPot = other.gameObject.GetComponent<FlowerpotBaseLogic>();

                    if (flowerPot != null) {
                        if (flowerPot.CanBeHarvested()) {
                            currentlyCarried = flowerPot.Harvest().GetComponent<Carrieable>();
                            currentlyCarried.PickUp(this);
                            currentlyCarriedType = CarriableItemTypes.finishedPlant;
                            pickupSource = flowerPot.gameObject;

                            ResetInteraction();
                            return;

                        } else if (flowerPot.IsDeposited()) {
                            currentlyCarried = flowerPot.PickUp().GetComponent<Carrieable>();
                            currentlyCarried.PickUp(this);
                            currentlyCarriedType = CarriableItemTypes.finishedPlant;
                            pickupSource = flowerPot.gameObject;

                            ResetInteraction();
                            return;
                        }
                        
                    }


                } else {

                    switch (currentlyCarriedType) {

                        case CarriableItemTypes.wateringCan:
                        case CarriableItemTypes.banjo:
                        case CarriableItemTypes.flySwatter: {

                                ItemDepot depot = other.gameObject.GetComponent<ItemDepot>();

                                if (depot != null && depot.AcceptObject(currentlyCarried.gameObject) && !depot.HasPossessionOfChildObject()) {
                                    depot.Drop();

                                    ResetCarrying();
                                    break;
                                }

                                PlantProgressionManager plantProgressionManager = other.gameObject.GetComponent<PlantProgressionManager>();

                                if (plantProgressionManager != null && plantProgressionManager.GetIsNeedCurrentlyActive() && canMove) {
                                    // TODO: use plantProgressionManager.GetObjectForCurrentNeed to check for the right object if more objects are available
                                    plantProgressionManager.OnObjectForNeedProvided();
                                }

                                break;
                            }


                        case CarriableItemTypes.seed: {
                                FlowerpotBaseLogic pot = other.gameObject.GetComponent<FlowerpotBaseLogic>();
                                GameObject plant = pickupSource.GetComponent<SeedDispenser>().GetPlantPrefab();


                                if (pot != null && pot.CanPlant(plant)) {
                                    pot.Plant(GameObject.Instantiate(plant));
                                    GameObject.Destroy(currentlyCarried.gameObject);

                                    ResetCarrying();
                                    break;
                                }

                                SeedDispenser seedDispenser = other.gameObject.GetComponent<SeedDispenser>();

                                if (seedDispenser != null && seedDispenser.AcceptObject(currentlyCarried.gameObject)) {
                                    seedDispenser.Drop();

                                    ResetCarrying();
                                    break;
                                }

                                break;
                            }

                        case CarriableItemTypes.finishedPlant: {
                                FlowerpotBaseLogic pot = other.gameObject.GetComponent<FlowerpotBaseLogic>();

                                if (pot != null) {
                                    
                                    if (pot.IsEmpty()) {
                                        pot.Deposit(currentlyCarried.gameObject);

                                        ResetCarrying();
                                        break;
                                    } else if (pot.CanBeSwapped(currentlyCarried.gameObject)) {
                                        currentlyCarried = pot.Swap(currentlyCarried.gameObject).GetComponent<Carrieable>();
                                        currentlyCarried.PickUp(this);

                                        ResetInteraction();
                                        break;
                                    }
                                }

                                SellBox sellBox = other.gameObject.GetComponent<SellBox>();

                                if (sellBox != null && sellBox.CanBeSold(currentlyCarried.gameObject)) {
                                    sellBox.Sell(currentlyCarried.gameObject);

                                    ResetCarrying();
                                    break;
                                }


                                break;
                            }


                        case CarriableItemTypes.unspecifiedObject: {
                                ItemDepot depot = other.gameObject.GetComponent<ItemDepot>();

                                if (depot != null && depot.AcceptObject(currentlyCarried.gameObject) && !depot.HasPossessionOfChildObject()) {
                                    depot.Drop();

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


        private void ResetInteraction() {
            interactionHitbox.enabled = false;
            interactionTimer = 0;
        }

        private void ResetCarrying() {
            currentlyCarried = null;
            currentlyCarriedType = CarriableItemTypes.none;
            pickupSource = null;

            ResetInteraction();
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
