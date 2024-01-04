using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Tapped();
    public void TapHold();
    public void TapReleased();
    public void EndTap();
}
