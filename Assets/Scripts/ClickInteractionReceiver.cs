using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BML.Scripts
{
    public class ClickInteractionReceiver : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private UnityEvent _interactionReceived;

        public void OnPointerDown(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left) {
                _interactionReceived.Invoke();
            }
        }
    }
}
