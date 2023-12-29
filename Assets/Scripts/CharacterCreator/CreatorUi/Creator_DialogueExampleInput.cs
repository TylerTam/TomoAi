using System.Collections.Generic;
using UnityEngine;

public class Creator_DialogueExampleInput : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField textInput;
    [SerializeField] private Creator_SelectedCharacterAttribs attribsEditor;
    public string GetDialogue => textInput.text;
    [SerializeField] private List<ContentSizeFitterEdit> contentFitters;

    public void InitDialogueExample(string dialogueExample, Creator_SelectedCharacterAttribs atr)
    {
        textInput.text = dialogueExample;
        attribsEditor = atr;
    }

    public void DialogueChanged()
    {
        if (attribsEditor) attribsEditor.MarkDirty();
    }
    
    public void RemoveDialogue()
    {
        attribsEditor.RemoveDialogue(transform.GetSiblingIndex());
    }
    public void ToggleFitters (bool enable)
    {
        foreach(ContentSizeFitterEdit fitter in contentFitters)
        {
            fitter.enabled = enable;
        }
    }
}
