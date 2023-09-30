using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts
{
    public class InteractionReceiver : MonoBehaviour
    {
        [SerializeField] private UnityEvent _interactionReceived;

        public void ReceiveInteraction() {
            _interactionReceived.Invoke();
        }
    }
}
