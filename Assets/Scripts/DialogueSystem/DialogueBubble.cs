using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI dialogueText;
    public ContentSizeFitter sizeFitter;
    public virtual void DialogueInit(CharacterData characterData, string spokenText)
    {
        nameText.text = characterData.Name;
        dialogueText.text = spokenText;
    }

}
