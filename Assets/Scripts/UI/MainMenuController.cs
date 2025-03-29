using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator anykeyAnim;
    [SerializeField] private Animator menuAnim;

    public void Update() {
        if (Input.anyKeyDown) {
            anykeyAnim.SetTrigger("dissapear");
            menuAnim.SetTrigger("show");
        }
    }

    public void StartGame() {
        Debug.Log("Start Game");
        Cutscenes.playInitialCutscene = true;
        ScreenTransition.Instance.LoadScene("Cutscene");
    }
}
