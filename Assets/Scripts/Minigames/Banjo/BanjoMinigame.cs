using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banjo : GenericMinigame
{
    private Rect boundaries;

    [SerializeField] private GameObject banjo;
    [SerializeField] private int notenum;
    [SerializeField] private GameObject note;
    [SerializeField] private GameObject[] trigger = new GameObject[3];
    //[SerializeField] private Canvas score;
    [SerializeField] private TMPro.TextMeshProUGUI scoreField;

    public float offset = 0.3f;
    public float trigger_y = 0f;
    public float spawn_y = 0f;
    public float velocity = 1f;
    public float toleranceRate = 0.3f;  // how close the note has to be to the trigger field
    public int numberNotes = 3;

    private float[] lanes;
    private List<GameObject> notes;
    private GameObject deathNote;
    private bool spawnNewNote = true;
    private int completedNotes = 0;
    private bool didYouHitANote = false;
    private float[] keyPositions;
    private bool finito = false;

    public override void StartUp()
    {
        SetBGColor(new Color(0.5f, 0.5f, 0.0f));
        boundaries = GetBounds();
        trigger_y = boundaries.yMin + offset;
        spawn_y = boundaries.yMax;
        lanes = new float[3];
        keyPositions = new float[3];
        SpawnTriggerFields();
        notes = new List<GameObject>();
        scoreField.transform.position = new Vector3(boundaries.xMin + 1.5f * offset, boundaries.yMax - offset);
    }
    private void SpawnTriggerFields()
    {
        for (int i = 0; i < 3; i++)
        {
            lanes[i] = boundaries.xMin + offset + 0.25f * (i + 1) * (boundaries.xMax - boundaries.xMin);
            trigger[i].transform.localPosition = new Vector3(lanes[i], trigger_y);
            keyPositions[i] = lanes[i]; // the positions for a (0), s (1), d (2)
        }
    }


    private void ButtonPressed(int key)
    {
        didYouHitANote = false;
        foreach (GameObject note in notes)
        {
            float yPosNote = note.transform.localPosition.y;
            float xPosNote = note.transform.localPosition.x;
            if (keyPositions[key] != xPosNote)
            {
                continue;
            }
            if (yPosNote > trigger_y - toleranceRate && yPosNote < trigger_y + toleranceRate)
            {
                completedNotes++;
                didYouHitANote = true;
                deathNote = note;
            }
        }
        if (deathNote != null)
        {
            notes.Remove(deathNote);
            Destroy(deathNote);
            deathNote = null;
        }
        if (!didYouHitANote)
        {
            completedNotes = 0;
        }

        scoreField.text = $"{completedNotes} / {numberNotes}";
    }
    // Update is called once per frame
    void Update()
    {
        if (finito)
        {
            return;
        }
        if (spawnNewNote)
        {
            SpawnNote();
            spawnNewNote = false;
            StartCoroutine(NoteInterval());
        }
        foreach (GameObject note in notes)
        {
            note.transform.localPosition = new Vector3(note.transform.localPosition.x, note.transform.localPosition.y - velocity * Time.deltaTime);
            // if note left the screen
            if(note.transform.localPosition.y < boundaries.yMin)
            {
                deathNote = note;
            }

        }
        if (deathNote != null)
        {
            notes.Remove(deathNote);
            Destroy(deathNote);
            deathNote = null;
        }

        if (Input.GetKeyDown("a"))
        {
            ButtonPressed(0);
        }
        if (Input.GetKeyDown("s"))
        {
            ButtonPressed(1);
        }
        if (Input.GetKeyDown("d"))
        {
            ButtonPressed(2);
        }
        if (completedNotes == numberNotes)
        {
            finito = true;
            StartCoroutine(Finished());
        }
    }

    private IEnumerator Finished()
    {
        yield return new WaitForSeconds(1);
        Close();
        Debug.Log("closing");
    }

    private IEnumerator NoteInterval()
    {
        float waitingTime = Random.Range(0.5f, 1.5f);
        yield return new WaitForSeconds(waitingTime);
        spawnNewNote = true;
    }
    private void SpawnNote()
    {
        int lane = Random.Range(0, 3);
        notes.Add(Instantiate(note, this.transform));
        notes[notes.Count - 1].transform.localPosition = new Vector3(lanes[lane], spawn_y);
    }
}
