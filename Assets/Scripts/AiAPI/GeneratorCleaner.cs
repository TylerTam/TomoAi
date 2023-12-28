using System.Security.Cryptography;
using UnityEngine;

public class GeneratorCleaner
{
    public string CleanSentence(string sentence)
    {
        string resString = sentence;
        int indexOfEnd = resString.IndexOf("You:");
        if (indexOfEnd >= 0)
        {
            resString = resString.Substring(0, indexOfEnd);
        }
        resString = resString.Replace("<USER>", DialogueSystem_Main.Instance.PlayerData.Name);
        return resString;
    }
}
