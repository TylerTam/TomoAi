using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator_TomoCharOutfit : TomoCharOutfit
{
    CharacterData charData;
    private void Awake()
    {
        charData = new CharacterData("", "", 0);
    }

    public void ChangeFavouriteColor(Color newCol)
    {
        Debug.Log(outfitMesh.sharedMaterials.Length);
        shirtBlock = new MaterialPropertyBlock();
        slevesBlock = new MaterialPropertyBlock();
        pantsBlock = new MaterialPropertyBlock();
        outfitMesh.GetPropertyBlock(shirtBlock, shirtMatIndex);
        outfitMesh.GetPropertyBlock(slevesBlock, slevesMatIndex);
        outfitMesh.GetPropertyBlock(pantsBlock, pantsMatIndex);

        
        charData.FavouriteColor = newCol;


        outfitData.AdjustMaterialBlocks(charData, ref shirtBlock, ref slevesBlock, ref pantsBlock);

        outfitMesh.SetPropertyBlock(shirtBlock, shirtMatIndex);
        outfitMesh.SetPropertyBlock(slevesBlock, slevesMatIndex);
        outfitMesh.SetPropertyBlock(pantsBlock, pantsMatIndex);

    }
}
