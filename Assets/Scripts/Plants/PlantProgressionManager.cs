using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(FlowerpotBaseLogic))]
public class PlantProgressionManager : MonoBehaviour
{
    [SerializeField] private GameObject minigame; //MinigameFrame
    private MinigameFrame minigameFrame;
    [SerializeField] private GameObject bubble; // PlantsNeedsBouble
    private PlantsNeedsBouble bubbleGameObject;

    private FlowerpotBaseLogic flowerpotBaseLogic;
    private PlantSO plantData;

    private float currentGrothProgress;
    private bool isPlantPlanted;

    private enum GrothPhase { NOCHANGE, PROGRESSING, REGRESSING }
    private GrothPhase currentGrothPhase;

    private Coroutine needTimer;

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
            Debug.Log($"Random pick: {randomNeedPick}");
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

            yield return new WaitUntil(() => minigameFrame.WasMinigameFinished);
            Destroy(minigameFrame.gameObject);

            break;            
        }
    }

    private void StartNeed(PlantNeed plantNeed) {
        Vector3 bubblePositionShift = new Vector3(0.8f, 0.5f, 0f);
        bubbleGameObject = Instantiate(bubble, this.gameObject.transform.position + bubblePositionShift, Quaternion.identity).GetComponent<PlantsNeedsBouble>();
        bubbleGameObject.Init(plantNeed);
    }


    private void Update()
    {
        if (currentGrothPhase == GrothPhase.PROGRESSING)
        {
            currentGrothProgress += Time.deltaTime;
            if (currentGrothProgress > plantData.neededGrothTime)
            {
                Debug.Log("Plant has finished growing");
                currentGrothPhase = GrothPhase.NOCHANGE;
            }
        }
        else if (currentGrothPhase == GrothPhase.REGRESSING) 
        {
            currentGrothProgress -= Time.deltaTime * plantData.regressionSpeedFactor;
            if (currentGrothProgress < 0)
            {
                Debug.Log("The Plant has died.");
                currentGrothPhase = GrothPhase.NOCHANGE;
            }
        }
    }
}
