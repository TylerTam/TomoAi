using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

/// <summary>
/// The script that assigns the NPCs to their respective areas during runtime;
/// </summary>
public class NpcPlacements : TickUpdate
{
    
    [SerializeField] private NpcAreaBaseWeights baseAreaWeights;
    public enum AreaType
    {
        Apartments = 0,
        Restaurant = 1,
    }

    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<AreaType, AreaData> allAreaData;
#if UNITY_EDITOR
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<int, NpcAreaData> npcAreaData;
#else
    private Dictionary<int, NpcAreaData> npcAreaData = new Dictionary<int, NpcAreaData>();
#endif
    [System.Serializable]
    public class AreaData
    {
        [SerializeField] private int MaxOccupancy;
        [SerializeField] private int CurrentOccupancy;

        public bool CanAddOccupant => (CurrentOccupancy + 1 <= MaxOccupancy);
        public void AddOccupant()
        {
            CurrentOccupancy += 1;
        }
        public void Clear()
        {
            CurrentOccupancy = 0;
        }
    }

    public List<NpcAreaData> GetAllCharactersInArea(AreaType area)
    {
        List<NpcAreaData> returnIds = new List<NpcAreaData>();
        foreach(KeyValuePair<int, NpcAreaData> key in npcAreaData)
        {
            if (key.Value.area == area) returnIds.Add(key.Value);
        }

        return returnIds;
    }
    public override void UpdateFromTick()
    {
        npcAreaData.Clear();
        foreach (KeyValuePair<AreaType, AreaData> area in allAreaData)
        {
            area.Value.Clear();
        }

        List<CharacterData> allChars = new List<CharacterData> (GameManager.Instance.SaveLoader.LoadedData.SavedCharacters);
        CharacterData tempData;
        for (int i = 0; i < allChars.Count; i++)
        {
            int rndIndex = Random.Range(0, allChars.Count);
            tempData = allChars[i];
            allChars[i] = allChars[rndIndex];
            allChars[rndIndex] = tempData;
        }

        for (int i = 0; i < allChars.Count; i++)
        {
            AreaType area = GetRandomArea();
            NpcAreaData areaData = new NpcAreaData();
            switch (area)
            {
                default:
                case AreaType.Apartments:

                    areaData.ApartmentOwnerId = allChars[i].LocalId;
                    break;
                case AreaType.Restaurant:
                    break;
            }
            areaData.npcId = allChars[i].LocalId;
            areaData.area = area;
            npcAreaData.Add(allChars[i].LocalId, areaData);
            allAreaData[area].AddOccupant();
        }

    }
    private AreaType GetRandomArea()
    {
        List<float> areaWeights = new List<float>();
        areaWeights.Add(0);
        int areaCount = System.Enum.GetNames(typeof(AreaType)).Length;
        float totalPercent = 0;
        for (int i = 1; i < areaCount; i++)
        {
            float percent = GetAreaWeight((AreaType)i);
            totalPercent += percent;
            areaWeights.Add(percent);
        }
        areaWeights[0] = (1f - totalPercent);

        float randomPlace = Random.Range(0, 1f);
        totalPercent = 0;
        for (int i = 0; i < areaWeights.Count; i++)
        {
            totalPercent += areaWeights[i];
            if (randomPlace < totalPercent) return (AreaType)i;
        }

        Debug.Log("Couldn't find area percent, Error!");
        return AreaType.Apartments;

    }
    private float GetAreaWeight(AreaType area)
    {
        if (allAreaData[area].CanAddOccupant == false) return 0;
        return baseAreaWeights.GetCurrentWeight(area);

    }

    public NpcAreaData GetNpcArea(int npcId)
    {
        if (npcAreaData.ContainsKey(npcId))
        {
            return npcAreaData[npcId];
        }
        else
        {
            NpcAreaData newArea = new NpcAreaData();
            newArea.npcId = npcId;
            newArea.area = AreaType.Apartments;
            return newArea;
        }
    }


    public void ForceNpcIntoArea(int npcId, AreaType area, int areaOwnerId)
    {
        NpcAreaData areaData = new NpcAreaData();
        switch (area)
        {
            case AreaType.Apartments:
                areaData.npcId = npcId;
                areaData.area = AreaType.Apartments;
                areaData.ApartmentOwnerId = areaOwnerId;
                npcAreaData[npcId] = areaData;
                break;
            case AreaType.Restaurant:
                areaData.npcId = npcId;
                areaData.area = area;
                npcAreaData[npcId] = areaData;
                break;
        }
    }
}

[System.Serializable]
public class NpcAreaData
{
    [SerializeField] public int npcId;
    [SerializeField] public NpcPlacements.AreaType area;
    [SerializeField] public int ApartmentOwnerId;
}

