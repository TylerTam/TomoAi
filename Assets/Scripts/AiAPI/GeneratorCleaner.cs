using System.Security.Cryptography;
using UnityEngine;

public class GeneratorCleaner
{

    private bool clearAtLastPeriod = true;
    public string CleanSentence(string sentence)
    {
        string resString = sentence;
        Debug.Log("Cleaning string: " + resString);
        int indexOfEnd = resString.IndexOf("You:");
        if (indexOfEnd >= 0)
        {
            resString = resString.Substring(0, indexOfEnd);
        }
        resString = resString.Replace("<USER>", DialogueSystem_Main.Instance.PlayerData.Name);

        if (clearAtLastPeriod)
        {
            resString = FindLastSentenceEnd(resString);
        }

        Debug.Log("Cleaned up string: " + resString);
        return resString;
    }

    private string FindLastSentenceEnd(string editString)
    {
        string resString = editString;
        int sentenceEnd = resString.LastIndexOf(".");
        int sentenceEnd2 = resString.LastIndexOf("?");
        if (sentenceEnd2 > sentenceEnd) sentenceEnd = sentenceEnd2;
        sentenceEnd2 = resString.LastIndexOf("!");
        if (sentenceEnd2 > sentenceEnd) sentenceEnd = sentenceEnd2;
        sentenceEnd2 = resString.LastIndexOf("*");
        if (sentenceEnd2 > sentenceEnd) sentenceEnd = sentenceEnd2;

        resString = resString.Substring(0, sentenceEnd + 1);
        resString = resString.Trim();
        return resString;
    }
}
