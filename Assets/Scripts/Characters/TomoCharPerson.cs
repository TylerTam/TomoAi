using UnityEngine;

public class TomoCharPerson : MonoBehaviour
{


    [SerializeField] private TomoCharUi tomoCharUi;
    [SerializeField] private TomoCharAppearenceConstructor tomoCharAppearenceConstructor;
    [SerializeField] private CharacterData personData;
    [SerializeField] private float speakFirstChance;
    [SerializeField] private TMPro.TextMeshProUGUI charNameTemp;
    [SerializeField] private TomoCharEmotions tomoCharEmotions;

    public CharacterData CharData => personData;
    public EmotionAnalysis.Emotion CurrentEmotion => tomoCharEmotions.MainEmotion;
    public void ConstructTomoChar(CharacterData characterData)
    {
        personData = characterData;
        tomoCharAppearenceConstructor.ConstructCharacter(characterData);
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
    public void WorldDialoguePopUp(string dialogue)
    {
        tomoCharUi.SetNextSpeechText(CharData, ServerLink.sentenceCleaner.AdjustScentenceForRichText(dialogue));
        tomoCharUi.DisplayUi(TomoCharUi.UiState.SpeechBubble, null);
    }
    public void WorldDialoguePopUp(string dialogue ,System.Action dialogueFinished)
    {
        tomoCharUi.SetNextSpeechText(CharData, ServerLink.sentenceCleaner.AdjustScentenceForRichText(dialogue));
        tomoCharUi.DisplayUi(TomoCharUi.UiState.SpeechBubble, null, dialogueFinished);
    }

    public void UpdateEmotions(EmotionAnalysis emotions)
    {
//        Debug.Log(emotions.ToString());
        if (tomoCharEmotions) tomoCharEmotions.AddEmotion(emotions);
        //Debug.Log("To Do: Do something with the emotional analysis data");
    }
    public void ReactionEmotion(EmotionAnalysis emotions)
    {
        if (tomoCharEmotions) tomoCharEmotions.CalculateReaction(emotions);
    }
}
