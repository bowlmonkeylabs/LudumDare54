using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using BML.ScriptableObjectCore.Scripts.Variables.SafeValueReferences;
using UnityEngine;
using UnityEngine.Events;

namespace BML.Scripts
{
    public class TransformDistanceCheck : MonoBehaviour
    {
        private enum EqualityCheck {
            GreaterThanOrEqualTo,
            LessThanOrEqualTo
        }

        [SerializeField] private Transform _transformOne;
        [SerializeField] private TransformSceneReference _transformTwo;
        [SerializeField] private SafeFloatValueReference _distanceToCheck;
        [SerializeField] private EqualityCheck _equalityCheck;
        [SerializeField] private UnityEvent _onSucceed;
        [SerializeField] private UnityEvent _onFail;

        public void PerformCheck() {
            if(_equalityCheck == EqualityCheck.GreaterThanOrEqualTo) {
                if(Vector3.Distance(_transformOne.position, _transformTwo.Value.position) >= _distanceToCheck.Value) {
                    _onSucceed.Invoke();
                } else {
                    _onFail.Invoke();
                }
            }

            if(_equalityCheck == EqualityCheck.LessThanOrEqualTo) {
                if(Vector3.Distance(_transformOne.position, _transformTwo.Value.position) <= _distanceToCheck.Value) {
                    _onSucceed.Invoke();
                } else {
                    _onFail.Invoke();
                }
            }
        }
    }
}
