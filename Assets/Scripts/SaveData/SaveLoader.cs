using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveLoader : MonoBehaviour
{
    public SaveData LoadedData;
    public UnityEvent SaveDataLoaded;
    public bool saveLoaded = false;
    public TestingCharacterData saveCharData;
    public string PATH
    {
        get { return System.IO.Path.Combine( Application.persistentDataPath, "Save.sav"); }
    }
    private void Awake()
    {
        LoadSaveData();
    }
    
    private void LoadSaveData()
    {
        if (System.IO.File.Exists(PATH))
        {
            string saveFileText = System.IO.File.ReadAllText(PATH);
            LoadedData = JsonUtility.FromJson<SaveData>(saveFileText);
        }
        else
        {
            LoadedData = new SaveData();
            SaveData();
            SaveDataLoaded?.Invoke();
            saveLoaded = true;
        }
    }
    
    public void SaveData()
    {
        System.IO.File.WriteAllText(PATH, JsonUtility.ToJson(LoadedData,true));
        Debug.Log("Save Loc: " + PATH);
    }

    public void UpdateCharacter(CharacterData characterData, bool isNewCharacter)
    {

        int index = -1;
        if (!isNewCharacter)
        {
            index = FindSavedCharacterIndex(characterData);
        }
        if(index == -1)
        {
            characterData.Creator = LoadedData.Player.PlayerName;
            characterData.Creator_Id = GenerateCharacterId();
            LoadedData.SavedCharacters.Add(characterData);
        }
        else
        {
            LoadedData.SavedCharacters[index] = characterData;
        }
        
    }

    private int FindSavedCharacterIndex (CharacterData data)
    {
        for(int i =0; i < LoadedData.SavedCharacters.Count; i++)
        {
            if (LoadedData.SavedCharacters[i].IsEqual(data))
            {
                return i;
            }
        }
        return -1;
    }
    private float GenerateCharacterId()
    {
        int id = 0;
        for (int i = 0; i < LoadedData.SavedCharacters.Count; i++)
        {
            if (LoadedData.SavedCharacters[i].Creator == LoadedData.Player.PlayerName)
            {
                id++;
            }
        }
        return id;
    }

    public void TestSvaeCharToData()
    {
        LoadedData.SavedCharacters.Add(saveCharData.characterData.Clone());

    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveLoader))]
public class SaveLoaderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if(GUILayout.Button("Save Data"))
        {
            (target as SaveLoader).SaveData();
        }
        if(GUILayout.Button("Add Test Char Data"))
        {
            (target as SaveLoader).TestSvaeCharToData();
        }
        base.OnInspectorGUI();
    }
}
#endif

[System.Serializable]
public class SaveData
{
    [SerializeField] public PlayerData Player;
    [SerializeField] public List<CharacterData> SavedCharacters;
}

[System.Serializable]
public class PlayerData
{
    [SerializeField] public string PlayerName;
}
