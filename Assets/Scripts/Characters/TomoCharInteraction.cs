using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharInteraction : MonoBehaviour, IInteractable
{
    protected TomoCharController controller;
    protected TomoCharPerson person;
    public System.Action CharacterTapped;
    public System.Action SelectEnded;

    [SerializeField] private Transform dialogueCamPos;
    private void Awake()
    {
        person = GetComponent<TomoCharPerson>();
        controller = GetComponent<TomoCharController>();
    }
    #region Interactable functions
    public virtual void Tapped()
    {
        CharacterTapped?.Invoke();
        person.StartConversation(AllPromptActions.ActionType.TapOnNPC);
        if (dialogueCamPos != null)
        {
            CameraManager.Instance.SetDialogueCam(dialogueCamPos);
        }
    }

    public virtual void TapHold()
    {
        controller.SwitchState(TomoCharController.ControllerState.Dragging);
    }

    public virtual void TapReleased()
    {
        controller.SwitchState(TomoCharController.ControllerState.Idle);
    }
    #endregion

    public virtual void EndTap()
    {
        SelectEnded?.Invoke();
        CameraManager.Instance.SetDialogueCam(null);
    }

    public void HoverToggle(bool hoverOn)
    {
        
    }
}
