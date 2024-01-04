using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RelationshipIncrements", menuName = "Data/Characters/RelationshipIncrements", order = 0)]
public class RelationshipIncrements : ScriptableObject
{
    public enum IncrementType
    {
        Idle = 0,       //When the characters aren't doing anything together
        Chatting = 1,   //When the characters are chatting with each other (ie. in the restaurant)
        Fighting = 2,   //When the characters have a fight
        HangingOut = 3,   //When the characters are directly hanging out
    }
    [SerializeField] private RotaryHeart.Lib.SerializableDictionary.SerializableDictionaryBase<IncrementType, RangeWeights> increments;
    [System.Serializable]
    public class IncrementRangeWeight
    {
        public Vector2Int incrementRange;
        [Range(0f,1f)]public float weight;
    }

    [System.Serializable]
    public struct RangeWeights
    {
        [SerializeField] public List<IncrementRangeWeight> rangeWeights;
    }
    public int GetIncrement(IncrementType incrementType)
    {
        List<IncrementRangeWeight> weights = increments[incrementType].rangeWeights;

        float rng = Random.Range(0, 1f);

        float currentWeight = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            currentWeight += weights[i].weight;
            if(rng < currentWeight)
            {
                return Random.Range(weights[i].incrementRange.x, weights[i].incrementRange.y);
            }
        }
        return Random.Range(weights[0].incrementRange.x, weights[0].incrementRange.y);
    }

}
