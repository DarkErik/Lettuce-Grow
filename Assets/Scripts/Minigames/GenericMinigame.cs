using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericMinigame : MonoBehaviour
{
    private MinigameFrame outerFrame;

    private Rect bounds;
    
    public void Init(MinigameFrame outerFrame) {
        this.outerFrame = outerFrame;
        bounds = outerFrame.GetBounds();
        StartUp();
    }

    public abstract void StartUp();

    public Rect GetBounds() {
        return bounds;
    }

    public void SetBGColor(Color c) {
        outerFrame.SetBGColor(c);
    }

    public void Close() {
        outerFrame.Close();
    }
}
