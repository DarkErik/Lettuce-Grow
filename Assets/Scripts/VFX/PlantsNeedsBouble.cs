using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantsNeedsBouble : MonoBehaviour
{
    [SerializeField] private EnumDataMapping<Sprite, PlantNeed> needSpriteMapping;

    [SerializeField] private SpriteRenderer render;

    public void Init(PlantNeed need) {
        render.sprite = needSpriteMapping[need];
    }
}

public enum PlantNeed {
    WATER
}