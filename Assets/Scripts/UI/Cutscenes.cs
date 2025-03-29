using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscenes : MonoBehaviour {
    public static bool playInitialCutscene = false;
    public static bool playFailureCutscene = false;

    [SerializeField] private CutsceneSO[] cutscenes;
    [SerializeField] private CutsceneSO intro;
    [SerializeField] private CutsceneSO failure;

    [SerializeField] private Transform textTransform;
    [SerializeField] private EnhancedTexts text;
    [SerializeField] private Animator animLeft;
    [SerializeField] private SpriteRenderer rendererLeft;
    [SerializeField] private Animator animRight;
    [SerializeField] private SpriteRenderer rendererRight;
    [SerializeField] private Vector3 textPositionLeftTalking;
    [SerializeField] private Vector3 textPositionRightTalking;
    private CutsceneSO cutScene = null;

    private void Start() {
        if (playFailureCutscene) {
            cutScene = failure;
            playFailureCutscene = false;
        }
        if (playInitialCutscene) {
            cutScene = intro;
            playInitialCutscene = false;
        }
        if (cutScene == null) {
            cutScene = cutscenes[GameManager.currentLevel];
        }

        StartCoroutine(DialogRoutine());
    }

    private IEnumerator DialogRoutine() {
        
        foreach (DialogPiece d in cutScene.dialog) {
            text.SetTextTyping(d.text, true);

            animLeft.SetBool("isTalking", d.leftSideTalking);
            animRight.SetBool("isTalking", !d.leftSideTalking);

            if (d.leftSideTalking)
                textTransform.position = textPositionLeftTalking;
            else
                textTransform.position = textPositionRightTalking;

            if (d.exchangeSpriteLeft != null)
                rendererLeft.sprite = d.exchangeSpriteLeft;

            if (d.exchangeSpriteRight != null)
                rendererRight.sprite = d.exchangeSpriteRight;

            
            yield return new WaitUntil(() => text.FinishedTyping());
            yield return new WaitUntil(() => Input.anyKeyDown);


        }

        yield return new WaitForSeconds(1);
        //ScreenTransition.Instance.LoadScene(cutScene.transitionToScene);
        if (cutScene == intro) {
            GameManager.currentLevel = 0;
        } else if (cutScene != failure) {
            GameManager.currentLevel++;
            if (GameManager.currentLevel >= 5) {
                ScreenTransition.Instance.LoadScene("MainMenu");
                yield break;
            }
        }
        ScreenTransition.Instance.LoadScene("Day_" + (GameManager.currentLevel + 1));

    }
}

