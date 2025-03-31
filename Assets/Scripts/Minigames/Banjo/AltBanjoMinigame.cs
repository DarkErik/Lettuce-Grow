using Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AltBanjoMinigame : GenericMinigame {


    private PlayerInputActions inputWrapper;
    private PlayerInputActions.MinigameActions controller;
    private InputButtonWrapper interactButton;
    private MouseInputHelper mouseInputHelper;
    private bool initializedControls;


    #region Inspector fields

    [SerializeField]
    private int lineSegmentResolution;

    [SerializeField]
    private List<float> yPositions;

    [SerializeField]
    private Material lineMaterial;

    [SerializeField]
    private float lineThickness;

    [SerializeField]
    private Color lineColor;

    [SerializeField]
    private string sortingLayerName;

    [SerializeField]
    private float timeScale = 1;

    [SerializeField]
    private float amplitude = 1;

    [SerializeField, Tooltip("Random between two constants")]
    private Vector2Int numNotes;

    [SerializeField]
    private float noteY;

    [SerializeField, Tooltip("Spacing along the x axis")]
    private float noteSpacing;

    [SerializeField]
    private float noteSize = 1;

    [SerializeField]
    private List<Sprite> noteSprites;

    [SerializeField]
    private Vector2 notePitchRange = new Vector2(1, 1);


    [SerializeField]
    private float closeDelay = 1;

    #endregion

    // The beginning and end of each BanjoString's line renderer
    private Vector2 lineStartEnd;

    private BanjoString[] banjoStrings;
    private Note[] notes;

    /// <summary>
    /// The index of the next note to play
    /// </summary>
    private int noteIndex = 0;


    /// <summary>
    /// Maps each directional vector to their specific key value
    /// </summary>
    private static (Vector2 vector, int key)[] inputAngleToKeyMap = new (Vector2, int)[] {
        (new Vector2(-1, 0), 0),
        (new Vector2(0, -1), 1),
        (new Vector2(1, 0), 2),
        (new Vector2(0, 1), 3)
    };
    /// <summary>
    /// Set to true when no input occured in the last frame. Prevents holding down any of the buttons
    /// </summary>
    private bool hasInputReset = true;



    #region Input setup logic
    private void InitControls() {

        inputWrapper = new PlayerInputActions();
        controller = inputWrapper.Minigame;

        interactButton = new InputButtonWrapper(controller.PrimaryInteract);
        mouseInputHelper = new MouseInputHelper(controller.MouseMovement, System.TimeSpan.FromMilliseconds(150));

        initializedControls = true;
        controller.Enable();
    }


    private void OnEnable() {
        if (initializedControls) { controller.Enable(); }
    }

    private void OnDisable() {
        if (initializedControls) { controller.Disable(); }
    }

    #endregion



    public override void StartUp() {
        SetBGColor(new Color(255 / 255f, 196 / 255f, 196 / 255f));
        InitControls();

        lineStartEnd = new Vector2(GetBounds().xMin - 0.2f, GetBounds().xMax + 0.2f);

        CreateBanjoStrings();
        GenerateNotes();
    }



    private void CreateBanjoStrings() {

        banjoStrings = new BanjoString[yPositions.Count];
        float length = Mathf.Abs(lineStartEnd.x) + Mathf.Abs(lineStartEnd.y);

        for (int i = 0; i < yPositions.Count; i++) {

            GameObject gameObject = new GameObject("Banjo String " + i);
            gameObject.transform.parent = transform;

            LineRenderer line = gameObject.AddComponent<LineRenderer>();

            line.positionCount = lineSegmentResolution;

            for (int i2 = 0; i2 < line.positionCount; i2++) {
                line.SetPosition(i2, new Vector3(lineStartEnd.x + length / lineSegmentResolution * i2, yPositions[i]));
            }

            // Configure the color
            Gradient gradient = new Gradient();
            gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(lineColor, 0), new GradientColorKey(lineColor, 1) }, new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
            line.colorGradient = gradient;

            line.material = lineMaterial;
            line.startWidth = lineThickness;
            line.endWidth = lineThickness;

            line.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            line.sortingLayerName = sortingLayerName;
            line.sortingOrder = 5;

            banjoStrings[i] = new BanjoString(line, yPositions[i], amplitude, 0.04f, lineStartEnd, timeScale);
        }

    }

    private void GenerateNotes() {

        notes = new Note[Random.Range(numNotes.x, numNotes.y)];

        for (int i = 0; i < notes.Length; i++) {
            notes[i] = new Note(Random.Range(0, banjoStrings.Length), (i, notes.Length), transform, noteY, noteSpacing, noteSize, noteSprites, sortingLayerName);
        }

    }


    /// <summary>
    /// Returns the key of the closest matching vector according to the <see cref="Banjo.inputAngleToKeyMap"/> lookup table.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private int ConvertInputvectorToKey(Vector2 input) {

        float minDifference = float.MaxValue;
        int minIndex = -1;

        for (int i = 0; i < inputAngleToKeyMap.Length; i++) {

            float angle = Vector2.Angle(input, inputAngleToKeyMap[i].vector);

            if (angle < minDifference) {
                minDifference = angle;
                minIndex = i;
            }
        }

        return inputAngleToKeyMap[minIndex].key;
    }

    

    private void PluckString(int key) {

        if (key > banjoStrings.Length) { return; }
        
        banjoStrings[key].Pluck();
        


        // Check if the input is correct
        if (notes[noteIndex].GetKey() != key) {

            // Wrong input
            for (int i = 0; i < notes.Length;i++) { notes[i].SetSuccessfullyPlayed(false); }
            noteIndex = 0;
            
            SoundManager.Instance.PlayBanjoMissedSound(transform.position);

            return;
        }

        notes[noteIndex].SetSuccessfullyPlayed(true);
        SoundManager.Instance.PlayBanjoHitSound(transform.position, 1, notePitchRange.x + key * (notePitchRange.y - notePitchRange.x) / (banjoStrings.Length - 1));
        noteIndex++;

        // Check if the minigame is finished
        if (noteIndex >= notes.Length) { this.Invoke(nameof(Close), closeDelay); }
    }



    private void FixedUpdate() {

        foreach (BanjoString banjoString in banjoStrings) {
            banjoString.Oscillate();
        }

        // Ignore all inputs after the game has finished (and wait for this object to be destroyed)
        if (noteIndex >= notes.Length) { return ; }


        Vector2 inputVector = controller.DirectionalButtons.ReadValue<Vector2>();

        if (hasInputReset && Mathf.Max(Mathf.Abs(inputVector.x), Mathf.Abs(inputVector.y)) > 0.8f) {
            hasInputReset = false;
            PluckString(ConvertInputvectorToKey(inputVector));

        } else if (Mathf.Max(Mathf.Abs(inputVector.x), Mathf.Abs(inputVector.y)) < 0.2f) {
            hasInputReset = true;
        }

    }


    private class BanjoString {

        private LineRenderer line;
        private float yPosition;

        private float amplitude, damping;

        private Vector2 lineStartEnd;

        private float oscilationTimer = 0, timeScale = 1;


        public BanjoString(LineRenderer line, float yPosition, float amplitude, float damping, Vector2 lineStartEnd, float timeScale) {
            this.line = line;
            this.yPosition = yPosition;
            this.amplitude = amplitude;
            this.damping = damping;
            this.lineStartEnd = lineStartEnd;
            this.timeScale = timeScale;

            oscilationTimer = float.MaxValue;
        }


        public void Oscillate() {


            if (oscilationTimer > 3) { return; }
            oscilationTimer += Time.deltaTime;

            float length = Mathf.Abs(lineStartEnd.x) + Mathf.Abs(lineStartEnd.y);

            // Calculate as much as possible beforehand
            float y = Mathf.Sin(oscilationTimer * timeScale) * amplitude * Mathf.Exp(-damping * oscilationTimer * timeScale);       // Actual physics based formula, yay

            for (int i = 0; i < line.positionCount; i++) {
                float x = lineStartEnd.x + length / line.positionCount * i;

                // Sine wave
                //line.SetPosition(i, new Vector3(x, yPosition + Mathf.Sin((float)i / line.positionCount * Mathf.PI + Time.timeSinceLevelLoad)));

                // Standing wave
                line.SetPosition(i, new Vector3(x, yPosition + Mathf.Sin((float)i / line.positionCount * Mathf.PI) * y ));
            }
        }


        public void Pluck() {
            oscilationTimer = 0;
        }

    }

    
    private class Note {

        private SpriteRenderer renderer;

        private int key;

        public Note(int key, (int current, int total) number, Transform parent, float y, float spacing, float noteSize, List<Sprite> sprites, string sortingLayerName) {
            this.key = key;

            GameObject gameObject = new GameObject("Note " + number.current);
            gameObject.transform.parent = parent;
            gameObject.transform.position = new Vector3(-spacing * number.total / 2f + spacing / 2 + number.current * spacing, y);
            gameObject.transform.localScale = new Vector3(noteSize, noteSize);

            renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprites[key];
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder = 5;
            
            renderer.color = new Color(64 / 255f, 64 / 255f, 64 / 255f);
        }

        public void SetSuccessfullyPlayed(bool success) {

            if (success) {
                renderer.color = Color.white;
            } else {
                renderer.color = new Color(64 / 255f, 64 / 255f, 64 / 255f);
            }
        }

        public int GetKey() { return key; }

    }

}
