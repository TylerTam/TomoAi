using UnityEngine;

public class Creator_DialogueExampleInput : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField textInput;
    
    public void InitDialogueExample(string dialogueExample)
    {
        textInput.text = dialogueExample;
    }
}
