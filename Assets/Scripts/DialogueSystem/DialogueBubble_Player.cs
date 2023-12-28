using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBubble_Player : DialogueBubble
{
    public GameObject typingMenu;
    public GameObject speakingMenu;

    private bool speakingEnabled;

    public override void DialogueInit(CharacterData characterData, string spokenText)
    {
        base.DialogueInit(characterData, spokenText);
        speakingEnabled = false;
    }
    public void SpeakingMenuPressed()
    {
        speakingEnabled = true;
        typingMenu.SetActive(false);
    }


}
