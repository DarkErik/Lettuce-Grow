using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantsNeedsBouble : MonoBehaviour
{
    [SerializeField] private EnumDataMapping<Sprite, PlantNeed> needSpriteMapping;

    [SerializeField] private SpriteRenderer render;
    [SerializeField] private Animator anim;

    public void Init(PlantNeed need) {
        render.sprite = needSpriteMapping[need];
    }

    public void Close() {
        anim.SetTrigger("close");
    }

    /// <summary>
    /// THIS IS ONLY FOR FUNCTION EVENTS, CALL CLOSE INSTEAD!!!
    /// </summary>
    public void KillThis() {
        Destroy(this.gameObject);
    }

    /* testing
    public void Awake() {
        StartCoroutine(testc());
    }
    private IEnumerator testc() {
        Init(PlantNeed.MUSIC);
        yield return new WaitForSeconds(5);
        Close();
    }*/
}

public enum PlantNeed {
    WATER,
    MUSIC,
    BUGS
}