using System;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using BML.ScriptableObjectCore.Scripts.Variables.SafeValueReferences;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace BML.Scripts.PID
{
    public class PidController : MonoBehaviour
    {
        #region Inspector

        private enum TargetMode
        {
            Direction,
            MatchRotation,
            MatchPosition,
        }

        [SerializeField] private TargetMode _targetMode = TargetMode.Direction;
        private bool _isMatchPositionMode => _targetMode == TargetMode.MatchPosition;
        [FormerlySerializedAs("_target")] [SerializeField] public SafeTransformValueReference Target;
        [SerializeField, HideIf("_isMatchPositionMode")] private bool _doAlignToTargetOnEnable;
        
        [SerializeField] private float _maxAngularVelocity = 20;

        [SerializeField] private float _constantThrust = 0;
        private bool _useConstantThrust => _constantThrust > 0;
        [SerializeField, ShowIf("_useConstantThrust")] private ForceMode _constantThrustForceMode;
        [SerializeField, HideIf("@_isMatchPositionMode || _useConstantThrust")] private PIDParameters _thrustParameters;

        [SerializeField] private bool _useXValuesForAllAxes = false;

        [SerializeField] private PIDParameters _xAxisParameters;
        
        [HideIf("$_useXValuesForAllAxes")]
        [SerializeField] private PIDParameters _yAxisParameters;
        
        [HideIf("$_useXValuesForAllAxes")]
        [SerializeField] private PIDParameters _zAxisParameters;
        
        [ShowInInspector, HideIf("_isMatchPositionMode")] private PID2 _thrustPIDController;
        [ShowInInspector] private PID2 _xAxisPIDController;
        [ShowInInspector] private PID2 _yAxisPIDController;
        [ShowInInspector] private PID2 _zAxisPIDController;

        private Rigidbody _rb;

        #endregion

        #region Unity lifecycle

        private void OnEnable()
        {
            if (_doAlignToTargetOnEnable)
            {
                Quaternion targetRotation;
                switch (_targetMode)
                {
                    default:
                    case TargetMode.Direction:
                        var targetPosition = Target.Value.transform.position;
                        var targetDirection = targetPosition - transform.position;
                        targetRotation = Quaternion.LookRotation(targetDirection);
                        break;
                    case TargetMode.MatchRotation:
                        targetRotation = Target.Value.rotation;
                        break;
                    case TargetMode.MatchPosition:
                        // do nothing
                        targetRotation = transform.rotation;
                        break;
                }
                

                transform.rotation = targetRotation;
            }
        }

        void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _thrustPIDController = new PID2(_thrustParameters);
            _xAxisPIDController = new PID2(_xAxisParameters);
            if (_useXValuesForAllAxes)
            {
                _yAxisPIDController = new PID2(_xAxisParameters);
                _zAxisPIDController = new PID2(_xAxisParameters);
            }
            else
            {
                _yAxisPIDController = new PID2(_yAxisParameters);
                _zAxisPIDController = new PID2(_zAxisParameters);
            }
        }
        
        private void Update()
        {
            if (Target.Value.SafeIsUnityNull()) return;

            _thrustPIDController.Parameters = _thrustParameters;
            _xAxisPIDController.Parameters = _xAxisParameters;

            if (_useXValuesForAllAxes)
            {
                _yAxisPIDController.Parameters = _xAxisParameters;
                _zAxisPIDController.Parameters = _xAxisParameters;
            }
            else
            {
                _yAxisPIDController.Parameters = _yAxisParameters;
                _zAxisPIDController.Parameters = _zAxisParameters;
            }
        }
        
        void FixedUpdate()
        {
            _rb.maxAngularVelocity = _maxAngularVelocity;
            
            if (Target.Value.SafeIsUnityNull()) return;

            if (_targetMode == TargetMode.MatchPosition)
            {
                var position = transform.position;
                var rotation = transform.rotation;
                var targetPosition = Target.Value.position;
                
                var positionDiff = (position - targetPosition);
                
                float xPositionCorrection = _xAxisPIDController.GetOutput(positionDiff.x, 0, Time.fixedDeltaTime);
                float yPositionCorrection = _yAxisPIDController.GetOutput(positionDiff.y, 0, Time.fixedDeltaTime);
                float zPositionCorrection = _zAxisPIDController.GetOutput(positionDiff.z, 0, Time.fixedDeltaTime);
                Vector3 forceCorrection = new Vector3(xPositionCorrection, yPositionCorrection, zPositionCorrection);
                
                _rb.AddForce(forceCorrection);
                
                // TODO add rotation for some more interesting movement?
            }
            else
            {
                //Get the required rotation based on the target position - we can do this by getting the direction
                //from the current position to the target. Then use rotate towards and look rotation, to get a quaternion thingy.
                var targetPosition = Target.Value.position;
                var rotation = transform.rotation;
                Vector3 targetDirection;
                Quaternion targetRotation;
                switch (_targetMode)
                {
                    default:
                    case TargetMode.Direction:
                        targetDirection = targetPosition - transform.position;
                        Vector3 rotationDirection = Vector3.RotateTowards(transform.forward, targetDirection, 360, 0.00f);
                        targetRotation = Quaternion.LookRotation(rotationDirection);
                        break;
                    case TargetMode.MatchRotation:
                        targetDirection = Target.Value.forward;
                        targetRotation = Target.Value.rotation;
                        break;
                }

                float xTorqueCorrection = 0;
                if ((_rb.constraints & RigidbodyConstraints.FreezeRotationX) > 0)
                {
                    xTorqueCorrection = _xAxisPIDController.GetOutput(
                        rotation.eulerAngles.x, 
                        targetRotation.eulerAngles.x, 
                        Time.fixedDeltaTime);
                }
                
                float yTorqueCorrection = 0;
                if ((_rb.constraints & RigidbodyConstraints.FreezeRotationY) > 0)
                {
                    yTorqueCorrection = _yAxisPIDController.GetOutput(
                        rotation.eulerAngles.y, 
                        targetRotation.eulerAngles.y, 
                        Time.fixedDeltaTime);
                }
                
                float zTorqueCorrection = 0;
                if ((_rb.constraints & RigidbodyConstraints.FreezeRotationZ) > 0)
                {
                    zTorqueCorrection = _zAxisPIDController.GetOutput(
                        rotation.eulerAngles.z, 
                        targetRotation.eulerAngles.z, 
                        Time.fixedDeltaTime);
                }
                
                var torque = (xTorqueCorrection * Vector3.right) + (yTorqueCorrection * Vector3.up) +
                             (zTorqueCorrection * Vector3.forward);
                _rb.AddRelativeTorque(torque);
                // var sqrDistance = (targetPosition - _rb.position).sqrMagnitude;
                // var sqrDistanceFactor = Mathf.Clamp01(sqrDistance / _maxSqrDistance);
                // var thrust = _thrustBySquareDistance.Evaluate(sqrDistanceFactor) * _maxThrust;
                // _rb.AddRelativeForce((Vector3.forward) * thrust * Time.fixedDeltaTime);

                //Figure out the error for each axis
                var currentSpeed = _rb.velocity.magnitude;
                var thrustAlignment =
                    (currentSpeed > 0
                        ? Vector3.Project(_rb.velocity, targetDirection).magnitude / _rb.velocity.magnitude
                        : 1);
                
                var velocityZero = Mathf.Clamp01(1 - (_rb.velocity.magnitude / 5f));

                if (_useConstantThrust)
                {
                    _rb.AddRelativeForce((Vector3.forward) * _constantThrust, _constantThrustForceMode);
                }
                else
                {
                    int thrustPrediction = 5;
                    var remainingDistance = targetDirection.magnitude;
                    float factor = 1;
                    if (currentSpeed > 0)
                    {
                        float remainingFramesAtCurrentSpeed = remainingDistance / currentSpeed;
                        factor = thrustPrediction - remainingFramesAtCurrentSpeed;
                    }
                    float thrustCorrection = _thrustPIDController.GetOutput(
                        0, 
                        factor * (thrustAlignment + velocityZero), 
                        Time.fixedDeltaTime
                    );
                    
                    _rb.AddRelativeForce((Vector3.forward) * thrustCorrection);
                    Debug.Log($"(Thrust correction {thrustCorrection}) (Factor {factor}) (Alignment {thrustAlignment}) (Velocity zero {velocityZero})");
                }
                
            }
            
        }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            
            if (!_isMatchPositionMode)
            {
                Shapes.Draw.Line(position, position + transform.forward, Color.blue);
            }

            if (Target.Value != null)
            {
                var targetPosition = Target.Value.transform.position;
                var targetDirection = targetPosition - position;
                Shapes.Draw.Line(position, position + targetDirection.normalized, Color.yellow);
            }
        }

        #endregion
    }
}