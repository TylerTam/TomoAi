using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AppearenceItems : ScriptableObject
{
    public Dictionary<string, Sprite> AppearencePieces_Sprites;
    public GameObject AppearenceItemSprite_Prefab;
    public GameObject GetAppearenceItem(string itemId)
    {
        AppearenceItemSprite spr = ObjectPooler.NewObject(AppearenceItemSprite_Prefab, Vector3.zero, Quaternion.identity).GetComponent<AppearenceItemSprite>();
        spr.SetSprite(AppearencePieces_Sprites[itemId]);
        return spr.gameObject;

    }
}
