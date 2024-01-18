using System.Collections;
using System.Collections.Generic;
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

    private float holdTime = 0;
    private bool isHoldingMouse;
    public bool CanInteract { get { return canInteract; } }

    #region Dragging
    [SerializeField] private LayerMask floorMask;
    private GameObject dragging;
    [SerializeField] private float dragHeightOffset;
    [SerializeField] private Vector3 mouseOffset;
    #endregion
    void Update()
    {

        RaycastForInteractable(Input.mousePosition);
        if (canInteract)
        {

            //Check for intial down
            if (Input.GetMouseButtonDown(0))
            {
                holdTime = 0;
                isHoldingMouse = false;
            }
            //Check for hold
            if (Input.GetMouseButton(0))
            {
                if(holdTime >= 0.1f)
                {
                    PerformDrag();
                    isHoldingMouse = true;
                }
                else
                {
                    holdTime += Time.deltaTime;
                }
            }
            //Check for release
            if(Input.GetMouseButtonUp(0))
            {
                if (!isHoldingMouse)
                {
                    ScreenTapped();
                }
                else
                {
                    StopDrag();
                }
                holdTime = 0;
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


    private void PerformDrag()
    {
        if (!isHoldingMouse)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 1000, InteractableLayer))
            {
                dragging = hit.transform.gameObject;
                IInteractable interactable = dragging.GetComponent<IInteractable>();
                if(interactable != null) interactable.TapHold();
                mouseOffset = Vector3.zero;
                if (Physics.Raycast(hit.point, Vector3.down, out RaycastHit hit2, 1000, floorMask))
                {
                    mouseOffset = (Camera.main.WorldToScreenPoint(hit2.point) - Input.mousePosition);

                }
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition + mouseOffset), out RaycastHit hit3, 1000, floorMask))
                {
                    Rigidbody rb = dragging.GetComponent<Rigidbody>();
                    if (rb) rb.isKinematic = true;
                    dragging.transform.position = hit3.point + Vector3.up * dragHeightOffset;
                }

            }
        }
        else
        {
            if (dragging != null)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition + mouseOffset), out RaycastHit hit, 1000, floorMask))
                {
                    dragging.transform.position = hit.point + Vector3.up * dragHeightOffset;
                }
            }
        }
    }
    private void StopDrag()
    {
        if (dragging == null) return;

        Rigidbody rb = dragging.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        IInteractable interactable = dragging.GetComponent<IInteractable>();
        if (interactable != null) interactable.TapReleased();
        dragging = null;

    }
}
