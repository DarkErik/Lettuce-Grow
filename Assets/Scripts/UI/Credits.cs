using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    private int currentlyClicked = 0;

    private void Update() {
        if (Input.anyKeyDown) {
            currentlyClicked++;

            if (currentlyClicked >= 2) {
                ScreenTransition.Instance.LoadScene("MainMenu");
            }
        }
    }
}
