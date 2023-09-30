using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BML.Scripts
{
    public class HoverInteractionReceiver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent _enterInteractionReceived;
        [SerializeField] private UnityEvent _exitInteractionReceived;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Hover Enter: " + transform.gameObject.name);
            _enterInteractionReceived.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("Hover Exit: " + transform.gameObject.name);
            _exitInteractionReceived.Invoke();
        }
    }
}
