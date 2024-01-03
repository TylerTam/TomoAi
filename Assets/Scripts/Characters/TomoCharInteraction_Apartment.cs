using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharInteraction_Apartment : TomoCharInteraction
{
    private bool isSelected;
    public override void Tapped()
    {
        if (!isSelected)
        {
            isSelected = true;
            base.Tapped();
            InGameUIManager.Instance.OpenMenu(InGameUIManager.InGameUIType.WorldNpcMenu, person);
            return;
        }

    }

    public override void EndSelect()
    {
        isSelected = false;
        base.EndSelect();
    }
}
