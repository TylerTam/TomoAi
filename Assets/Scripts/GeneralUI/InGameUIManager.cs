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
    [SerializeField] public ToggleableInGameUI worldNpcMenu;

    private TomoCharPerson targetChar;
    private Dictionary<InGameUIType, bool> openMenus = new Dictionary<InGameUIType, bool>();
    public enum InGameUIType
    {
        None,
        WorldNpcMenu,
        Dialogue,
        Relationship,
    }
    private void Awake()
    {
        dialogueUi.ForceClose();
        relationshipMenu.ForceClose();
        worldNpcMenu.ForceClose();

        openMenus.Add(InGameUIType.None, false);
        openMenus.Add(InGameUIType.WorldNpcMenu, false);
        openMenus.Add(InGameUIType.Dialogue, false);
        openMenus.Add(InGameUIType.Relationship, false);
    }
    public void OpenMenu(InGameUIType menuType, TomoCharPerson character = null)
    {
        if (openMenus[menuType]) return;
        
        if (character != null) targetChar = character;
        PlayerController.Instance.ToggleInteractionInput(false);
        switch (menuType)
        {
            case InGameUIType.None:
                worldNpcMenu.ToggleMenu(false, null);
                dialogueUi.ToggleMenu(false);
                relationshipMenu.ToggleMenu(false, null);
                StartCoroutine(DelayPlayerInteractionEnable(targetChar));
                dialogueUi.ResetConversation();
                targetChar = null;

                openMenus[InGameUIType.WorldNpcMenu] = false;
                openMenus[InGameUIType.Dialogue] = false;
                openMenus[InGameUIType.Relationship] = false;
                break;
            case InGameUIType.WorldNpcMenu:
                worldNpcMenu.gameObject.SetActive(true);
                worldNpcMenu.ToggleMenu(true, targetChar);
                openMenus[menuType] = true;
                break;
            case InGameUIType.Dialogue:
                dialogueUi.gameObject.SetActive(true);
                openMenus[menuType] = true;
                dialogueUi.ToggleMenu(true);
                break;
            case InGameUIType.Relationship:
                relationshipMenu.gameObject.SetActive(true);
                relationshipMenu.ToggleMenu(true,targetChar);
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
                worldNpcMenu.ToggleMenu(false, null);
                dialogueUi.ToggleMenu(false);
                relationshipMenu.ToggleMenu(false, null);
                openMenus[InGameUIType.WorldNpcMenu] = false;
                openMenus[InGameUIType.Dialogue] = false;
                openMenus[InGameUIType.Relationship] = false;
                break;
            case InGameUIType.Dialogue:
                dialogueUi.ToggleMenu(false);
                openMenus[InGameUIType.Dialogue] = false;
                break;
            case InGameUIType.Relationship:
                relationshipMenu.ToggleMenu(false, null);
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
