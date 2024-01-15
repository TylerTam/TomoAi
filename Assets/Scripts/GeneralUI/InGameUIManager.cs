using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    public static InGameUIManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<InGameUIManager>();
            return _instance;
        }
    }
    private static InGameUIManager _instance;

    [SerializeField] public DialogueUiSystem dialogueUi;
    [SerializeField] public ToggleableInGameUI relationshipMenu;
    [SerializeField] public ToggleableInGameUI happinessMenu;
    [SerializeField] public ToggleableInGameUI worldNpcMenu;

    private TomoCharPerson targetChar;
    private Dictionary<InGameUIType, bool> openMenus = new Dictionary<InGameUIType, bool>();
    public enum InGameUIType
    {
        None,
        WorldNpcMenu,
        Dialogue,
        Relationship,
        Happiness,
    }
    private void Awake()
    {
        dialogueUi.ForceClose();
        relationshipMenu.ForceClose();
        happinessMenu.ForceClose();
        worldNpcMenu.ForceClose();


        openMenus.Add(InGameUIType.None, false);
        openMenus.Add(InGameUIType.WorldNpcMenu, false);
        openMenus.Add(InGameUIType.Dialogue, false);
        openMenus.Add(InGameUIType.Relationship, false);
    }
    public void OpenMenu(InGameUIType menuType, TomoCharPerson character = null)
    {
        //if (openMenus[menuType]) return;
        
        if (character != null) targetChar = character;
        PlayerController.Instance.ToggleInteractionInput(false);
        switch (menuType)
        {
            case InGameUIType.None:
                worldNpcMenu.CloseMenu();
                dialogueUi.CloseMenu();
                relationshipMenu.CloseMenu();
                happinessMenu.CloseMenu();
                StartCoroutine(DelayPlayerInteractionEnable(targetChar));
                dialogueUi.ResetConversation();
                targetChar = null;

                openMenus[InGameUIType.WorldNpcMenu] = false;
                openMenus[InGameUIType.Dialogue] = false;
                openMenus[InGameUIType.Relationship] = false;
                break;
            case InGameUIType.WorldNpcMenu:
                worldNpcMenu.gameObject.SetActive(true);
                worldNpcMenu.ToggleMenu( targetChar);
                openMenus[menuType] = true;
                break;
            case InGameUIType.Dialogue:
                dialogueUi.gameObject.SetActive(true);
                openMenus[menuType] = true;
                dialogueUi.ToggleMenu();
                break;
            case InGameUIType.Relationship:
                relationshipMenu.gameObject.SetActive(true);
                relationshipMenu.ToggleMenu(targetChar);
                happinessMenu.CloseMenu();
                openMenus[menuType] = true;
                break;
            case InGameUIType.Happiness:
                happinessMenu.gameObject.SetActive(true);
                happinessMenu.ToggleMenu(targetChar);
                relationshipMenu.CloseMenu();
                openMenus[menuType] = true;
                break;
        }
    }
    public void CloseMenu(InGameUIType menuType)
    {
        if (openMenus[menuType] == false) return;
        switch (menuType)
        {
            case InGameUIType.WorldNpcMenu:
                worldNpcMenu.CloseMenu();
                dialogueUi.CloseMenu();
                relationshipMenu.CloseMenu();
                openMenus[InGameUIType.WorldNpcMenu] = false;
                openMenus[InGameUIType.Dialogue] = false;
                openMenus[InGameUIType.Relationship] = false;
                break;
            case InGameUIType.Dialogue:
                dialogueUi.CloseMenu();
                openMenus[InGameUIType.Dialogue] = false;
                break;
            case InGameUIType.Relationship:
                relationshipMenu.CloseMenu();
                openMenus[InGameUIType.Relationship] = false;
                break;
        }
    }
    private IEnumerator DelayPlayerInteractionEnable(TomoCharPerson tomochar)
    {
        if (tomochar != null)
        {
            tomochar.GetComponent<TomoCharInteraction>().EndTap();
        }
        yield return new WaitForSeconds(1);
        PlayerController.Instance.ToggleInteractionInput(true);


    }

}
