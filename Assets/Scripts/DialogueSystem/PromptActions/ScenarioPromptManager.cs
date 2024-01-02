using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioPromptManager : MonoBehaviour
{
    private ScenarioPrompt_SO currentScenarioPrompt;

    public ScenarioPrompt_SO ScenarioPrompt => currentScenarioPrompt;

    [SerializeField] private ScenarioPrompt_SO defScenarioPrompt;
    [SerializeField] private List<ScenarioPrompt_SO> relevantScenarioPrompts;

    public void SetScenarioPrompt(ScenarioPrompt_SO scenarioPrompt, List<string> relevantCharacters)
    {
        if (scenarioPrompt.CanUsePrompt(relevantCharacters.Count))
        {
            currentScenarioPrompt = scenarioPrompt;
        }
        else
        {
            currentScenarioPrompt = defScenarioPrompt;
        }
    }
    public void LoadPromptByIndex(int index, int characterCount)
    {
        if(relevantScenarioPrompts.Count <= index || !relevantScenarioPrompts[index].CanUsePrompt(characterCount))
        {
            currentScenarioPrompt = defScenarioPrompt;
        }
        else
        { 
            currentScenarioPrompt = relevantScenarioPrompts[index];
        }
    }
    public string GetPrompt(List<string> characterNames)
    {
        return currentScenarioPrompt.GetRandomPrompt(characterNames);
    }
}
