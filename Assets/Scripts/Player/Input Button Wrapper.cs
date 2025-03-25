using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputButtonWrapper {

    private InputAction action;
    private bool isPressed;

    public System.Action<InputAction.CallbackContext> onButtonDown, onButtonUp, onButtonPressed;

    /// <summary>
    /// Concerns the onButtonPressed callback:
    /// Sets the timeframe for deciding whether the button is pressed or held down. All event callbacks will be delayed by this time (except onButtonPressed, this will always be called earlier).
    /// Set to TimeSpan.Zero to disable this feature.
    /// </summary>
    public TimeSpan buttonPressTime = TimeSpan.Zero;
    private Task delayedCall;


    private CancellationTokenSource tokenSource = new CancellationTokenSource();



    /// <summary>
    /// Creates a new wrapper around a button input to support extra features.
    /// </summary>
    /// <param name="action">The button action to attach to</param>
    public InputButtonWrapper(InputAction action) : this(action, TimeSpan.Zero) { }


    /// <summary>
    /// Creates a new wrapper around a button input to support extra features.
    /// </summary>
    /// <param name="action">The button action to attach to</param>
    /// <param name="buttonPressTime">The timeframe for a buttonpress to be decided. See <see cref="buttonPressTime"/> for more details.</param>
    public InputButtonWrapper(InputAction action, TimeSpan buttonPressTime) { 
        this.action = action;
        this.buttonPressTime = buttonPressTime;

        this.action.started += OnStart;
        this.action.canceled += OnCancel;
    }


    /// <summary>
    /// Checks if the button is currently held down
    /// </summary>
    /// <returns>True if the button is currently held down</returns>
    public bool IsPressed() {
        return isPressed;
    }


    private void OnStart(InputAction.CallbackContext ctx) {
        isPressed = true;

        if (buttonPressTime > TimeSpan.Zero) {
            // Delay the onButtonDown call to give a timeframe for the onButtonPressed event to be triggered
            tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;

            delayedCall = new Task(() => { Thread.Sleep(buttonPressTime); if (cancellationToken.IsCancellationRequested) { return; } onButtonDown(ctx); });
            delayedCall.Start();

            return;
        }

        if (onButtonDown != null) { onButtonDown(ctx); }
    }


    private void OnCancel(InputAction.CallbackContext ctx) {
        isPressed = false;

        if (buttonPressTime > TimeSpan.Zero && delayedCall != null && !delayedCall.IsCompleted) {
            tokenSource.Cancel();
            delayedCall = null;

            if (onButtonPressed != null) { onButtonPressed(ctx); }
            return;
        }

        delayedCall = null;
        if (onButtonUp != null) { onButtonUp(ctx); }
    }




}
