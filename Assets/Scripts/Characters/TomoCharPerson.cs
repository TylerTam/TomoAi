using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharPerson : MonoBehaviour
{


    [SerializeField] private TomoCharUi tomoCharUi;
    [SerializeField] private TomoCharAppearenceConstructor tomoCharAppearenceConstructor;
    [SerializeField] private CharacterData personData;
    [SerializeField] private float speakFirstChance;

    public void ConstructTomoChar(CharacterData characterData)
    {
        personData = characterData;
        tomoCharAppearenceConstructor.ConstructCharacter(characterData.CharacterAppearence);
    }

    public void StartConversation()
    {

        //Can change this so that they start speaking, if they have a '...' over their head, like in tomodachi life when they want to talk to you.
        bool speakFirst = false;
        if(Random.Range(0f,1f) <= speakFirstChance)
        {
            speakFirst = true;
            tomoCharUi.DisplayUi(TomoCharUi.UiState.Thinking);
        }


        DialogueSystem_Main.Instance.StartConversation(personData, this, speakFirst, GenerateScenarioPrompt());
    }

    private string GenerateScenarioPrompt()
    {
        return "";
    }

    public void DialogueEnded()
    {
        GetComponent<TomoCharInteraction>().EndDialogue();
    }

    public void ShowThinkingBubble()
    {
        tomoCharUi.DisplayUi(TomoCharUi.UiState.Thinking);
    }
    public void WorldDialoguePopUp(CharacterData charData, string dialogue)
    {
        tomoCharUi.SetNextSpeechText(charData, dialogue);
        tomoCharUi.DisplayUi(TomoCharUi.UiState.SpeechBubble);
    }
}
