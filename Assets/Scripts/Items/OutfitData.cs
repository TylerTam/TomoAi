using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "OutfitData_", menuName = "Data/Items/Outfit", order = 0)]
public class OutfitData : ScriptableObject
{
    public ClothesData shirt;
    public ClothesData sleves;
    public ClothesData pants;

    public void AdjustMaterialBlocks(CharacterData chardata, ref MaterialPropertyBlock shirtBlock, ref MaterialPropertyBlock slevesBlock, ref MaterialPropertyBlock pantsBlock)
    {
        shirt.AdjustMaterialPropBlock   (chardata,ref shirtBlock);
        sleves.AdjustMaterialPropBlock  (chardata,ref slevesBlock);
        pants.AdjustMaterialPropBlock   (chardata,ref pantsBlock);
    }                                            
                                         
}
