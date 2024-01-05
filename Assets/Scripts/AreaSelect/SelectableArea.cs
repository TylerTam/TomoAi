using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableArea : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform camPos;
    [SerializeField] private SceneLoader.SceneNames areaName;
    public void EndTap()
    {
        PlayerController.Instance.ToggleInteractionInput(true);
        CameraManager.Instance.SwitchCam(CameraManager.CamType.FarCam);
    }

    public void HoverToggle(bool hoverOn)
    {
        
    }

    public void TapHold()
    {

        
    }

    public void Tapped()
    {
        PlayerController.Instance.ToggleInteractionInput(false);
        CameraManager.Instance.SetDialogueCam(camPos);
        AreaSelectManager.Instance.AreaPressed(areaName);
    }

    public void TapReleased()
    {
        throw new System.NotImplementedException();
    }

    
}
