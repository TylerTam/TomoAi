using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance
    {
        get
        {
            if (_instance == null) _instance = FindAnyObjectByType<PlayerController>();
            return _instance;
        }
    }
    private static PlayerController _instance;

    [SerializeField] private Camera CurrentCamera;
    [SerializeField] private LayerMask InteractableLayer;
    [Header("Cam Rotate")]
    [SerializeField] private Vector2 camRotateSpeed;
    [SerializeField] private Transform camHoriz, camVert, cam;
    [Header("Zoom Cam")]
    [SerializeField] private float zoomSpeed;
    [SerializeField] private Vector2 zoomBounds;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector2 moveBounds;

    [SerializeField] private bool canInteract = false;

    [SerializeField] private IInteractable currentHoveredInteractable;
    void Update()
    {

        RaycastForInteractable(Input.mousePosition);
        if (canInteract)
        {

            if (Input.GetMouseButtonDown(0))
            {
                ScreenTapped();
            }
        }
        if (Input.GetMouseButton(1))
        {
            RotateCam(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
        }
        if (Input.mouseScrollDelta != Vector2.zero)
        {
            ZoomCam(Input.mouseScrollDelta.y);
        }

        MoveCam(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
    }


    public void ResetCamPos()
    {
        camHoriz.localEulerAngles = camHoriz.localPosition = camVert.localEulerAngles = Vector3.zero;
        cam.localPosition = new Vector3(0, 0, -10);
    }

    public void ToggleInteractionInput(bool enable)
    {
        canInteract = enable;
    }
    private void RaycastForInteractable(Vector2 screenPos)
    {
        if (!canInteract)
        {
            if (currentHoveredInteractable != null)
            {
                currentHoveredInteractable.HoverToggle(false);
                currentHoveredInteractable = null;
            }
            return;
        }
        Ray ray = CurrentCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 10000, InteractableLayer))
        {
            IInteractable tempIteractable = hit.transform.GetComponent<IInteractable>();
            if (tempIteractable == currentHoveredInteractable) return;
            if (currentHoveredInteractable != null)
            {
                currentHoveredInteractable.HoverToggle(false);
            }
            currentHoveredInteractable = tempIteractable;
            currentHoveredInteractable.HoverToggle(true);
        }
        else
        {
            if (currentHoveredInteractable != null)
            {
                currentHoveredInteractable.HoverToggle(false);
            }
            currentHoveredInteractable = null;
        }
    }
    private void ScreenTapped()
    {
        if (currentHoveredInteractable != null)
        {
            currentHoveredInteractable.Tapped();
        }
    }


    private void RotateCam(Vector2 delta)
    {
        camHoriz.Rotate(Vector3.up, delta.x * camRotateSpeed.x);
        camVert.Rotate(Vector3.right, -delta.y * camRotateSpeed.y);
    }
    private void ZoomCam(float delta)
    {
        cam.localPosition = new Vector3(0, 0, Mathf.Clamp(cam.localPosition.z + delta * zoomSpeed, zoomBounds.x, zoomBounds.y));
    }

    private void MoveCam(Vector3 delta)
    {
        if (delta == Vector3.zero) return;
        Vector3 localDelta = camHoriz.transform.TransformVector(delta);
        Vector3 newPosition = camHoriz.position + (localDelta * moveSpeed * Time.deltaTime);

        newPosition.x = Mathf.Clamp(newPosition.x, -moveBounds.x, moveBounds.x);
        newPosition.z = Mathf.Clamp(newPosition.z, -moveBounds.y, moveBounds.y);
        camHoriz.transform.position = newPosition;

    }
}
