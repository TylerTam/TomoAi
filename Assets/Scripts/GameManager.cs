using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }
    private static GameManager _instance;
    public ItemDictionary AllItems;
    public SaveLoader SaveLoader;
    public RelationshipMatrix RelationshipMatrix;
    public CharacterData PlayerCharacterData;

    private void Start()
    {
        PlayerCharacterData = SaveLoader.LoadedData.Player.ToCharData();
    }

    public static int GetCurrentInGameHour()
    {
        return DateTime.Now.Hour;
    }
}
