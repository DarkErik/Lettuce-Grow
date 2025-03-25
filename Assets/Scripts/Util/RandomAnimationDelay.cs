using System.Collections;
using UnityEngine;

public class RandomAnimationDelay : MonoBehaviour
{
    [SerializeField] public Animator animator;
    [SerializeField] public float minDelay = 0f;
    [SerializeField] public float maxDelay = 0.75f;

    void Start()
    {
        StartCoroutine(PlayAnimationWithDelay());
    }

    IEnumerator PlayAnimationWithDelay()
    {
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        if (animator != null) animator.Play(0);
        else Debug.LogWarning("Animator not set.");
    }
}
