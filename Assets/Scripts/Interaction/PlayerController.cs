using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    private bool canInteract = true;

    void Update()
    {
        if (canInteract)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ScreenTapped(Input.mousePosition);
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
    private void ScreenTapped(Vector2 screenPos)
    {
        Ray ray = CurrentCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 10000, InteractableLayer))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Tapped();
            }
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
