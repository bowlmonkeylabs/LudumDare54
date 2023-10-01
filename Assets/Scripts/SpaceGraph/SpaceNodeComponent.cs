using System;
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
        [SerializeField] private UnityEvent _onNavigate;

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
            else if (SpaceNode.PlayerVisited)
            {
                result = (_hoverInteraction.IsHovered ? _colorHoverReachable : _colorExplored);
            }
            else if (SpaceNode.PlayerOccupiedAdjacent)
            {
                result = (_hoverInteraction.IsHovered ? _colorHoverReachable : _colorReachable);
            }
            
            _sphere.Color = result;
        }

        public void TryNavigate()
        {
            if (!this.SpaceNode.PlayerOccupiedAdjacent)
            {
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
        }

        #endregion
    }
}