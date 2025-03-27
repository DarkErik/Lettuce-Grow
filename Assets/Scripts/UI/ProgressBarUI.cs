using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Color progressingColor;
    [SerializeField] private Color regressingColor;

    [SerializeField] private Image progressBar;
    [SerializeField] private PlantProgressionManager manager;

    private Animator progressBarAnimator;
    private PlantProgressionManager.GrothPhase currentGrothPhase = PlantProgressionManager.GrothPhase.NOCHANGE;

    private void Awake()
    {
        progressBarAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentGrothPhase != PlantProgressionManager.GrothPhase.PROGRESSING && manager.GetCurrentGrothPhase() == PlantProgressionManager.GrothPhase.PROGRESSING) {
            progressBar.color = progressingColor;
            SetRegressionAnimationBool(false);

        }
        if (currentGrothPhase != PlantProgressionManager.GrothPhase.REGRESSING && manager.GetCurrentGrothPhase() == PlantProgressionManager.GrothPhase.REGRESSING)
        {
            progressBar.color = regressingColor;
            SetRegressionAnimationBool(true);
        }

        progressBar.fillAmount = manager.GetGrothProgressInPercent();
    }

    private void SetRegressionAnimationBool (bool isRegressionActive){
        progressBarAnimator.SetBool("isRegressionActive", isRegressionActive);
    }
}
