using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleableInGameUI : MonoBehaviour
{
    private bool isOpen;
    public virtual bool ToggleMenu(bool enable)
    {
        if (enable == isOpen) return false;
        isOpen = enable;
        return true;
    }    
    public virtual bool ToggleMenu(bool enable, TomoCharPerson tomoChar)
    {

        if (enable == isOpen) return false;
        isOpen = enable;
        return true;
    }

    public virtual void ForceClose()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }
}
