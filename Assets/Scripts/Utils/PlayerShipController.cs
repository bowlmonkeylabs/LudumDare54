using System;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.Scripts.PID;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace BML.Scripts.Utils
{
    public class PlayerShipController : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private DynamicGameEvent _mapPointClicked;
        [SerializeField] private DynamicGameEvent _playerReachedMapPoint;

        [SerializeField] private Rigidbody _rigidbody;
        [FormerlySerializedAs("_mainEngingPid")] [SerializeField] private PidController _mainEnginePid;
        [SerializeField] private PidController _precisionThrustersPid;

        [SerializeField] private float _mainEngineCutoffDistance = 2f;
        [SerializeField] private float _precisionThrusterEngageDistance = 2f;
        [FormerlySerializedAs("_reachedTargetThresholed")] [SerializeField] private float _reachedTargetThreshold = 0.2f;
        
        #endregion

        [ShowInInspector, NonSerialized, ReadOnly] private Transform _currentOccupiedPoint;
        [ShowInInspector, NonSerialized, ReadOnly] private Transform _currentNavigationTarget;
        [ShowInInspector, ReadOnly] private bool _isNavigating => _currentNavigationTarget != null;
        
        #region Unity lifecycle

        private void OnEnable()
        {
            _mapPointClicked.Subscribe(OnMapPointClickedDynamic);
        }

        private void OnDisable()
        {
            _mapPointClicked.Unsubscribe(OnMapPointClickedDynamic);
        }

        private void FixedUpdate()
        {
            if (_currentNavigationTarget != null)
            {
                // Check distance to change engine characteristics for approach
                var diffToTarget = _currentNavigationTarget.position - transform.position;
                var distanceToTarget = diffToTarget.magnitude;

                // Check if reached target
                if (distanceToTarget <= _reachedTargetThreshold)
                {
                    _mainEnginePid.enabled = false;
                    // _precisionThrustersPid.enabled = false;
                    OnNavigationCompleted();
                    return;
                }
            
                if (distanceToTarget <= _mainEngineCutoffDistance)
                {
                    _mainEnginePid.enabled = false;
                }

                if (distanceToTarget <= _precisionThrusterEngageDistance)
                {
                    _precisionThrustersPid.enabled = true;
                }
            }
            else if (_currentOccupiedPoint)
            {
                _precisionThrustersPid.enabled = true;
            }
            else
            {
                _precisionThrustersPid.enabled = false;
                _rigidbody.isKinematic = true;
            }
        }

        #endregion

        
        private void OnMapPointClickedDynamic(object prev, object curr)
        {
            OnMapPointClicked(curr as Transform);
        }
        
        private void OnMapPointClicked(Transform point)
        {
            // TODO start navigating
            if (_currentNavigationTarget != null)
            {
                Debug.LogError("Already navgiating!");
                return;
            }
            Debug.Log($"OnMapPointClicked {point}");
            _currentNavigationTarget = point;
            _rigidbody.isKinematic = false;
            
            _mainEnginePid.Target.AssignConstantValue(_currentNavigationTarget);
            _mainEnginePid.enabled = true;
            _precisionThrustersPid.Target.AssignConstantValue(_currentNavigationTarget);
            _precisionThrustersPid.enabled = false;
        }

        private void OnNavigationCompleted()
        {
            var reachedPoint = _currentNavigationTarget;
            _currentOccupiedPoint = _currentNavigationTarget;
            _currentNavigationTarget = null;
            _playerReachedMapPoint?.Raise(reachedPoint);
        }
        
    }
}