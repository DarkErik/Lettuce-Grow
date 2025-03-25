using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public Camera cam;


    public void Awake() {
        Instance = this;
        //Debug.Log("Cam Awake");
    }


    public Vector3 GetMouseWorld() {
        Vector3 res = cam.ScreenToWorldPoint(Input.mousePosition);
        res.z = 0;

        return res;
    }


}
