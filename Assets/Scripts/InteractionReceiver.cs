using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionReceiver : MonoBehaviour
{
    [SerializeField] private UnityEvent _interactionReceived;

    public void ReceiveInteraction() {
        _interactionReceived.Invoke();
    }
}
