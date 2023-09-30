using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastToInteractionReceivers : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layerMask;

    public void DoRaycast(InputAction.CallbackContext callbackContext) {
        RaycastHit hitInfo;
        Physics.Raycast(_camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hitInfo, 100000, _layerMask);
        Debug.Log(hitInfo.transform.gameObject.name);
        hitInfo.transform.GetComponent<InteractionReceiver>()?.ReceiveInteraction();
    }
}
