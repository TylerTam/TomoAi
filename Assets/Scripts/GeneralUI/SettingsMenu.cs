using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private TMPro.TMP_InputField apiUrlField;


    public void ToggleSettingsMenu(bool enable)
    {

        PlayerController.Instance.ToggleInteractionInput(!enable);
        settingsMenu.SetActive(enable);
    }
    public void SaveSettings()
    {
        Settings newSettings = new Settings();
        newSettings.apiUrl = apiUrlField.text;
        GameManager.Instance.SaveLoader.SaveSettings(newSettings);
        ToggleSettingsMenu(false);
    }
}
