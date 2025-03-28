using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;


/// <summary>
/// Helps determining if the mouse has been used as an input device recently. Update should be called everytime after the activity has been checked.
/// </summary>
public class MouseInputHelper {

    InputAction mouseMoveAction;

    private DateTime lastMouseInput = DateTime.MinValue;                            // The last time the mouse was used
    private TimeSpan mouseInputGracePeriod;                                         // The period to wait before ignoring the mouse again
    private Vector2 lastMousePosWorld;                                                // The position of the mouse from the last check


    /// <summary>
    /// Creates a new instance of the MouseInputHelper class
    /// </summary>
    /// <param name="gracePeriod">The time after which the mouse is considered "inactive"</param>
    public MouseInputHelper(InputAction mouseMoveAction, TimeSpan gracePeriod) { 
        this.mouseMoveAction = mouseMoveAction;
        this.mouseInputGracePeriod = gracePeriod;
    }

    /// <summary>
    /// Checks if the mouse is considered active according to all given criteria
    /// </summary>
    /// <returns></returns>
    public bool IsMouseActive() {
        return mouseMoveAction.activeControl != null || (DateTime.Now - lastMouseInput) < mouseInputGracePeriod;
    }

    /// <summary>
    /// Updated the mouse activity. Automatically determines the last mouse position.
    /// </summary>
    public void Update() {
        Update(CameraController.Instance.GetMouseWorld());
    }

    /// <summary>
    /// Updated the mouse activity. Uses the given mouse world position. Use this override to save one redundant call to <see cref="CameraController.GetMouseWorld()">CameraController.Instance.GetMouseWorld()</see>.
    /// </summary>
    /// <param name="mousePos">The current mouse position in world coordinates</param>
    public void Update(Vector2 mousePos) {
        if (mouseMoveAction.activeControl != null || mousePos != lastMousePosWorld) { lastMouseInput = DateTime.Now; }
        lastMousePosWorld = mousePos;
    }

}
