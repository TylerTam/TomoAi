using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private TMPro.TMP_InputField apiUrlField;
    [SerializeField] private TMPro.TMP_InputField emotionalAnalysisUrlField;

    private void Awake()
    {
        settingsMenu.SetActive(false);
    }
    public void ToggleSettingsMenu(bool enable)
    {

        PlayerController.Instance.ToggleInteractionInput(!enable);
        apiUrlField.text = GameManager.Instance.SaveLoader.LoadedSettings.apiUrl;
        emotionalAnalysisUrlField.text = GameManager.Instance.SaveLoader.LoadedSettings.emotionAnalysisUrl;
        settingsMenu.SetActive(enable);
    }
    public void SaveSettings()
    {
        Settings newSettings = new Settings();
        newSettings.apiUrl = apiUrlField.text;
        newSettings.emotionAnalysisUrl = emotionalAnalysisUrlField.text;
        GameManager.Instance.SaveLoader.SaveSettings(newSettings);
        ToggleSettingsMenu(false);
    }
}
