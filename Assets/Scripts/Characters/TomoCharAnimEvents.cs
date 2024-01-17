using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TomoCharAnimEvents : MonoBehaviour
{
    [SerializeField] private TomoCharController tomoCharController;
    public void StopFalling()
    {
        tomoCharController.waitForFallingAnim = false;
    }
}
