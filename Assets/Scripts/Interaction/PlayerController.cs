using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Camera CurrentCamera;
    public LayerMask InteractableLayer;
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            ScreenTapped(Input.mousePosition);
        }
    }

    public void ScreenTapped(Vector2 screenPos)
    {
        Ray ray = CurrentCamera.ScreenPointToRay(screenPos);
        if(Physics.Raycast(ray, out RaycastHit hit, 10000, InteractableLayer))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if(interactable != null)
            {
                interactable.Tapped();
            }
        }
    }
}
