using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenTransition : MonoBehaviour
{
    public static ScreenTransition Instance { get; private set; }
    private float fadeOutTime = 0.5f;
    [SerializeField] private Animator anim;

    private void Awake() {
        Instance = this;
        var clips = anim.GetCurrentAnimatorClipInfo(0);
        for (int i = 0; i < clips.Length; i++) {
            if (clips[i].clip.name == "FadeIn") {
                fadeOutTime = clips[i].clip.length;
                break;
            }
        }

    }

    public void LoadScene(string scene) {
        anim.SetTrigger("FadeOut");
        StartCoroutine(LoadSceneAsnc(scene));
    }

    public void LoadScene(int id) {
        anim.SetTrigger("FadeOut");
        StartCoroutine(LoadSceneAsnc(id));
    }

    private IEnumerator LoadSceneAsnc(int id) {
        yield return new WaitForSeconds(fadeOutTime);
        SceneManager.LoadScene(id);
    }

    private IEnumerator LoadSceneAsnc(string scene) {
        yield return new WaitForSeconds(fadeOutTime);
        SceneManager.LoadScene(scene);
    }
}
