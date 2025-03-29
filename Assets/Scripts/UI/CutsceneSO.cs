using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CutsceneSO : ScriptableObject
{
    //public string transitionToScene;
    public DialogPiece[] dialog;
}

[System.Serializable]
public class DialogPiece {
    [TextArea(5, 10)]
    public string text;
    public bool leftSideTalking;
    public Sprite exchangeSpriteLeft;
    public Sprite exchangeSpriteRight;
}