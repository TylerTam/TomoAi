using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator_TomoCharAppearenceConstructor : TomoCharAppearenceConstructor
{
    [SerializeField] private Creator_TomoCharOutfit tomoCharOutfit;
    public void ChangeFavoriteColor(Color newCol)
    {
        tomoCharOutfit.ChangeFavouriteColor(newCol);
    }
}
