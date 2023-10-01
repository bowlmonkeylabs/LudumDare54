using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BML.Scripts
{
    public class HoverInteractionReceiver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private UnityEvent _enterInteractionReceived;
        [SerializeField] private UnityEvent _exitInteractionReceived;

        public delegate void _OnHoverChange(bool isHovered);
        public _OnHoverChange OnHoverChange;
        
        [ShowInInspector, ReadOnly] public bool IsHovered { get; private set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _enterInteractionReceived.Invoke();
            IsHovered = true;
            OnHoverChange?.Invoke(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _exitInteractionReceived.Invoke();
            IsHovered = false;
            OnHoverChange?.Invoke(false);
        }
    }
}
