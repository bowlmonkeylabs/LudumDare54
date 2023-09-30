using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputProcessor : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layerMask;

    private Transform prevHoverTransform;

    public void HandleClick(InputAction.CallbackContext callbackContext) {
        if(callbackContext.performed) {
            RaycastHit hitInfo;
            Physics.Raycast(_camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hitInfo, 100000, _layerMask);
            // hitInfo.transform.GetComponent<ClickInteractionReceiver>()?.ReceiveInteraction();
        }
    }

    public void Update() {
        RaycastHit hitInfo;
        Physics.Raycast(_camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hitInfo, 100000, _layerMask);
        // if() {

        // }
        // prevHoverTransform?.GetComponent<HoverInteractionReceiver>()?.ReceiveUnHoverInteraction();
        // prevHoverTransform = hitInfo.transform.GetComponent<HoverInteractionReceiver>()?.ReceiveHoverInteraction();
    }
}
