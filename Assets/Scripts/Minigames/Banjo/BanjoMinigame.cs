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

    public float offset = 0.3f;
    public float trigger_y = 0f;
    public float spawn_y = 0f;
    public float velocity = 1f;

    private float[] lanes;
    private List<GameObject> notes;
    private Vector3[] noteSpawns;


    public override void StartUp()
    {
        SetBGColor(new Color(0.5f, 0.5f, 0.0f));
        boundaries = GetBounds();
        trigger_y = boundaries.yMin + offset;
        spawn_y = boundaries.yMax;
        lanes = new float[3];
        SpawnTriggerFields();
        notes = new List<GameObject>();
    }

    private void SpawnTriggerFields()
    {

        for (int i = 0; i < 3; i++)
        {
            lanes[i] = boundaries.xMin + offset + 0.25f * (i + 1) * (boundaries.xMax - boundaries.xMin);
            trigger[i].transform.localPosition = new Vector3(lanes[i], trigger_y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SpawnNote();
        foreach (GameObject note in notes)
        {
            note.transform.localPosition = new Vector3(note.transform.localPosition.x, note.transform.localPosition.y - velocity * Time.deltaTime);
        }
    }

    private void SpawnNote()
    {
        int lane = Random.Range(0, 3);
        notes.Add(Instantiate(note, this.transform));
        notes[notes.Count - 1].transform.localPosition = new Vector3(lanes[lane], spawn_y);
    }
}
