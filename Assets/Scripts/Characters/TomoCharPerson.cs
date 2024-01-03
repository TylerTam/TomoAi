using UnityEngine;

public class TomoCharPerson : MonoBehaviour
{


    [SerializeField] private TomoCharUi tomoCharUi;
    [SerializeField] private TomoCharAppearenceConstructor tomoCharAppearenceConstructor;
    [SerializeField] private CharacterData personData;
    [SerializeField] private float speakFirstChance;
    [SerializeField] private TMPro.TextMeshProUGUI charNameTemp;

    public CharacterData CharData => personData;
    public void ConstructTomoChar(CharacterData characterData)
    {
        personData = characterData;
        tomoCharAppearenceConstructor.ConstructCharacter(characterData.CharacterAppearence);
        charNameTemp.text = characterData.Name;
        tomoCharUi.SetBubbleColors(characterData.FavouriteColor);
    }

    public void StartConversation(AllPromptActions.ActionType promptAction)
    {

        //Can change this so that they start speaking, if they have a '...' over their head, like in tomodachi life when they want to talk to you.

        bool speakFirst = false;
        string promptActionAddendum = "";
        if(Random.Range(0f,1f) <= speakFirstChance)
        {
            speakFirst = true;
            tomoCharUi.DisplayUi(TomoCharUi.UiState.Thinking);
            promptActionAddendum = GenerateStartingActionPrompt(promptAction);
        }
        DialogueSystem_Main.Instance.StartConversationWithPlayer(personData, this, speakFirst, promptActionAddendum, "");

    }

    private string GenerateStartingActionPrompt(AllPromptActions.ActionType promptAction)
    {
        return DialogueSystem_Main.Instance.GetPromptAction(promptAction, personData.Name);  
    }

    public void ShowThinkingBubble()
    {
        tomoCharUi.DisplayUi(TomoCharUi.UiState.Thinking);
    }
    public void WorldDialoguePopUp(CharacterData charData, string dialogue)
    {
        tomoCharUi.SetNextSpeechText(charData, ServerLink.sentenceCleaner.AdjustScentenceForRichText(dialogue));
        tomoCharUi.DisplayUi(TomoCharUi.UiState.SpeechBubble);
    }
}
