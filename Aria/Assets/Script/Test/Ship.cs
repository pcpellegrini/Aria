using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Ship : MonoBehaviour {

    public float constSpeed = 2000f;
    public float maxSpeed = 8000f;
    public float acceleration = 50f;
    public float rotationRollSpeed = 600.0f;
    public float rotationYawSpeed = 150.0f;
    public float rotationPitchSpeed = 150.0f;
    public float specialWeaponDamage = 100f;
    public float specialWeaponSpeed = 0.08f;
    public float normalWeaponDamage = 1f;
    public float normalWeaponSpeed = 0.01f;

    // Temp
    public Camera camera_TPV;
    public Camera camera_FPV;
    public Transform centerOfMass;
    public Transform fireExit;
    public CameraContoller cameraTPVController;
    public List<GameObject> bullets = new List<GameObject>();
    public AudioSource boostSource;
    public AudioSource specialWeaponSource;
    public AudioSource normalWeaponSource;
    public AudioClip[] normalWeaponSound;
    public AudioClip[] specialWeaponSound;

    private Rigidbody _rigidbody;

    private float _currentSpeed;
    private float _roll;
    private float _pitch;
    private float _yaw;
    private float _timeDT;
    private float _rollAdditive;
    private float _yawAdditive;
    private float _pitchAdditive;
    private float _rotationDirection;
    private float _pitchDirection;
    private float _rollLimitMax = 150f;
    private float _rollLimitMin = -150f;

    private bool _rotating;
    private bool _pitching;
    private bool _accelerating;
    private bool _lockRotation;
    private bool _applyingForce;
    private bool _inGame;
    private bool _normalGunFiring;

    private Quaternion _AddRotation;
    private Quaternion _AddRotationCameraTPV;
    private Vector3 _AddPos;
    private AudioSource _audioSource;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _rigidbody.centerOfMass = centerOfMass.position;
        AccelerationControl();
        _inGame = true;
        _audioSource.pitch = 0.5f;
        normalWeaponSource.pitch = 0.8f;
        specialWeaponSource.pitch = 0.8f;
        boostSource.pitch = 0.5f;
    }

    void FixedUpdate()
    {
        _timeDT = Time.deltaTime;
        
        if (!_applyingForce && _inGame)
        {
            RotationContol();
            AccelerationControl();
        }
        
    }

    void Update()
    {
        InputControl();
        if (_normalGunFiring)
            Fire("normal");
    }

    private void AccelerationControl()
    {
        if (_accelerating && _currentSpeed < maxSpeed)
        {
            _currentSpeed += acceleration;
            if (camera_TPV.enabled)
                cameraTPVController.ChangePosition("z", acceleration*0.1f);
        }
        else if (!_accelerating && _currentSpeed != constSpeed)
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
            if ((_rollAdditive / rotationRollSpeed < _rollLimitMax) && (_rollAdditive / rotationRollSpeed > _rollLimitMin))
            {
                if ((_rotationDirection < 0 && _rollAdditive > 0) || (_rotationDirection > 0 && _rollAdditive < 0)) // Invert control roll faster
                    _rollAdditive += ((rotationRollSpeed * 3) * _rotationDirection);
                else
                    _rollAdditive += ((rotationRollSpeed) * _rotationDirection);
            }
            _roll = (_timeDT * _rollAdditive);
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
    private bool InputControl() // Check Rotations control input
    {
        if (Input.GetButtonDown("Accelerate"))
        {
            _accelerating = true;
            boostSource.Play();
        }
        if (Input.GetButtonUp("Accelerate"))
        {
            _accelerating = false;
        }

        _rotationDirection = Input.GetAxis("Horizontal");
        _pitchDirection = Input.GetAxis("Vertical");

        if (_rotationDirection != 0f && !_rotating)
            _rotating = true;
        else if (_rotating && _rotationDirection == 0f)
            _rotating = false;

        if (_pitchDirection != 0f && !_pitching)
            _pitching = true;
        else if (_pitching && _pitchDirection == 0f)
            _pitching = false;


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

            // Cameras
            if (Input.GetKeyDown(KeyCode.F1))
        {
            camera_FPV.enabled = true;
            camera_TPV.enabled = false;
            _audioSource.pitch = 0.5f;
            boostSource.pitch = 0.5f;
            normalWeaponSource.pitch = 0.8f;
            specialWeaponSource.pitch = 0.8f;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            camera_TPV.enabled = true;
            camera_FPV.enabled = false;
            _audioSource.pitch = 1;
            boostSource.pitch = 1;
            normalWeaponSource.pitch = 1;
            specialWeaponSource.pitch = 1;
        }

        // Fire
        if (Input.GetButtonDown("Fire2"))
        {
            Fire("special");
            FireSound(true, "special");
        }
        if (Input.GetButtonDown("Fire1"))
        {
            //_normalGunFiring = true;
            InvokeRepeating("NormalFireCaller", 0f, 0.1f);
            FireSound(true, "normal");
        }

        if (Input.GetButtonUp("Fire1"))
        {
            _normalGunFiring = false;
            FireSound(false, "normal");
            CancelInvoke("NormalFireCaller");
        }

        if (_pitching || _rotating)
            return true;
        else
            return false;
    }

    private void FireSound(bool p_state, string p_type)
    {
        switch (p_type)
        {
            case "normal":
                if (p_state)
                {
                    int __randSound = Random.Range(0, normalWeaponSound.Length);
                    normalWeaponSource.clip = normalWeaponSound[__randSound];
                    normalWeaponSource.Play();
                    //Invoke("ChangeNormalWeaponSound", normalWeaponSound[__randSound].length);
                }
                else
                    normalWeaponSource.Stop();
                break;
            case "special":
                int __randSound2 = Random.Range(0, specialWeaponSound.Length);
                specialWeaponSource.clip = specialWeaponSound[__randSound2];
                specialWeaponSource.Play();
                break;
        }
    }

    private void ChangeNormalWeaponSound()
    {
        int __randSound = Random.Range(0, normalWeaponSound.Length);
        normalWeaponSource.clip = normalWeaponSound[__randSound];
        normalWeaponSource.Play();
    }

    private void NormalFireCaller()
    {
        Fire("normal");
    }

    private void Fire(string p_type)
    {
        switch(p_type)
        {
            case "normal":
                FireAction(normalWeaponDamage, normalWeaponSpeed);
                break;
            case "special":
                for (int i = 0; i < bullets.Count; i++)
                {
                    int __idx = i;
                    Bullet __bullet = bullets[__idx].GetComponent<Bullet>();
                    if (!__bullet.isActive)
                    {
                        Bullet __bulletCreated = __bullet;
                        __bullet.Enable(fireExit.position);
                        __bulletCreated.bulletRigidbody.AddForce((((_currentSpeed / _rigidbody.mass) + 10f) * _rigidbody.transform.forward), ForceMode.Impulse);
                        FireAction(specialWeaponDamage, specialWeaponSpeed);
                        break;
                    }
                }
                break;
        }
    }

    private void FireAction(float p_damage, float p_speed)
    {
        RaycastHit __hit;
        if (Physics.Raycast(_rigidbody.position, _rigidbody.transform.forward, out __hit, 1000))
        {

            if (__hit.collider.tag == "tower")
            {
                if (__hit.collider.GetComponent<Tower>() != null)
                {
                    Tower __tower = __hit.collider.GetComponent<Tower>();
                    float __timeToHit = __hit.distance * p_speed * _timeDT;
                    __tower.Damage(p_damage, __timeToHit);
                }
            }
        }
    }

    public void ApplyContactForce()
    {
        Vector3 __rotationVector = _rigidbody.rotation.eulerAngles;
        __rotationVector.y = -__rotationVector.y;
        _rigidbody.rotation = Quaternion.Euler(__rotationVector);

        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, -_rigidbody.velocity.y, _rigidbody.velocity.z);
        //_applyingForce = false;
    }    
    void OnCollisionEnter(Collision p_collision)
    {
        if (p_collision.gameObject.tag == "terrain")
        {
           // _applyingForce = true;
           // ApplyContactForce();
        }
    }
}
