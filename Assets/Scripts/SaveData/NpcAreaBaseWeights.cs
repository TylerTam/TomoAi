using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(fileName = "NpcAreaWeights_", menuName = "Data/NpcAreaWeights", order = 0)]
public class NpcAreaBaseWeights : ScriptableObject
{
    [SerializeField] private List<SerializableDictionaryBase<NpcPlacements.AreaType, float>> weightByTime;

    public float GetCurrentWeight(NpcPlacements.AreaType area)
    {
        return weightByTime[GameManager.GetCurrentInGameHour()][area];
    }
}
