using System;
using BML.ScriptableObjectCore.Scripts.Variables;
using Codice.Client.GameUI.Update;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts.SpaceGraph
{
    public class SpaceNodeComponent : MonoBehaviour
    {
        #region Inspector

        [TitleGroup("Instance params")]
        [SerializeField] public SpaceNode SpaceNode;
        
        [TitleGroup("Prefab params")]
        [SerializeField] private Sphere _sphere;
        [SerializeField] private HoverInteractionReceiver _hoverInteraction;
        [SerializeField] private Color _colorExplored;
        [SerializeField] private Color _colorReachable;
        [SerializeField] private Color _colorHoverReachable;
        [SerializeField] private Color _colorUnreachable;
        [SerializeField] private IntVariable _fuelAmount;
        [SerializeField] private UnityEvent _onNavigate;
        [SerializeField] private UnityEvent _onFuelCheckFailed;
        [SerializeField] private UnityEvent<bool> _onHoverNotExplored;

        #endregion

        #region Unity lifecycle

        private void OnEnable()
        {
            UpdateDisplay();
            _hoverInteraction.OnHoverChange += OnHoverChange;
            if (this.SpaceNode != null)
            {
                this.SpaceNode.OnUpdate += UpdateDisplay;
            }
        }

        private void OnDisable()
        {
            _hoverInteraction.OnHoverChange -= OnHoverChange;
            if (this.SpaceNode != null)
            {
                this.SpaceNode.OnUpdate -= UpdateDisplay;
            }
        }

        #endregion

        #region Public interface

        public void Init(SpaceNode spaceNode)
        {
            if (this.SpaceNode != null)
            {
                this.SpaceNode.OnUpdate -= UpdateDisplay;
            }
            this.SpaceNode = spaceNode;
            if (this.SpaceNode != null)
            {
                this.SpaceNode.OnUpdate += UpdateDisplay;
            }

            // Position and align
            this.transform.localPosition = spaceNode.LocalPosition;
            this.transform.rotation = Quaternion.identity;
        }

        public void UpdateDisplay()
        {
            if (SpaceNode.ParentGraph == null || this.SpaceNode.IsStartNode() || this.SpaceNode.IsEndNode())
            {
                return;
            }
            
            Color result = _colorUnreachable;
            if (SpaceNode.PlayerOccupied)
            {
                result = _colorExplored;
            }
            else if (SpaceNode.PlayerOccupiedAdjacent)
            {
                if (_hoverInteraction.IsHovered)
                {
                    result = _colorHoverReachable;
                }
                else
                {
                    result = (SpaceNode.PlayerVisited ? _colorExplored : _colorReachable);
                }
            }
            else if (SpaceNode.PlayerVisited)
            {
                result = _colorExplored;
            }
            
            _sphere.Color = result;
        }

        public void TryNavigate()
        {
            if (!this.SpaceNode.PlayerOccupiedAdjacent)
            {
                return;
            }

            if(_fuelAmount.Value <= 0) {
                _onFuelCheckFailed.Invoke();
                return;
            }

            DoNavigate();
        }

        public void DoNavigate()
        {
            _onNavigate?.Invoke();
        }

        #endregion

        #region Event callbacks

        private void OnHoverChange(bool isHovered)
        {
            UpdateDisplay();
            if(SpaceNode.PlayerVisited || SpaceNode.PlayerOccupiedAdjacent || SpaceNode.PlayerOccupied) {
                _onHoverNotExplored.Invoke(isHovered);
            }
        }

        #endregion
    }
}