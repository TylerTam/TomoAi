using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScenarioPrompt_", menuName = "Data/Dialogue/PromptScenario", order = 0)]
public class ScenarioPrompt_SO : ScriptableObject
{

#if UNITY_EDITOR
    [SerializeField][TextArea] private string scenarioDescription_EditorRef;
#endif

    [SerializeField][TextArea] private List<string> prompts;
    [SerializeField] private List<string> promptCharTags;

    [SerializeField] private int minCharactersCount;
    public bool CanUsePrompt(int charCount)
    {
        return minCharactersCount <= charCount;
    }

    public string GetRandomPrompt(List<string> relevantCharacters)
    {
        if (prompts == null || prompts.Count == 0) return "";
        string prompt = prompts[Random.Range(0, prompts.Count)];
        for (int i = 0; i < promptCharTags.Count; i++)
        {
            prompt = prompt.Replace(promptCharTags[i], relevantCharacters[i]);
        }
        return prompt;
    }
}
