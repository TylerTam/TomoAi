using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI nameText;
    [SerializeField] private TMPro.TextMeshProUGUI dialogueText;
    [SerializeField] private Image bubbleImage;
    public virtual void DialogueInit(CharacterData characterData, string spokenText)
    {
        nameText.text = characterData.Name;
        dialogueText.text = ServerLink.sentenceCleaner.AdjustScentenceForRichText( spokenText);
    }
    public virtual void SetBubbleColor(Color col)
    {
        bubbleImage.color = col;
    }

}
