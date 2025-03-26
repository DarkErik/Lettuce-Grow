using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantAnimation : MonoBehaviour
{
    [SerializeField] private Animator animBase;
    [SerializeField] private Animator animFace;
    private float blinkTime;
    public bool isPanic;
    // Start is called before the first frame update
    void Start()
    {
        blinkTime = Random.Range(0.5f, 4f);
        isPanic = false;
    }

    // Update is called once per frame
    void Update()
    {
        animBase.SetBool("isNeedy", isPanic);
        animFace.SetBool("isNeedy", isPanic);
        if (Time.time > blinkTime)
        {
            blinkTime = Time.time + Random.Range(2f, 9f);
            animFace.SetTrigger("isBlinking");
        }
    }

    // call ChangeRunning when the Player is either starting to move or when it stops moving
    public void ChangePanic(bool panics)
    {
        isPanic = !isPanic;
        animBase.SetBool("isNeedy", isPanic);
        animFace.SetBool("isNeedy", isPanic);

    }
}
