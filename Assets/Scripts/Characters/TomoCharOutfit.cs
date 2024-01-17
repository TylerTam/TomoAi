using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharOutfit : MonoBehaviour
{
    [SerializeField] protected OutfitData outfitData;
    [SerializeField] protected SkinnedMeshRenderer outfitMesh;
    [SerializeField] protected int shirtMatIndex;
    [SerializeField] protected int slevesMatIndex;
    [SerializeField] protected int pantsMatIndex;

    protected MaterialPropertyBlock shirtBlock, slevesBlock, pantsBlock;

    private void Awake()
    {

    }
    public void InitOutfit(CharacterData charData)
    {
        Debug.Log(outfitMesh.sharedMaterials.Length);
        shirtBlock = new MaterialPropertyBlock();
        slevesBlock = new MaterialPropertyBlock();
        pantsBlock = new MaterialPropertyBlock();
        outfitMesh.GetPropertyBlock(shirtBlock, shirtMatIndex);
        outfitMesh.GetPropertyBlock(slevesBlock, slevesMatIndex);
        outfitMesh.GetPropertyBlock(pantsBlock, pantsMatIndex);

        outfitData.AdjustMaterialBlocks(charData, ref shirtBlock, ref slevesBlock, ref pantsBlock);

        outfitMesh.SetPropertyBlock(shirtBlock, shirtMatIndex);
        outfitMesh.SetPropertyBlock(slevesBlock, slevesMatIndex);
        outfitMesh.SetPropertyBlock(pantsBlock, pantsMatIndex);
        
    }
}
