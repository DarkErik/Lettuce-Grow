using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator faceAnimator;
    [SerializeField] private Animator torsoAnimator;
    [SerializeField] private Animator playerAnimator;
    private float blinkTime;
    private float tapFootTime;
    private float hatTime;
    public bool isRunning;
    // Start is called before the first frame update
    void Start()
    {
        blinkTime = Random.Range(0.5f, 4f);
        hatTime = Random.Range(3f, 7f);
        tapFootTime = Random.Range(4f, 8f);
        isRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > blinkTime)
        {
            blinkTime = Time.time + Random.Range(0.5f, 6f);
            faceAnimator.SetTrigger("isBlinking");
        }

        if (!isRunning)
        {
            if (Time.time > tapFootTime)
            {
                tapFootTime = Time.time + Random.Range(4f, 12f);
                torsoAnimator.SetTrigger("isTapping");
            }
            if (Time.time > hatTime)
            {
                hatTime = Time.time + Random.Range(7f, 15f);
                faceAnimator.SetTrigger("isHat");
            }
        }
    }

    // call ChangeRunning when the Player is either starting to move or when it stops moving
    public void ChangeRunning(bool runs)
    {
        if (!runs && isRunning)
        {
            hatTime = Time.time + Random.Range(3f, 7f);
            tapFootTime = Time.time + Random.Range(4f, 8f);
        }

        isRunning = runs;
        playerAnimator.SetBool("isRunning", isRunning);

    }
}
