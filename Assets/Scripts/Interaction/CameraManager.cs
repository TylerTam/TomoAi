using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraManager>();
            }
            return _instance;
        }
    }
    private static CameraManager _instance;
    public enum CamType
    {
        FarCam,
        MainCam,
        DialogueCam
    }
    [SerializeField] private CamType camType;
    [SerializeField] private CinemachineVirtualCamera apartmentSelectCam, mainCam, dialogueCam;

    public void SwitchCam(CamType newCamType, bool disableInteract = true)
    {
        if (disableInteract)
        {
            PlayerController.Instance.ToggleInteractionInput(false);
        }
        switch (newCamType)
        {
            case CamType.FarCam:
                apartmentSelectCam.gameObject.SetActive(true);
                dialogueCam.gameObject.SetActive(false);
                mainCam.gameObject.SetActive(false);
                break;
            case CamType.MainCam:
                StartCoroutine(DelayPlayerInput());
                mainCam.gameObject.SetActive(true);
                apartmentSelectCam.gameObject.SetActive(false);
                dialogueCam.gameObject.SetActive(false);
                break;
            case CamType.DialogueCam:
                dialogueCam.gameObject.SetActive(true);
                apartmentSelectCam.gameObject.SetActive(false);
                mainCam.gameObject.SetActive(false);
                break;
        }
    }

    public void OpenApartmentCam()
    {
        PlayerController.Instance.ResetCamPos();
        SwitchCam(CamType.MainCam);
    }

    public void SetDialogueCam(Transform parent)
    {
        if (parent != null)
        {
            dialogueCam.transform.parent = parent;
            dialogueCam.transform.localPosition = Vector3.zero;
            dialogueCam.transform.localRotation = Quaternion.identity;
            dialogueCam.transform.localScale = Vector3.one;
            SwitchCam(CamType.DialogueCam);
        }
        else
        {
            SwitchCam(CamType.MainCam);
            dialogueCam.transform.parent = transform;

        }
    }

    private IEnumerator DelayPlayerInput()
    {
        yield return new WaitForSeconds(1);
        PlayerController.Instance.ToggleInteractionInput(true);
    }

    
}
