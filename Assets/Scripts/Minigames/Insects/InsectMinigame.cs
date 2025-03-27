using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectMinigame : GenericMinigame
{
    [SerializeField] private Transform klatsche;
    [SerializeField] private int fliegenanzahl;
    [SerializeField] private GameObject fly;
    [SerializeField] private GameObject flutsch;
    public float offset = 0f;
    private int num_flies = 0;
    private Vector3[] flies_direction;
    public float fly_velocity = 1f;
    private GameObject[] flies;
    private Rect boundaries;
    public float klatschenradius = 1f;
    public Vector3 klatschenoffset = new Vector3(1f, 1f);
    private int deathCounter = 0;
    [SerializeField] private Animator klatschenanim;


    public override void StartUp()
    {
        SetBGColor(new Color(0.2f, 0.6f, 0.2f));
        SpawnFlies();
    }

    public void Klatsch()
    {
        for (int i = 0; i < num_flies; i++)
        {
            if (flies[i] == null)
            {
                continue;
            }
            float distance = (klatsche.transform.position + klatschenoffset - flies[i].transform.position).magnitude;
            if (distance < klatschenradius)
            {
                deathCounter++;
                GameObject deadFly = Instantiate(flutsch, this.transform);
                deadFly.transform.position = flies[i].transform.position;
                deadFly.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
                Destroy(flies[i]);
                if (deathCounter == num_flies)
                {
                    StartCoroutine(WaitAndClose());
                }
            }
        }
    }

    private void SpawnFlies()
    {
        boundaries = GetBounds();
        num_flies = Random.Range(fliegenanzahl - 1, fliegenanzahl + 2);
        flies = new GameObject[num_flies];
        flies_direction = new Vector3[num_flies];

        for (int i = 0; i < num_flies; i++)
        {
            float xPos = Random.Range(boundaries.xMin + offset, boundaries.xMax - offset);
            float yPos = Random.Range(boundaries.yMin + offset, boundaries.yMax - offset);
            flies[i] = Instantiate(fly, this.transform);
            flies[i].transform.position = new Vector3(xPos, yPos);
            flies_direction[i] = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        }
    }

    public void Update()
    {
        klatsche.transform.position = Util.ClampV3IntoRect(CameraController.Instance.GetMouseWorld(), boundaries);
        for (int i = 0; i < num_flies; i++)
        {
            if (flies[i] == null)
            {
                continue;
            }
            if (flies[i].transform.position.x < (boundaries.xMin + offset) || flies[i].transform.position.x > (boundaries.xMax - offset))
            {
                flies_direction[i].x *= -1;
            }
            if (flies[i].transform.position.y < (boundaries.yMin + offset) || flies[i].transform.position.y > (boundaries.yMax - offset))
            {
                flies_direction[i].y *= -1;
            }
            flies[i].transform.localPosition += flies_direction[i] * fly_velocity * Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(0))
        {
            klatschenanim.SetTrigger("klatsch");
        }
    }

    private IEnumerator WaitAndClose()
    {
        yield return new WaitForSeconds(1);
        Close();
        Debug.Log("closing");
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(klatsche.transform.position + klatschenoffset, klatschenradius);
    }
}
