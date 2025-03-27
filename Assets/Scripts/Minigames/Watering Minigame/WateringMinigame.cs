using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringMinigame : GenericMinigame {

    private static PlayerInputActions inputWrapper;
    private static PlayerInputActions.MinigameActions controller;
    private static InputButtonWrapper interactButton;

    private static System.Random random = new System.Random();


    #region Inspector parameters

    [SerializeField]
    private GameObject wateringCan;

    [SerializeField]
    private ParticleSystem waterDropletParticleSystem;

    [SerializeField]
    private WaterdropsParticleHandler particleSystemHandler;

    [SerializeField]
    private float yPosition;


    [SerializeField]
    private float wateringCanTiltAngle = 315;

    [SerializeField]
    private float wateringCanTiltSpeed = 1;


    [SerializeField]
    private GameObject plantPrefab;

    [SerializeField]
    private float plantY;

    [SerializeField]
    private float plantWidth;

    [SerializeField, Tooltip("Offset the spawn position from the left and right border - Otherwise some plants on the left border may not be reachable")]
    private Vector2 positionOffset;


    [SerializeField, Tooltip("The min and max amount of plants")]
    private Vector2Int numPlants;

    [SerializeField, Tooltip("The mean and standard deviation for selecting the number of plants")]
    private Vector2 normDistrValues;


    [SerializeField, Tooltip("Delay for closing the minigame after finishing")]
    private float closeDelay;

    #endregion

    private bool isRunning;
    private GameObject[] plants;
    private int remainingPlants;




    #region Input setup logic
    private void InitControls() {

        // Only need to initialize the input controller once
        if (inputWrapper != null) { return; }

        inputWrapper = new PlayerInputActions();
        controller = inputWrapper.Minigame;

        interactButton = new InputButtonWrapper(controller.PrimaryInteract);
        controller.Enable();
    }

    private void OnDisable() {
        controller.Disable();
    }

    #endregion



    public override void StartUp() {
        SetBGColor(new Color(135 / 255f, 205 / 255f, 250 / 255f));
        InitControls();


        SpawnPlants();
        remainingPlants = plants.Length;

        isRunning = true;

        // this is bad. Dont do this. This should belong into OnEnable if it would only work :(
        controller.Enable();
    }


    /// <summary>
    /// Randomly distributes all plant according to the available space
    /// </summary>
    private void SpawnPlants() {

        Rect bounds = GetBounds();
        float leftmostPlant = bounds.xMin + positionOffset.x;

        int numPlants = Mathf.Clamp(Mathf.RoundToInt((float)SampleGaussian(random, normDistrValues.x, normDistrValues.y)), this.numPlants.x, this.numPlants.y);
        double widthPerPlant = (double)(bounds.width - positionOffset.x - positionOffset.y) / numPlants;
        plants = new GameObject[numPlants];

        //Debug.Log(numPlants + " " + widthPerPlant + " " + leftmostPlant);
        for (int i = 0; i < plants.Length; i++) {
            plants[i] = GameObject.Instantiate(plantPrefab);

            plants[i].transform.parent = this.transform;
            // Holy frick, this was a giant pain. NO TOUCHY!
            plants[i].transform.position = new Vector3(Mathf.Clamp((float)SampleGaussian(random, bounds.xMin + positionOffset.x + widthPerPlant * i + widthPerPlant / 2, widthPerPlant / 2 - plantWidth), leftmostPlant + plantWidth, bounds.xMin + positionOffset.x + (float)widthPerPlant * (i + 1) - plantWidth), plantY, 0);
            leftmostPlant = plants[i].transform.position.x;

            particleSystemHandler.AddPlant(plants[i].GetComponent<GrowingPlant>());
        }

    }


    private static double SampleGaussian(System.Random random, double mean, double stddev) {
        // The method requires sampling from a uniform random of (0,1]
        // but Random.NextDouble() returns a sample of [0,1).
        double x1 = 1 - random.NextDouble();
        double x2 = 1 - random.NextDouble();

        double y1 = System.Math.Sqrt(-2.0 * System.Math.Log(x1)) * System.Math.Cos(2.0 * System.Math.PI * x2);
        return y1 * stddev + mean;
    }



    public void FixedUpdate() {

        if (!isRunning) { return; }

        Rect bounds = GetBounds();
        wateringCan.transform.position = new Vector3(Mathf.Clamp(CameraController.Instance.GetMouseWorld().x, bounds.xMin, bounds.xMax), yPosition, wateringCan.transform.position.z);


        // isEmitting seems to be the best one - isStopped needs the current cycle to end before it updates
        //Debug.Log(waterDropletParticleSystem.isPlaying + " " + waterDropletParticleSystem.isStopped + " " + waterDropletParticleSystem.isEmitting);

        if (interactButton.IsPressed()) {

            wateringCan.transform.eulerAngles = new Vector3(wateringCan.transform.eulerAngles.x, wateringCan.transform.eulerAngles.y, Mathf.Max((wateringCan.transform.eulerAngles.z == 0 ? 360 : wateringCan.transform.eulerAngles.z) - wateringCanTiltSpeed, wateringCanTiltAngle));

            // Turn on the water droplets
            if (wateringCan.transform.eulerAngles.z < wateringCanTiltAngle + 5 && !waterDropletParticleSystem.isEmitting) { waterDropletParticleSystem.Play(); }
        } else {

            // Snap to 0 degrees
            if (wateringCan.transform.eulerAngles.z > wateringCanTiltSpeed) {
                wateringCan.transform.eulerAngles = new Vector3(wateringCan.transform.eulerAngles.x, wateringCan.transform.eulerAngles.y, Mathf.Min(wateringCan.transform.eulerAngles.z + wateringCanTiltSpeed, 360));
            } else {
                wateringCan.transform.eulerAngles = new Vector3(wateringCan.transform.eulerAngles.x, wateringCan.transform.eulerAngles.y, 0);
            }

            // Turn off the water droplets
            if (wateringCan.transform.eulerAngles.z >= wateringCanTiltAngle + 5 && waterDropletParticleSystem.isEmitting) { waterDropletParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting); }
        }


    }


    public void PlantFullyGrownCallback() {
        remainingPlants--;

        if (remainingPlants == 0) {
            this.Invoke(nameof(Close), closeDelay);
        }
    }

}
