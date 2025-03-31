using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlsTextController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private bool open = false;
    public void Open() {
        anim.SetBool("open", true);
        open = true;
        
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void Update() {
        if (open && Input.anyKeyDown) {
            anim.SetBool("open", false);
            open = false;

            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }
}
