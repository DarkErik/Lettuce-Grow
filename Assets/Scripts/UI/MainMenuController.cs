using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Animator anykeyAnim;
    [SerializeField] private Animator menuAnim;

    private int selectDay = 0;
    public void Update() {
        if (Input.anyKeyDown) {
            anykeyAnim.SetTrigger("dissapear");
            menuAnim.SetTrigger("show");

            this.Invoke(nameof(WaitForControllerButtonReleaseInputDelay), 1.0f);
            
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            selectDay = Mathf.Min(4, selectDay + 1);
        if (Input.GetKeyDown(KeyCode.LeftControl))
            selectDay = 0;
    }

    public void StartGame() {

        GameManager.currentLevel = selectDay;
        if (GameManager.currentLevel == 0)
            Cutscenes.playInitialCutscene = true;
        Debug.Log("Start Game");
        ScreenTransition.Instance.LoadScene("Cutscene");
    }

    public void RollCredits() {
        ScreenTransition.Instance.LoadScene("Credits");
    }

    private void WaitForControllerButtonReleaseInputDelay() {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("StartGame"));
        Debug.Log("Currently selected UI element: " + EventSystem.current.currentSelectedGameObject);
    }

}
