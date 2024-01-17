using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WorldNpcMenu : ToggleableInGameUI
{

    [SerializeField] private CanvasGroup cg;
    [SerializeField] private VerticalLayoutGroup vertLayout;
    [SerializeField] private float targetSpacing = 0;
    [SerializeField] private float hiddenSpacing = -1;
    [SerializeField] private float animTime = 0.5f;
    [SerializeField] private float fadeTime = 0.5f;
    private TomoCharInteraction targetTomochar;
    [SerializeField] private AnimationCurve animCurve;
    private void Awake()
    {
        cg.interactable = false;
        cg.gameObject.SetActive(false);
    }

    public override bool ToggleMenu(TomoCharPerson tomoChar)
    {
        if (!base.ToggleMenu(tomoChar)) return false;
        

        if (isOpen)
        {
            transform.position = tomoChar.transform.position;
            targetTomochar = tomoChar.GetComponent<TomoCharInteraction>();
        }

        cg.interactable = false;
        cg.gameObject.SetActive(true);
        StopAllCoroutines();
        if (isOpen) StartCoroutine(ShowMenu());
        else StartCoroutine(HideMenu());
        return true;

    }

    public override bool CloseMenu()
    {
        if (!base.CloseMenu()) return false;
        StartCoroutine(HideMenu());
        return true;
    }

    public override void ForceClose()
    {
        vertLayout.spacing = hiddenSpacing;
        cg.alpha = 0;
        base.ForceClose();
    }
    private IEnumerator ShowMenu()
    {
        cg.interactable = true;
        cg.blocksRaycasts = false;
        StartCoroutine(DoFadeIn());
        float timer = 0;
        while (timer < animTime)
        {
            vertLayout.spacing = Mathf.LerpUnclamped(hiddenSpacing, targetSpacing, animCurve.Evaluate(timer / animTime));
            timer += Time.deltaTime;
            yield return null;
        }
        cg.blocksRaycasts = true;
        vertLayout.spacing = targetSpacing;
    }
    private IEnumerator DoFadeIn()
    {
        float timer = 0;
        while (timer < fadeTime)
        {
            cg.alpha = timer / fadeTime;
            timer += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1;
    }
    private IEnumerator HideMenu()
    {
        float timer = 0;
        while(timer < fadeTime)
        {
            cg.alpha = 1 - (timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }
        ForceClose();
    }

    /// <summary>
    /// Pressed from button unity events in the world npc menu ui
    /// </summary>
    /// <param name="buttonIndex"></param>
    public void ButtonPressed(int buttonIndex)
    {
        switch (buttonIndex)
        {
            //Exit
            case 0:
                InGameUIManager.Instance.OpenMenu(InGameUIManager.InGameUIType.None);
                break;

            //Talk
            case 1:
                InGameUIManager.Instance.OpenMenu(InGameUIManager.InGameUIType.Dialogue);
                break;

            //open relation ship menu
            case 2:
                InGameUIManager.Instance.OpenMenu(InGameUIManager.InGameUIType.Relationship);
                break;
            case 3:
                InGameUIManager.Instance.OpenMenu(InGameUIManager.InGameUIType.Happiness);
                break;
            default:
                break;
        }

    }
}
