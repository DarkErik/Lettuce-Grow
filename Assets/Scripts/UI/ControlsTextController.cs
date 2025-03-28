using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsTextController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private bool open = false;
    public void Open() {
        anim.SetBool("open", true);
        open = true;
    }

    private void Update() {
        if (open && Input.anyKeyDown) {
            anim.SetBool("open", false);
        }
    }
}
