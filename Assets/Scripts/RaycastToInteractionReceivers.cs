using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BML.Scripts
{
    public class RaycastToInteractionReceivers : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerMask;

        public void DoRaycast(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.performed)
            {
                RaycastHit hitInfo;
                Physics.Raycast(_camera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hitInfo, 100000,
                    _layerMask);
                hitInfo.transform.GetComponent<InteractionReceiver>()?.ReceiveInteraction();
            }
        }
    }
}
