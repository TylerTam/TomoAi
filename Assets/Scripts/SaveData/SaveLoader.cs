using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting.FullSerializer;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveLoader : MonoBehaviour
{
    public SaveData LoadedData;
    public Settings LoadedSettings;
    public UnityEvent SaveDataLoaded;
    public bool saveLoaded = false;
    public TestingCharacterData saveCharData;
#if UNITY_EDITOR
    [SerializeField] private bool CanSaveData = true;
#endif
    public string PATH
    {
        get { return System.IO.Path.Combine(Application.persistentDataPath, "Save.sav"); }
    }
    public string SETTINGSPATH
    {
        get { return System.IO.Path.Combine(Application.persistentDataPath, "Settings.sav"); }
    }
    private void Awake()
    {
        LoadSaveData();
        LoadSettings();
    }

    private void LoadSaveData()
    {
        if (System.IO.File.Exists(PATH))
        {
            string saveFileText = System.IO.File.ReadAllText(PATH);
            LoadedData = JsonUtility.FromJson<SaveData>(saveFileText);
            LoadedData.InitSaveData();
            SaveData();
        }
        else
        {
            LoadedData = new SaveData();
            SaveData();
            SaveDataLoaded?.Invoke();
            saveLoaded = true;
        }
    }
    private void LoadSettings()
    {
        if (System.IO.File.Exists(SETTINGSPATH))
        {
            string settingsFileData = System.IO.File.ReadAllText(SETTINGSPATH);
            LoadedSettings = JsonUtility.FromJson<Settings>(settingsFileData);
        }
        else
        {
            LoadedSettings = new Settings();
            SaveSettings(LoadedSettings);

            saveLoaded = true;
        }
    }

    public void SaveData()
    {
#if UNITY_EDITOR
        if (!CanSaveData)
        {
            Debug.Log("Saving disabled!!!!!");
            return;
        }
#endif
        System.IO.File.WriteAllText(PATH, JsonUtility.ToJson(LoadedData, true));
        Debug.Log("Save Loc: " + PATH);
    }    
    public void SaveSettings(Settings newSettings)
    {
        LoadedSettings = newSettings;
#if UNITY_EDITOR
        if (!CanSaveData)
        {
            Debug.Log("Saving disabled!!!!!");
            return;
        }
#endif
        System.IO.File.WriteAllText(SETTINGSPATH, JsonUtility.ToJson(LoadedSettings, true));
        Debug.Log("Save Loc: " + SETTINGSPATH);
    }


    public CharacterData GetCharacterByID(int characterId)
    {
        foreach (CharacterData chardata in LoadedData.SavedCharacters)
        {
            if (chardata.LocalId == characterId)
            {
                return chardata;
            }
        }
        Debug.Log("Couldn't find character");
        return null;
    }
    public List<int> GetCharacterIdsByName(string characterName)
    {
        List<int> returnIndexes = new List<int>();
        for (int i = 0; i < LoadedData.SavedCharacters.Count; i++)
        {
            if (LoadedData.SavedCharacters[i].Name == characterName)
            {
                returnIndexes.Add(LoadedData.SavedCharacters[i].LocalId);
            }
        }
        return returnIndexes;
    }
    public void UpdateCharacter(CharacterData characterData, bool isNewCharacter, int savedIndex)
    {

        int index = -1;
        if (!isNewCharacter)
        {
            index = savedIndex;
        }
        if (index == -1)
        {
            characterData.UpdateForNewVersion();
            characterData.LocalId = GetUnusedLocalId();
            LoadedData.SavedCharacters.Add(characterData);
        }
        else
        {
            characterData.UpdateForNewVersion();
            LoadedData.SavedCharacters[index] = characterData;
        }
        SaveData();
    }
    private int GetUnusedLocalId()
    {
        return LoadedData.SavedCharacters[LoadedData.SavedCharacters.Count - 1].LocalId + 1;
    }

    public float GenerateCharacterId()
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

    public void TestSaveCharToData()
    {
        LoadedData.SavedCharacters.Add(saveCharData.characterData.Clone());

    }

    public bool DeleteCharacter(int characterId)
    {
        bool success = LoadedData.RemoveCharacter(characterId);
        if (success)
        {
            SaveData();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void UpdateApartmentConfig(int apartmentOwnerLocalId, ApartmentConfig config)
    {
        try
        {
            int id = LoadedData.SavedApartmentConfigs.IndexOf( LoadedData.SavedApartmentConfigs.Find(x => x.apartmentOwnerLocalId == apartmentOwnerLocalId));
            if(id == -1)
            {
                LoadedData.SavedApartmentConfigs.Add(config);
            }
            else
            {
                LoadedData.SavedApartmentConfigs[id] = config;
            }
        }
        catch
        {
            LoadedData.SavedApartmentConfigs.Add(config);
        }
        SaveData();
    }
    public ApartmentConfig GetApartmentConfig(int ownerLocalId)
    {
        try
        {
            int id = LoadedData.SavedApartmentConfigs.IndexOf(LoadedData.SavedApartmentConfigs.Find(x => x.apartmentOwnerLocalId == ownerLocalId));
            if (id == -1)
            {
                ApartmentConfig newConfig = new ApartmentConfig();
                newConfig.apartmentOwnerLocalId = ownerLocalId;
                LoadedData.SavedApartmentConfigs.Add(newConfig);
                SaveData();
                return newConfig;
            }
            else
            {
                return LoadedData.SavedApartmentConfigs[id];
            }
        }
        catch
        {
            ApartmentConfig newConfig = new ApartmentConfig();
            newConfig.apartmentOwnerLocalId = ownerLocalId;
            LoadedData.SavedApartmentConfigs.Add(newConfig);
            SaveData();
            return newConfig;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SaveLoader))]
public class SaveLoaderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Save Data"))
        {
            (target as SaveLoader).SaveData();
        }
        if (GUILayout.Button("Add Test Char Data"))
        {
            (target as SaveLoader).TestSaveCharToData();
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
    [SerializeField] public List<ApartmentConfig> SavedApartmentConfigs;

    public bool RemoveCharacter(int charId)
    {
        int id = -1;
        try
        {
            id = SavedCharacters.IndexOf(SavedCharacters.Find(x => x.LocalId == charId));
        }
        catch
        {
            id = -1;
        }
        if (id != -1)
        {
            SavedCharacters.RemoveAt(id);
            foreach(CharacterData charData in SavedCharacters)
            {
                charData.DeleteRelationship(charId);
            }
            return true;
        }
        else return false;
    }
    public void InitSaveData()
    {
        InitCharacterDatas();
    }
    private void InitCharacterDatas()
    {
        foreach (CharacterData charData in SavedCharacters)
        {
            charData.UpdateForNewVersion();
            charData.InitRuntimeData();
        }
    }
}



[System.Serializable]
public class PlayerData
{
    [SerializeField] public string PlayerName;

    public CharacterData ToCharData()
    {
        CharacterData playerChar = new CharacterData(PlayerName, PlayerName, -1);
        playerChar.LocalId = -1;
        playerChar.FavouriteColor = new Color(0.5f, 0.5f, 0.5f);
        return playerChar;
    }
}

[SerializeField]
public class Settings
{
    [SerializeField] public string apiUrl;
    [SerializeField] public string emotionAnalysisUrl;
}

