using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AllItems", menuName = "Data/Items/AllItems", order = 0)]
public class AllItems : ScriptableObject
{
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<string, FurnitureData> allFurniture;

#if UNITY_EDITOR
    [Header("EDITOR ONLY")]
    [SerializeField] private List<FurnitureData> allFurnintureData;

    public void SetupFurnitureList()
    {
        allFurniture.Clear();
        foreach(FurnitureData furn in allFurnintureData)
        {
            allFurniture.Add(furn.ItemId, furn);
        }
        EditorUtility.SetDirty(this);
    }
#endif

    public ItemData GetItem(string itemName, ItemData.ItemType itemType)
    {
        switch (itemType)
        {
            case ItemData.ItemType.Clothes:
                return null;
                
            case ItemData.ItemType.Furniture:
                return allFurniture[itemName];
                
            default:
                return null;
        }
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AllItems))]
public class AllItemsInspector: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Setup Furniture List"))
        {
            (target as AllItems).SetupFurnitureList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
