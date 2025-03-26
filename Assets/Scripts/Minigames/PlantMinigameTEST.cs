using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantMinigameTEST : GenericMinigame
{
    [SerializeField] private Transform quad;

    public override void StartUp() {
        SetBGColor(Color.black);
    }

    public void Update() {
        quad.transform.position = Util.ClampV3IntoRect(CameraController.Instance.GetMouseWorld(), GetBounds());

        if (Input.GetMouseButtonDown(0)) {
            Close();
        }
    }
}
