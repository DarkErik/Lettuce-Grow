using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameFrame : MonoBehaviour
{
    public bool WasMinigameFinished { get; private set; } = false;

    [SerializeField] private EnumDataMapping<GameObject, PlantNeed> typeToMinigamePrefabMapping;
    [SerializeField] private Vector2 targetScale;
    [SerializeField] private Transform minigameBase;
    [SerializeField] private SpriteRenderer bgImage;
    [SerializeField] private Animator anim;


    private float closeAnimTime = -1;
    private void Awake() {
        closeAnimTime = Util.GetAnimationClipLength(anim, "Close");
    }

    public void Init(PlantNeed minigameType) {
        GameObject minigameObj = typeToMinigamePrefabMapping[minigameType];
        minigameObj = Instantiate(minigameObj, minigameBase);
        
        GenericMinigame game = minigameObj.GetComponent<GenericMinigame>();
        game.Init(this);
        PlayerController.Instance.SetCanMove(false);
    }

    public void Close() {
        anim.SetTrigger("close");
        StartCoroutine(_Close(false));
    }

    public void ForceClose() {
        anim.SetTrigger("close");
        StartCoroutine(_Close(true));
    }

    private IEnumerator _Close(bool destroy) {
        yield return new WaitForSeconds(closeAnimTime);
        PlayerController.Instance.SetCanMove(true);

        WasMinigameFinished = true;

        if (destroy) Destroy(this.gameObject);
    }

    public Rect GetBounds() {

        Vector3 size = bgImage.transform.localScale * targetScale;
        Vector2 center = transform.position - (size * 0.5f);
        //Debug.Log(new Rect(center, size));
        return new Rect(center, size);
    }

    public void SetBGImage(Sprite backgroundImage)
    {
        bgImage.sprite = backgroundImage;
    }

    public void SetBGColor(Color c) {
        bgImage.color = c;
    }
}
