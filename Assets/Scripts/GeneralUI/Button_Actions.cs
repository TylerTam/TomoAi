using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_Actions : Button
{
    public System.Action Submitted;
    public override void OnSubmit(BaseEventData eventData)
    {
        base.OnSubmit(eventData);
        Submitted?.Invoke();
    }
}
