using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToggleableInGameUI : MonoBehaviour
{
    protected bool isOpen;
    public virtual bool ToggleMenu()
    {
        
        isOpen = !isOpen;
        return true;
    }    
    public virtual bool ToggleMenu(TomoCharPerson tomoChar)
    {
        isOpen = !isOpen;
        return true;
    }

    public virtual bool CloseMenu()
    {
        if (!isOpen) return false;
        isOpen = false;
        return true;
    }

    public virtual void ForceClose()
    {
        isOpen = false;
        gameObject.SetActive(false);
    }
}
