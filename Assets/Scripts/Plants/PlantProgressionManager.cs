using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FlowerpotBaseLogic))]
public class PlantProgressionManager : MonoBehaviour
{
    [SerializeField] private GameObject minigameGameObject; //MinigameFrame
    private MinigameFrame minigameFrame;
    [SerializeField] private GameObject bubbleGameObject; // PlantsNeedsBouble
    private PlantsNeedsBouble bubble;
    [SerializeField] private GameObject dustCloudPrefab;

    private FlowerpotBaseLogic flowerpotBaseLogic;
    private PlantSO plantData;

    private float currentGrothProgress;
    private bool isPlantPlanted;
    public bool IsPlantReadyToHarvest { private set; get; }

    private enum GrothPhase { NOCHANGE, PROGRESSING, REGRESSING }
    private GrothPhase currentGrothPhase;

    private Coroutine needTimer;

    private bool isNeedCurrentlyActive = false;
    private PlantNeed selectedNeed;

    private bool minigameHasStarted;
    private static bool globalMinigameHasStarted;

    private void Awake()
    {
        flowerpotBaseLogic = GetComponent<FlowerpotBaseLogic>();
        isPlantPlanted = false;
        currentGrothPhase = GrothPhase.NOCHANGE;
    }

    private void OnEnable()
    {
        flowerpotBaseLogic.OnPlanted += FlowerpotBaseLogic_OnPlanted;
    }
    private void OnDisable()
    {
        flowerpotBaseLogic.OnPlanted -= FlowerpotBaseLogic_OnPlanted;
    }

    private void FlowerpotBaseLogic_OnPlanted(object sender, FlowerpotBaseLogic.OnPlantedEventArgs e)
    {
        plantData = e.plantData;
        isPlantPlanted = true;
        currentGrothPhase = GrothPhase.PROGRESSING;
        currentGrothProgress = 0;
        minigameHasStarted = false;
        IsPlantReadyToHarvest = false;

        Debug.Log("Plant Progression Started");

        needTimer = StartCoroutine(NeedLoop());
    }

    private IEnumerator NeedLoop () { 
        yield return new WaitForSeconds(plantData.initalNeedDelayAfterPlanting);

        while (true) {
            float timeUntilNeed = Random.Range(plantData.minNeedTime, plantData.maxNeedTime);
            yield return new WaitForSeconds(timeUntilNeed);

            //float plantNeedCount = PlantNeed.GetNames(typeof(PlantNeed)).Length;
            float totalNeedWeight = 0;
            foreach (PlantNeed plantNeed in System.Enum.GetValues(typeof(PlantNeed))) {
                totalNeedWeight += plantData.plantNeedMapping[plantNeed].pickChance;               
            }

            float randomNeedPick = Random.Range(0f, 1f);
            foreach (PlantNeed plantNeed in System.Enum.GetValues(typeof(PlantNeed)))
            { 
                float relativeNeedWeight = plantData.plantNeedMapping[plantNeed].pickChance / totalNeedWeight;
                if (relativeNeedWeight >= randomNeedPick)
                {
                    // current PlantNeed got randomly selected
                    StartNeed(plantNeed);
                    break;
                }
                else {
                    randomNeedPick -= relativeNeedWeight;
                }
            }

            yield return new WaitUntil(() => minigameHasStarted);
            yield return new WaitUntil(() => minigameFrame.WasMinigameFinished);
            Destroy(minigameFrame.gameObject);

            minigameHasStarted = false;
            isNeedCurrentlyActive = false;
            globalMinigameHasStarted = false;
            currentGrothPhase = GrothPhase.PROGRESSING;

            bubble.Close();           
        }
    }

    private void StartNeed(PlantNeed plantNeed) {
        selectedNeed = plantNeed;

        Vector3 bubblePositionShift = new Vector3(0.8f, 0.5f, 0f);
        bubble = Instantiate(bubbleGameObject, this.gameObject.transform.position + bubblePositionShift, Quaternion.identity).GetComponent<PlantsNeedsBouble>();
        bubble.Init(selectedNeed);

        currentGrothPhase = GrothPhase.REGRESSING;
        isNeedCurrentlyActive = true;
    }


    private void Update()
    {
        if (currentGrothPhase == GrothPhase.PROGRESSING)
        {
            currentGrothProgress += Time.deltaTime;
            if (currentGrothProgress > plantData.neededGrothTime)
            {
                Debug.Log("The Plant has finished growing.");
                currentGrothPhase = GrothPhase.NOCHANGE;
                StopCoroutine(needTimer);

                IsPlantReadyToHarvest = true;
            }
        }
        else if (currentGrothPhase == GrothPhase.REGRESSING) 
        {
            currentGrothProgress -= Time.deltaTime * plantData.regressionSpeedFactor;
            if (currentGrothProgress < 0)
            {
                Debug.Log("The Plant has died.");
                currentGrothPhase = GrothPhase.NOCHANGE;
                StopCoroutine(needTimer);
               
                isNeedCurrentlyActive = false;
                bubble.Close();
                
                if (minigameHasStarted) {
                    minigameHasStarted = false;
                    globalMinigameHasStarted = false;

                    //forceclose minigame if the minigame belongs to this plant
                    minigameFrame.ForceClose();
                }

                StartCoroutine(DeathSequence());
            }
        }
    }

    private IEnumerator DeathSequence() {
        GameObject dustCloud = Instantiate(dustCloudPrefab, gameObject.transform);
        float clipLength = dustCloud.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLength);
        isPlantPlanted = false;

        flowerpotBaseLogic.DestroyPlant();
        Destroy(dustCloud);
    }

    public bool GetIsNeedCurrentlyActive() { 
        return isNeedCurrentlyActive;
    }

    public Player.PlayerController.CarriableItemTypes GetObjectForCurrentNeed() {
        if (!isNeedCurrentlyActive) return Player.PlayerController.CarriableItemTypes.invalid;

        return plantData.plantNeedMapping[selectedNeed].neededCarriableItem;
    }

    public void OnObjectForNeedProvided() {
        if (globalMinigameHasStarted) return;

        minigameFrame = Instantiate(minigameGameObject).GetComponent<MinigameFrame>();
        minigameFrame.Init(selectedNeed);
        minigameHasStarted = true;
        globalMinigameHasStarted = true;
    }

    public void Harvest() {
        IsPlantReadyToHarvest = false;
    }
}
