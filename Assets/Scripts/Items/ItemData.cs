using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        Clothes,
        Furniture,
    }
    public string ItemId;
    public string ItemDisplayName;
}
