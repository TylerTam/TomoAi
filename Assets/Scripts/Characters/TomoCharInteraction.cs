using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharInteraction : MonoBehaviour, IInteractable
{
    private TomoCharController controller;
    private TomoCharPerson person;
    public System.Action CharacterTapped;
    public System.Action DialogueEnded;
    private void Awake()
    {
        person = GetComponent<TomoCharPerson>();
    }
    #region Interactable functions
    public void Tapped()
    {
        CharacterTapped?.Invoke();
        person.StartConversation();
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
    }
}
