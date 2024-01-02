using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharInteraction : MonoBehaviour, IInteractable
{
    private TomoCharController controller;
    private TomoCharPerson person;
    public System.Action CharacterTapped;
    public System.Action DialogueEnded;

    [SerializeField] private Transform dialogueCamPos;
    private void Awake()
    {
        person = GetComponent<TomoCharPerson>();
    }
    #region Interactable functions
    public void Tapped()
    {
        CharacterTapped?.Invoke();
        person.StartConversation(AllPromptActions.ActionType.TapOnNPC);
        if (dialogueCamPos != null)
        {
            CameraManager.Instance.SetDialogueCam(dialogueCamPos);
        }
    }

    public void TapHold()
    {
        throw new System.NotImplementedException();
    }

    public void TapReleased()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    public void EndDialogue()
    {
        DialogueEnded?.Invoke();
        CameraManager.Instance.SetDialogueCam(null);
    }
}
