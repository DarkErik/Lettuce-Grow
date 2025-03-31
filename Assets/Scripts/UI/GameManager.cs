using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static int currentLevel = 0;
    public static bool easyMode = false;

    public static event EventHandler OnStressPhaseEntered;

    [SerializeField] private Level[] levels;
    
    [SerializeField] private TextMeshProUGUI saladProgress;
    [SerializeField] private TextMeshProUGUI carrotProgress;
    [SerializeField] private TextMeshProUGUI pumpkinProgress;

    [SerializeField] private TextMeshProUGUI dayBannerTxt;

    [SerializeField] private Image timePassedProgressBar;
    [SerializeField] private Animator vinigetteAnim;

    [Range(0.1f, 0.9f)]
    [SerializeField] private float stressPhaseEnteringThreshold;

    private int saladAmount = -1;
    private int carrotAmount = -1;
    private int pumpkinAmount = -1;

    private float levelStartedTime;
    private bool dayFinishedSucessfully = false;

    
    private bool stressPhaseHasBeenEntered = false;

    private void Awake() {
        Instance = this;
        levelStartedTime = Time.time;

        AddSalad();
        AddCarrot();
        AddPumpkin();

        dayBannerTxt.text = "DAY " + (currentLevel + 1);
    }

    public void Update() {
        float progress = (Time.time - levelStartedTime) / levels[currentLevel].dayTimeSeconds;
        timePassedProgressBar.fillAmount = progress;

        if (progress >= stressPhaseEnteringThreshold && !stressPhaseHasBeenEntered) {
            stressPhaseHasBeenEntered = true;
            MusicManager.Instance.GameManager_OnStressPhaseEntered();
            StartCoroutine(StartVinigetteDelayed());
            Debug.Log("Entered Stress Phase");
        }
        
        if (progress >= 1f && !dayFinishedSucessfully && !easyMode) {
            Debug.Log("Time up!");
            Cutscenes.playFailureCutscene = true;
            ScreenTransition.Instance.LoadScene("Cutscene");
        }
    }

    private IEnumerator StartVinigetteDelayed() {
        yield return new WaitForSeconds(2.5f);
        vinigetteAnim.SetTrigger("alarm");
    }
    public void AddSalad() {
        saladAmount++;

        saladProgress.text = saladAmount + "/" + levels[currentLevel].targetSalad;
        if (saladAmount >= levels[currentLevel].targetSalad)
            saladProgress.color = Color.green;

        CheckDemands();
    }

    public void AddCarrot() {
        carrotAmount++;

        carrotProgress.text = carrotAmount + "/" + levels[currentLevel].targetCarrot;
        if (carrotAmount >= levels[currentLevel].targetCarrot)
            carrotProgress.color = Color.green;

        CheckDemands();
    }

    public void AddPumpkin() {
        pumpkinAmount++;

        pumpkinProgress.text = pumpkinAmount + "/" + levels[currentLevel].targetPumpkin;
        if (pumpkinAmount >= levels[currentLevel].targetPumpkin)
            pumpkinProgress.color = Color.green;

        CheckDemands();
    }

    public void CheckDemands() {
        if (saladAmount >= levels[currentLevel].targetSalad && carrotAmount >= levels[currentLevel].targetCarrot && pumpkinAmount >= levels[currentLevel].targetPumpkin) {
            dayFinishedSucessfully = true;
            StartCoroutine(EndDay());
        }
    }

    private IEnumerator EndDay() {
        yield return new WaitForSeconds(1);
        ScreenTransition.Instance.LoadScene("Cutscene");
    }
}

[Serializable]
public class Level {
    public int targetSalad = 5;
    public int targetCarrot = 5;
    public int targetPumpkin = 5;

    public int dayTimeSeconds = 300;
}