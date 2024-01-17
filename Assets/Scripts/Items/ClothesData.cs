using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClothesData_", menuName = "Data/Items/Clothes", order = 0)]
public class ClothesData : ScriptableObject
{
    [SerializeField] private bool useFavoriteColor;
    [SerializeField] private Color baseClothesColor;

    public void AdjustMaterialPropBlock(CharacterData charData, ref MaterialPropertyBlock propBlock)
    {
        propBlock.SetColor("_BaseColor", useFavoriteColor ? charData.FavouriteColor : baseClothesColor);
    }
}
