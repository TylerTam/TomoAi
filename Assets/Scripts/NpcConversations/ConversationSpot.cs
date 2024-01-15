using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ConversationSpot : MonoBehaviour, IInteractable
{
    [SerializeField] private List<TomoCharPerson> npcsHere = new List<TomoCharPerson>();

    [SerializeField] private List<Transform> npcSpots;
    private List<Transform> npcAvailableSpots;

    [SerializeField] private List<GameObject> editorRefPositions;
    [SerializeField] private ScenarioPrompt_SO scenarioPrompt;
    [SerializeField] private bool isSelected;

    [SerializeField] private List<CharacterDialogueBacklog> dialogueBacklog = new List<CharacterDialogueBacklog>();

    private TomoCharPerson speakingPerson;
    private int maxBacklogSize = 3;
    private bool isGenerating = false;
    private bool wasReset;
    [System.Serializable]
    private struct CharacterDialogueBacklog
    {
        public CharacterDialogueBacklog(int charId, string charText,EmotionAnalysis ea)
        {
            characterId = charId;
            characterText = charText;
            emotAnalysis = ea;
        }
        public int characterId;
        public string characterText;
        public EmotionAnalysis emotAnalysis;
    }

    private TomoCharPerson GetChar(int id) => npcsHere.Find(x => x.CharData.LocalId == id);


    private void Awake()
    {
        for (int i = editorRefPositions.Count-1; i >= 0; i--)
        {
            Destroy(editorRefPositions[i]);
        }
        editorRefPositions.Clear();
    }

    public void ResetSpots()
    {
        npcAvailableSpots = new List<Transform>(npcSpots);
        for (int i = npcsHere.Count -1; i >=0; i--)
        {
            ObjectPooler.ReturnToPool(npcsHere[i].gameObject);
        }
        npcsHere.Clear();
        dialogueBacklog.Clear();
        speakingPerson = null;
        wasReset = true;
    }
    public bool CanAddPerson(TomoCharPerson npcPerson)
    {
        if (npcSpots == null || npcSpots.Count == 0) return false;
        int randomTransform = Random.Range(0, npcSpots.Count);
        npcsHere.Add(npcPerson);
        npcPerson.transform.position = npcSpots[randomTransform].position;
        npcPerson.transform.rotation = npcSpots[randomTransform].rotation;
        
        npcSpots.RemoveAt(randomTransform);
        return true;
    }
    public void Tapped()
    {
        wasReset = false;
        isSelected = true;
        DialogueSystem_Main.Instance.scenarioPromptManager.SetScenarioPrompt(scenarioPrompt, npcsHere.Count);
        DialogueSystem_Main.Instance.StartConversationWithNpcsOnly(npcsHere, RecievedDialogue);

    }

    public void TapHold()
    {
        throw new System.NotImplementedException();
    }

    public void TapReleased()
    {
        throw new System.NotImplementedException();
    }

    public void EndTap()
    {
        isSelected = false;
    }


    /// <summary>
    /// Called when the current dialogue has been generated, to start generating the next one;
    /// </summary>
    /// <param name="dialogue"></param>
    private void StartNextGeneration()
    {
        if (isSelected)
        {
            isGenerating = true;
            DialogueSystem_Main.Instance.ContinueWithAiGenerate(RecievedDialogue);
            
        }
    }
    private void RecievedDialogue(bool success, int characterId, string text, EmotionAnalysis emotionalAnalysis)
    {
        if (wasReset) return;
        isGenerating = false;
        DialogueSystem_Main.Instance.AddDialogue(characterId, false, text);
        if (dialogueBacklog.Count == 0 && speakingPerson == null)
        {
            TomoCharPerson person = GetChar(characterId);
            person.WorldDialoguePopUp(text, CurrentDialogueDone);
            person.UpdateEmotions(emotionalAnalysis);
            speakingPerson = person;
            StartNextGeneration();
        }
        else
        {
            dialogueBacklog .Add(new CharacterDialogueBacklog(characterId, text,emotionalAnalysis));
            if (dialogueBacklog.Count < maxBacklogSize)
            {
                StartNextGeneration();
            }
            
        }
    }
    private void CurrentDialogueDone()
    {
        if (wasReset) return;
        if (dialogueBacklog.Count == 0)
        {
            speakingPerson = null;
            //Show thinking dialogue;
            if(isGenerating) GetChar(DialogueSystem_Main.Instance.GetCurrentThinkingCharacterId()).ShowThinkingBubble();
        }
        else
        {
            if(dialogueBacklog.Count == maxBacklogSize && !isGenerating)
            {
                StartNextGeneration();
            }
            CharacterDialogueBacklog backlog = dialogueBacklog[0];
            TomoCharPerson person = GetChar(backlog.characterId);
            speakingPerson = person;
            person.WorldDialoguePopUp(backlog.characterText, CurrentDialogueDone);
            person.UpdateEmotions(backlog.emotAnalysis);
            dialogueBacklog.RemoveAt(0);

        }
    }

    public void HoverToggle(bool hoverOn)
    {
        
    }
}
