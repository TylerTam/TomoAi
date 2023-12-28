using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PromptWindow : MonoBehaviour
{
    public static PromptWindow Instance { 
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<PromptWindow>();
            }
            return _instance;
        }
    }
    private static PromptWindow _instance;
    [SerializeField] private TMPro.TextMeshProUGUI promptText;
    [SerializeField] private Button_Actions yesButton;
    [SerializeField] private Button_Actions noButton;
    [SerializeField] private Button_Actions cancelButton;

    public UnityEvent YesPressed;
    public UnityEvent NoPressed;
    public UnityEvent Cancelled;
    private void Awake()
    {
        yesButton.Submitted += SubmitYes;
        noButton.Submitted += SubmitNo;
        cancelButton.Submitted += SubmitCancel;
    }


    public void ShowPromptWindow(string message, bool canSayNo = false, bool canCancel = false)
    {
        promptText.text = message;
        noButton.gameObject.SetActive(canSayNo);
        cancelButton.gameObject.SetActive(canCancel);
    }

    private void SubmitYes()
    {
        YesPressed?.Invoke();
        ClearListeners();
    }
    private void SubmitNo()
    {
        NoPressed?.Invoke();
        ClearListeners();
    }
    private void SubmitCancel()
    {
        Cancelled?.Invoke();
        ClearListeners();
    }

    private void ClearListeners()
    {
        YesPressed.RemoveAllListeners();
        NoPressed.RemoveAllListeners();
        Cancelled.RemoveAllListeners();
    }
}
