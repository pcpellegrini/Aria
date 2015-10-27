using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Flight : MonoBehaviour
{

    public float breakSpeed = 1500f;
    public float constSpeed = 6000f;
    public float maxSpeed = 1200f;
    public float acceleration = 50f;
    public float rotationRollSpeed = 600.0f;
    public float rotationYawSpeed = 150.0f;
    public float rotationPitchSpeed = 150.0f;
    public Camera camera_TPV;
    public Camera camera_FPV;
    public Camera camera_TPVBack;
    public Transform centerOfMass;
    public CameraContoller cameraTPVController;

    private bool _rotating;
    public bool rotating
    {
        get { return _rotating; }
        set { _rotating = value; }
    }
    private bool _inGame;
    public bool inGame
    {
        get { return _inGame; }
        set { _inGame = value; }
    }
    private bool _pitching;
    public bool pitching
    {
        get { return _pitching; }
        set { _pitching = value; }
    }
    private bool _accelerating;
    public bool accelerating
    {
        get { return _accelerating; }
        set { _accelerating = value; }
    }
    private bool _breaking;
    public bool breaking
    {
        get { return _breaking; }
        set { _breaking = value; }
    }
    private bool _lockRotation;
    public bool lockRotation
    {
        get { return _lockRotation; }
        set { _lockRotation = value; }
    }
    private bool _barrelRollActive;
    public bool barrelRollActive
    {
        get { return _barrelRollActive; }
        set { _barrelRollActive = value; }
    }
    private float _rotationDirection;
    public float rotationDirection
    {
        get { return _rotationDirection; }
        set { _rotationDirection = value; }
    }
    private float _pitchDirection;
    public float pitchDirection
    {
        get { return _pitchDirection; }
        set { _pitchDirection = value; }
    }
    private float _currentSpeed;
    public float currentSpeed
    {
        get { return _currentSpeed; }
        set { _currentSpeed = value; }
    }
    private float _acceleratorValue;
    public float acceleratorValue
    {
        get { return _acceleratorValue; }
        set { _acceleratorValue = value; }
    }
    private float _breakValue;
    public float breakValue
    {
        get { return _breakValue; }
        set { _breakValue = value; }
    }

    private bool _applyingForce;
    private float _roll;
    private float _pitch;
    private float _yaw;
    private float _timeDT;
    private float _rollAdditive;
    private float _yawAdditive;
    private float _pitchAdditive;
    private float _rollLimitMax = 150f;
    private float _rollLimitMin = -150f;
    private Rigidbody _rigidbody;
    private Quaternion _AddRotation;
    private Quaternion _AddRotationCameraTPV;
    private Vector3 _AddPos;
    

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.centerOfMass = centerOfMass.position;
        AccelerationControl();
        _inGame = true;
        
    }

    void FixedUpdate()
    {
        _timeDT = Time.deltaTime;

        if (!_applyingForce && _inGame )//&& !_barrelRollActive)
        {
            RotationContol();
            AccelerationControl();
        }
    }

    private void AccelerationControl()
    {
        if (_accelerating && _currentSpeed < maxSpeed)
        {
            _currentSpeed += acceleration * _acceleratorValue;
            if (camera_TPV.enabled)
                cameraTPVController.ChangePosition("z", acceleration * 0.1f);
        }
        else if(_breaking && _currentSpeed > breakSpeed)
        {
            if (!cameraTPVController.normalizeZPosition && cameraTPVController.NeedsNormalize("z"))
                cameraTPVController.normalizeZPosition = true;
            _currentSpeed -= acceleration * _breakValue;
        }
        else if (!_accelerating && !_breaking && _currentSpeed != constSpeed)
        {
            if (!cameraTPVController.normalizeZPosition && cameraTPVController.NeedsNormalize("z"))
                cameraTPVController.normalizeZPosition = true;
            if (_currentSpeed - acceleration < constSpeed)
            {
                _currentSpeed = constSpeed;
            }
            else
            {
                _currentSpeed -= acceleration;
            }
        }

        _AddPos = Vector3.forward;
        _AddPos = _rigidbody.rotation * _AddPos;
        _rigidbody.velocity = _AddPos * (_timeDT * _currentSpeed);
    }

    private void RotationContol()
    {
        _AddRotation = Quaternion.identity;
        if (_rotating)
        {
            if ((_rollAdditive / rotationRollSpeed < _rollLimitMax && _rotationDirection > 0) || (_rollAdditive / rotationRollSpeed > _rollLimitMin && _rotationDirection < 0))
            {
                if ((_rotationDirection < 0 && _rollAdditive > 0) || (_rotationDirection > 0 && _rollAdditive < 0)) // Invert control roll faster
                    _rollAdditive += ((rotationRollSpeed * 3) * _rotationDirection);
                else
                    _rollAdditive += ((rotationRollSpeed) * _rotationDirection);
                _roll = (_timeDT * _rollAdditive);
            }
            
            _yawAdditive += (rotationYawSpeed * _rotationDirection);
            _yaw = (_timeDT * _yawAdditive);
            if (camera_TPV.enabled)
            {
                cameraTPVController.ChangePosition("x", _rotationDirection);
            }
        }
        else
        {
            if (cameraTPVController.NeedsNormalize("x") && !cameraTPVController.normalizeXPosition)
                cameraTPVController.normalizeXPosition = true;
        }
        if (_pitching)
        {
            _pitchAdditive += (rotationPitchSpeed * _pitchDirection);
            _pitch = (_timeDT * _pitchAdditive);
            if (camera_TPV.enabled)
                cameraTPVController.ChangePosition("y", _pitchDirection);
        }
        else
        {
            if (cameraTPVController.NeedsNormalize("y") && !cameraTPVController.normalizeYPosition)
                cameraTPVController.normalizeYPosition = true;
        }

        if ((!_rotating))
            NormalizeRotation();

        // Change rotation
        _AddRotation.eulerAngles = new Vector3(_pitch, _yaw, -_roll);
        _rigidbody.rotation = _AddRotation;

        // Third person camera rotation
        _AddRotationCameraTPV.eulerAngles = new Vector3(_pitch, _yaw, 0);
        camera_TPV.transform.rotation = _AddRotationCameraTPV;
    }

    private void NormalizeRotation()
    {
        if (!_lockRotation)
        {
            // Roll rotation
            if (_rollAdditive != 0f && !_rotating)
            {
                if (_rollAdditive < 0f)
                {
                    if (_rollAdditive + rotationRollSpeed > 0f)
                    {
                        _rollAdditive = 0f;
                    }
                    else
                    {
                        _rollAdditive += rotationRollSpeed;
                    }
                }
                else if (_rollAdditive > 0f)
                {
                    if (_rollAdditive - rotationRollSpeed < 0f)
                    {
                        _rollAdditive = 0f;
                    }
                    else
                    {
                        _rollAdditive -= rotationRollSpeed;
                    }
                }
            }
            _roll = (_timeDT * _rollAdditive);
        }
    }
}
