using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class GeneratorCleaner
{

    private bool clearAtLastPeriod = true;
    public string CleanSentence(string sentence)
    {
        string resString = sentence;
        //Debug.Log("Cleaning string: " + resString);
        int indexOfEnd = resString.IndexOf("You:");
        if (indexOfEnd >= 0)
        {
            resString = resString.Substring(0, indexOfEnd);
        }
        try
        {
            resString = resString.Replace("<USER>", GameManager.Instance.PlayerCharacterData.Name);
        }catch
        {
            resString = resString.Replace("<USER>", "Test User");
        }

        if (clearAtLastPeriod)
        {
            resString = FindLastSentenceEnd(resString);
        }

        

        //Debug.Log("Cleaned up string: " + resString);
        return resString;
    }

    public string AdjustScentenceForRichText(string sentence)
    {
        return ReplaceAsterisksWithItalics(sentence);
    }
    private string FindLastSentenceEnd(string editString)
    {

        
        string resString = editString;
        int sentenceEnd = resString.LastIndexOf(".");
        int sentenceEnd2 = resString.LastIndexOf("?");
        if (sentenceEnd2 > sentenceEnd) sentenceEnd = sentenceEnd2;
        sentenceEnd2 = resString.LastIndexOf("!");
        if (sentenceEnd2 > sentenceEnd) sentenceEnd = sentenceEnd2;
        if(sentenceEnd == -1 && sentenceEnd2 == -1)
        {
            return resString;
        }

        sentenceEnd2 = resString.LastIndexOf("*");
        if (sentenceEnd2 > sentenceEnd) sentenceEnd = sentenceEnd2;

        resString = resString.Substring(0, sentenceEnd + 1);

        resString = resString.Trim();
        return resString;
    }

    public string RemoveAsteriskActions(string editString)
    {

        string returnString = editString;
        
        List<int> asteriskIndexes = new List<int>();
        for (int i = 0;; i+= 1)
        {
            i = editString.IndexOf("*",i);
            if (i == -1)
                break;
            asteriskIndexes.Add(i);
        }

        asteriskIndexes.Reverse();
        if(asteriskIndexes.Count > 1)
        {
            bool hasAsterisk = true;
            while (hasAsterisk)
            {
                int secondIndex = asteriskIndexes[0];
                asteriskIndexes.RemoveAt(0);
                if (asteriskIndexes.Count <=0) break;
                int firstIndex = asteriskIndexes[0];
                asteriskIndexes.RemoveAt(0);
                returnString = returnString.Remove(firstIndex, secondIndex - firstIndex+1);
                if(asteriskIndexes.Count <=1) hasAsterisk = false;
            }
        }
        return returnString;
    }

    public string ReplaceAsterisksWithItalics(string editString)
    {
        string returnString = editString;
        List<int> asteriskIndexes = new List<int>();
        for (int i = 0; ; i += 1)
        {
            i = editString.IndexOf("*", i);
            if (i == -1)
                break;
            asteriskIndexes.Add(i);
        }

        asteriskIndexes.Reverse();
        if (asteriskIndexes.Count > 1)
        {
            bool hasAsterisk = true;
            while (hasAsterisk)
            {
                int secondIndex = asteriskIndexes[0];
                asteriskIndexes.RemoveAt(0);
                if (asteriskIndexes.Count <= 0) break;
                int firstIndex = asteriskIndexes[0];
                asteriskIndexes.RemoveAt(0);
                //returnString = returnString.Remove(firstIndex, secondIndex - firstIndex + 1);
                returnString = returnString.Remove(secondIndex, 1).Insert(secondIndex, "</i>");
                returnString = returnString.Remove(firstIndex, 1).Insert(firstIndex, "<i>");
                if (asteriskIndexes.Count <= 1) hasAsterisk = false;
            }
        }
        return returnString;
    }
}
