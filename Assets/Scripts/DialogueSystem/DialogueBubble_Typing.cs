using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBubble_Typing : DialogueBubble
{
    private float timer;
    public float DotsPerSecond;
    public UnityEngine.UI.Image Dots;


    private void OnEnable()
    {
        timer = 0;
        Dots.fillAmount = 0;
    }
    public override void DialogueInit(CharacterData characterData, string spokenText)
    {
        base.DialogueInit(characterData, spokenText);
        timer = 0;
    }

    private void Update()
    {


        timer += Time.deltaTime;
        Dots.fillAmount = ((Mathf.Round((timer / DotsPerSecond) * 10 ))  % 4)/3;
        if(Dots.fillAmount == 0 && timer/DotsPerSecond >0.5f)
        {
            timer = 0;
        }
    }
}
