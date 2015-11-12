using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirCraft : MonoBehaviour {

    public enum  type
    {
        AIRCRAFT_1,
        AIRCRAFT_2,
        AIRCRAFT_3
    }

    public type currentType;
    public float energy = 100f;
    public float armorEnergy = 10f;
    public float specialWeaponDamage = 100f;
    public float specialWeaponSpeed = 0.08f;
    public int specialWeaponAmmo = 3;
    public float normalWeaponDamage = 1f;
    public float normalWeaponSpeed = 0.01f;
    public float normalWeaponTime = 0.1f;
    public float normalWeaponHeatTime = 3f;
    public float normalWeaponRepairTime = 2f;
    public CameraContoller FPSCameraController;
    public CameraContoller TPSCameraController;
    public Camera cameraBack;
    public SoundController soundController;
    public GameObject airCraftBody;
    public ParticleSystem[] fireEmission;
    public ParticleSystem[] fireEmissionBoost;
    public Light[] lightEmission;
    public GameObject[] fireEffectLeft1;
    public GameObject[] fireEffectLeft2;
    public GameObject[] fireEffectRight1;
    public GameObject[] fireEffectRight2;
    public Transform fireExit;
    public Transform fireExitR;
    public AudioClip engineSound;
    public AudioClip boostSound;
    public AudioClip[] normalWeaponSound;
    public AudioClip[] specialWeaponSound;

    

    protected bool _normalGunFiring;
    protected bool _normalGunLocked;
    protected bool _specialGunFiring;
    protected bool _inGame;
    protected float _timeDT;
    protected float _normalWeaponHeatCount;
    protected float _initialEmissionStartSpeed;
    protected float _emissionLightNormalIntensity;
    protected float _speed;
    protected int _animIDBarrelLeft;
    protected int _animIDBarrelRight;
    protected int _animIDAcc;
    protected int _animIDBoost;
    protected int _animIDShake;
    protected CameraContoller _currentCameraController;
    protected AircraftCollisionManager _collisionManager;
    protected Flight _flightController;
    protected List<Bullet> _bullets = new List<Bullet>();
    protected GameObject _instPanel;
    protected GuiManager _guiManager;
    protected GameObject _aimHUD;
    protected AudioSource _audioSource;
    protected Rigidbody _rigidbody;
    protected Animator _anim;

    public virtual void ManualStart (List<Bullet> p_bullet, GameObject p_panel, GuiManager p_gui, GameObject p_aim, AudioSource p_musicSource, AudioClip p_musicLevel) {

        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _anim = GetComponent<Animator>();
        _flightController = GetComponent<Flight>();
        _collisionManager = GetComponent<AircraftCollisionManager>();

        _flightController.ManualStart();
        TPSCameraController.ManualStart(_rigidbody);
        FPSCameraController.ManualStart(_rigidbody);
        _currentCameraController = TPSCameraController;

        soundController.musicSound = p_musicSource;
        _bullets = p_bullet;
        _instPanel = p_panel;
        _guiManager = p_gui;
        _aimHUD = p_aim;
        _guiManager.energySlider.maxValue = energy;
        _guiManager.energySlider.value = energy;
        _guiManager.heatSlider.maxValue = normalWeaponHeatTime;
        _guiManager.heatSlider.value = _normalWeaponHeatCount;

        _instPanel.SetActive(false);

        _initialEmissionStartSpeed = fireEmission[0].startSpeed;
        _emissionLightNormalIntensity = lightEmission[0].intensity;


        _animIDBarrelLeft = Animator.StringToHash("BarrelRollLeft");
        _animIDBarrelRight = Animator.StringToHash("BarrelRollRight");
        _animIDShake = Animator.StringToHash("ShakeThird");
        
        if (currentType != type.AIRCRAFT_3)
        {
            _animIDAcc = Animator.StringToHash("Accelerate");
            _animIDBoost = Animator.StringToHash("Boost");
        }
        if (currentType == type.AIRCRAFT_1)
            _anim.SetBool(_animIDAcc, true);
        _collisionManager.ManualStart();
        _collisionManager.onHitGround += delegate
        {
            DecreaseEnergy(5f);
            _flightController.ApplyImpactForce();
                _anim.SetTrigger(_animIDShake);
                Invoke("CancelAnim", 0.5f);
        };
        _collisionManager.onHitStaticObject += delegate
        {
            DecreaseEnergy(5f);
            _flightController.ApplyImpactForce();
            _anim.SetTrigger(_animIDShake);
            Invoke("CancelAnim", 0.5f);
        };
        _collisionManager.onHitEnemy += delegate
        {
            DecreaseEnergy(10f);
            _flightController.ApplyImpactForce();
            _anim.SetTrigger(_animIDShake);
            Invoke("CancelAnim", 0.5f);
        };

        soundController.PlaySound(SoundController.source.MUSIC, p_musicLevel);
        soundController.PlaySound(SoundController.source.ENGINE, engineSound);

        // Change camera
        soundController.InsideCockpit(false);
        _inGame = true;
    }

    public virtual void DecreaseEnergy(float p_value)
    {
        if (armorEnergy > 0)
        {
            armorEnergy -= p_value;
            _guiManager.ChangeValue("armor", armorEnergy);
        }
        else
        {
            energy -= p_value;
            _guiManager.ChangeValue("energy", energy);
        }
        
    }

    public virtual void IncreaseEnergy(float p_value)
    {
        if (armorEnergy < 10)
        {
            armorEnergy += p_value;
            _guiManager.ChangeValue("armor", armorEnergy);
        }
    }

    public virtual void ManualFixedUpdate()
    {
        _timeDT = Time.deltaTime;
        if (!_normalGunFiring && _normalWeaponHeatCount > 0)
        {
            _normalWeaponHeatCount -= normalWeaponRepairTime * _timeDT;
            _guiManager.ChangeValue("heat", _normalWeaponHeatCount);
            if (_normalGunLocked && _normalWeaponHeatCount <= 0 )
            {
                _normalWeaponHeatCount = 0f;
                _normalGunLocked = false;
            }
        }
        _flightController.ManualFixedUpdate();
        _currentCameraController.ManualFixedUpdate();
        _speed = _flightController.currentSpeed;
        
            IncreaseEnergy(_timeDT);
    }

    public virtual void ManualUpdate()
    {
        InputControl();
    }

    public virtual void CancelAnim()
    {
        //_mainAnim.enabled = false;
        _flightController.applyingForce = false;
    }

    public virtual void InputControl() // Check Rotations control input
    {
        float __accelerator = Input.GetAxis("Accelerate");
        float __breaker = Input.GetAxis("Break");

        //Pause Game
        if (Input.GetButtonDown("Pause"))
        {
            if (_inGame)
            {
                UnityEngine.Cursor.visible = true;
                soundController.PauseAll(true);
                _flightController.inGame = false;
                _inGame = false;
                Time.timeScale = 0;
                _instPanel.SetActive(true);
            }
            else
            {
                UnityEngine.Cursor.visible = false;
                soundController.PauseAll(false);
                if (!_normalGunFiring)
                    soundController.StopSound(SoundController.source.NORMAL_WEAPON);
                _flightController.inGame = true;
                _inGame = true;
                Time.timeScale = 1f;
                _instPanel.SetActive(false);
            }
        }

        

        // Joystick Aim
        /* if (Input.GetAxis("MouseH") != 0 || Input.GetAxis("MouseV") != 0)
         {
             Vector2 __dir = new Vector2(Input.GetAxis("MouseH"), Input.GetAxis("MouseV"));
             _currentCameraController.aimFollowMouse = false;
             _currentCameraController._mousePos = __dir;
         }
         else
         {
             _currentCameraController._mousePos = new Vector2(0f, 0f);
         }*/

        
        if (_inGame)
        {
            // Boost
            if (__accelerator > 0f && !_flightController.accelerating)
            {
                ChangeSpeed("boost");
                _flightController.acceleratorValue = 1f;
                _flightController.accelerating = true;
                soundController.PlaySound(SoundController.source.BOOST, boostSound);
                for (int i = 0; i < lightEmission.Length; i++)
                {
                    int __num = i;
                    lightEmission[__num].color = Color.blue;
                }
                for (int i = 0; i < fireEmission.Length; i++)
                {
                    int __num = i;
                    fireEmission[__num].enableEmission = false;
                }
                for (int i = 0; i < fireEmissionBoost.Length; i++)
                {
                    int __num = i;
                    fireEmissionBoost[__num].enableEmission = true;
                }
            }
            else if (__accelerator == 0f && _flightController.accelerating)
            {
                ChangeSpeed("normal");
                soundController.StopSound(SoundController.source.BOOST);
                _flightController.accelerating = false;
                _flightController.acceleratorValue = 0f;
                for (int i = 0; i < lightEmission.Length; i++)
                {
                    int __num = i;
                    lightEmission[__num].color = Color.yellow;
                }
                for (int i = 0; i < fireEmission.Length; i++)
                {
                    int __num = i;
                    fireEmission[__num].enableEmission = true;
                }
                for (int i = 0; i < fireEmissionBoost.Length; i++)
                {
                    int __num = i;
                    fireEmissionBoost[__num].enableEmission = false;
                }
            }

            // Break
            if (__breaker > 0f && !_flightController.breaking)
            {
                ChangeSpeed("break");
                for (int i = 0; i < fireEmission.Length; i++)
                {
                    int __num = i;
                    fireEmission[__num].startSpeed = 0f;
                }
                for (int i = 0; i < lightEmission.Length; i++)
                {
                    int __num = i;
                    lightEmission[__num].intensity = 0.5f;
                }
                _flightController.breakValue = 1;
                _flightController.breaking = true;
            }
            else if (__breaker == 0f && _flightController.breaking)
            {
                ChangeSpeed("normal");
                for (int i = 0; i < fireEmission.Length; i++)
                {
                    int __num = i;
                    fireEmission[__num].startSpeed = _initialEmissionStartSpeed;
                }
                for (int i = 0; i < lightEmission.Length; i++)
                {
                    int __num = i;
                    lightEmission[__num].intensity = _emissionLightNormalIntensity;
                }
                _flightController.breaking = false;
                _flightController.breakValue = 0f;
            }

            _flightController.rotationDirection = Input.GetAxis("Horizontal");
            _flightController.pitchDirection = Input.GetAxis("Vertical");

            if (_flightController.rotationDirection != 0f && !_flightController.rotating)
            {
                _flightController.rotating = true;
            }
            else if (_flightController.rotating && _flightController.rotationDirection == 0f)
            {
                _flightController.rotating = false;
            }

            if (_currentCameraController.type == CameraContoller.cameraType.THIRD_PERSON_VSION)
                _currentCameraController.roll = _flightController.roll;

            if (_flightController.pitchDirection != 0f && !_flightController.pitching)
            {
                _flightController.pitching = true;
            }
            else if (_flightController.pitching && _flightController.pitchDirection == 0f)
            {
                _flightController.pitching = false;
            }

            // Cameras
            if (Input.GetButtonDown("Camera"))
            {
                if (_currentCameraController.type == CameraContoller.cameraType.FIRST_PERSON_VISION)
                {
                    _currentCameraController = TPSCameraController;
                    TPSCameraController.mainCam.enabled = true;
                    FPSCameraController.mainCam.enabled = false;
                    soundController.InsideCockpit(false);
                    //EnableExternal(true);
                }
                else
                {
                    _currentCameraController = FPSCameraController;
                    FPSCameraController.mainCam.enabled = true;
                    TPSCameraController.mainCam.enabled = false;
                    soundController.InsideCockpit(true);
                    //EnableExternal(false);
                }
            }

            // Camera Back
            if (Input.GetButtonDown("Camera Back"))
            {
                cameraBack.enabled = true;
                TPSCameraController.mainCam.enabled = false;
                FPSCameraController.mainCam.enabled = false;
                _aimHUD.SetActive(false);
            }
            if (Input.GetButtonUp("Camera Back"))
            {
                cameraBack.enabled = false;
                _aimHUD.SetActive(true);
                if (_currentCameraController.type == CameraContoller.cameraType.FIRST_PERSON_VISION)
                {
                    TPSCameraController.mainCam.enabled = false;
                    FPSCameraController.mainCam.enabled = true;
                }
                else
                {
                    TPSCameraController.mainCam.enabled = true;
                    FPSCameraController.mainCam.enabled = false;
                }

            }

            // Fire
            if (Input.GetButtonDown("Fire2") && specialWeaponAmmo > 0)
            {
                specialWeaponAmmo--;
                _guiManager.ChangeValue("special", specialWeaponAmmo);
                Fire("special");
                FireSound(true, "special");
            }
            if (Input.GetButtonDown("Fire1"))
            {
                _normalGunFiring = true;
                if (!_normalGunLocked)
                {
                    InvokeRepeating("NormalFireCaller", 0f, normalWeaponTime);
                    FireSound(true, "normal");
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                _normalGunFiring = false;
                FireSound(false, "normal");
                CancelInvoke("NormalFireCaller");
                DisableAllFire();
            }

            // Joystick Firing Normal
            if (Input.GetButtonDown("Fire1Joystick") && !_normalGunFiring)
            {
                _normalGunFiring = true;
                if (!_normalGunLocked)
                {
                    InvokeRepeating("NormalFireCaller", 0f, normalWeaponTime);
                    FireSound(true, "normal");
                }
            }
            if (Input.GetButtonUp("Fire1Joystick"))
            {
                _normalGunFiring = false;
                FireSound(false, "normal");
                CancelInvoke("NormalFireCaller");
                DisableAllFire();
            }

            // Fire Special
            if (Input.GetButtonDown("Fire2Joystick") && !_specialGunFiring && specialWeaponAmmo > 0)
            {
                specialWeaponAmmo--;
                _guiManager.ChangeValue("special", specialWeaponAmmo);
                _specialGunFiring = true;
                Fire("special");
                FireSound(true, "special");
            }
            if (Input.GetButtonUp("Fire2Joystick"))
            {
                _specialGunFiring = false;
            }

            // BarrelRoll
            if (Input.GetButtonDown("Barrel Roll Left") && !_flightController.barrelRollActive)
            {
                _anim.SetTrigger(_animIDBarrelLeft);
                _flightController.barrelRollActive = true;
            }
            if (Input.GetButtonDown("Barrel Roll Right") && !_flightController.barrelRollActive)
            {
                _anim.SetTrigger(_animIDBarrelRight);
                _flightController.barrelRollActive = true;
            }
        }
    }

    public virtual void ChangeSpeed(string p_state)
    {

    }

    public virtual void EndAnimation(string p_anim)
    {
        switch (p_anim)
        {
            case "BarrelRoll":
                _flightController.barrelRollActive = false;
                break;
        }
    }

    public virtual void FireSound(bool p_state, string p_type)
    {
        switch (p_type)
        {
            case "normal":
                if (p_state)
                {
                    int __rndNormal = Random.Range(0, normalWeaponSound.Length);
                    soundController.PlaySound(SoundController.source.NORMAL_WEAPON, normalWeaponSound[__rndNormal]);
                    //Invoke("ChangeNormalWeaponSound", normalWeaponSound[__randSound].length);
                }
                else
                    soundController.StopSound(SoundController.source.NORMAL_WEAPON);
                break;
            case "special":
                int __rnd = Random.Range(0, specialWeaponSound.Length);
                soundController.PlaySound(SoundController.source.SPECIAL_WEAPON, specialWeaponSound[__rnd]);
                break;
        }
    }

    public virtual void ChangeNormalWeaponSound()
    {
        int __randSound = Random.Range(0, normalWeaponSound.Length);
        soundController.PlaySound(SoundController.source.NORMAL_WEAPON, normalWeaponSound[__randSound]);
    }

    public virtual void NormalFireCaller()
    {
        if (_normalWeaponHeatCount < normalWeaponHeatTime)
        {
            _normalWeaponHeatCount += normalWeaponTime;
            _guiManager.ChangeValue("heat", _normalWeaponHeatCount);
            Fire("normal");
        }
        else
        {
            _normalGunLocked = true;
            //_normalGunFiring = false;
            FireSound(false, "normal");
            CancelInvoke("NormalFireCaller");
            DisableAllFire();
        }
    }

    public virtual void DisableAllFire()
    {
        for (int i = 0; i < fireEffectLeft1.Length; i++)
        {
            int __num = i;
            fireEffectLeft1[__num].SetActive(false);
        }
        if (currentType == type.AIRCRAFT_1)//tmp
        {
            for (int i = 0; i < fireEffectLeft2.Length; i++)
            {
                int __num = i;
                fireEffectLeft2[__num].SetActive(false);
            }
        }
        for (int i = 0; i < fireEffectRight1.Length; i++)
        {
            int __num = i;
            fireEffectRight1[__num].SetActive(false);
        }
        if (currentType == type.AIRCRAFT_1)//ymp
        {
            for (int i = 0; i < fireEffectRight2.Length; i++)
            {
                int __num = i;
                fireEffectRight2[__num].SetActive(false);
            }
        }
    }

    public virtual void Fire(string p_type)
    {
        switch (p_type)
        {
            case "normal":
                FireAction(normalWeaponDamage, normalWeaponSpeed, null);
                DisableAllFire();
                fireEffectLeft1[Random.Range(0, fireEffectLeft1.Length)].SetActive(true);
                if (currentType == type.AIRCRAFT_1)//tmp
                    fireEffectLeft2[Random.Range(0, fireEffectLeft2.Length)].SetActive(true);
                fireEffectRight1[Random.Range(0, fireEffectRight1.Length)].SetActive(true);
                if (currentType == type.AIRCRAFT_1)//tmp
                    fireEffectRight2[Random.Range(0, fireEffectRight2.Length)].SetActive(true);
                Invoke("DisableAllFire", normalWeaponTime *0.5f);
                break;
            case "special":
                Vector3 __dir = _currentCameraController.AimPosition().direction;
                for (int i = 0; i< _bullets.Count; i++)
                {
                    int __num = i;
                    if (!_bullets[__num].isActive)
                    {
                        _bullets[__num].isActive = false;
                        _bullets[__num].transform.position = fireExit.position;
                        _bullets[__num].transform.rotation = transform.rotation;
                        _bullets[__num].transform.LookAt(__dir);
                        _bullets[__num].ManualStart();
                        _bullets[__num].bulletRigidbody.AddForce((((_flightController.currentSpeed / _rigidbody.mass) + 10f) * __dir), ForceMode.Impulse);
                        FireAction(specialWeaponDamage, specialWeaponSpeed, _bullets[__num]);
                    }
                }
                if (currentType != type.AIRCRAFT_1)
                {
                    for (int i = 0; i < _bullets.Count; i++)
                    {
                        int __num = i;
                        if (!_bullets[__num].isActive)
                        {
                            _bullets[__num].isActive = false;
                            _bullets[__num].transform.position = fireExitR.position;
                            _bullets[__num].transform.rotation = transform.rotation;
                            _bullets[__num].transform.LookAt(__dir);
                            _bullets[__num].ManualStart();
                            _bullets[__num].bulletRigidbody.AddForce((((_flightController.currentSpeed / _rigidbody.mass) + 10f) * __dir), ForceMode.Impulse);
                            FireAction(specialWeaponDamage, specialWeaponSpeed, _bullets[__num]);
                        }
                    }
                }
                break;
        }
    }

    public virtual void FireAction(float p_damage, float p_speed, Bullet p_bullet)
    {
        RaycastHit __hit;
        if (Physics.Raycast(_currentCameraController.AimPosition(), out __hit, 1000))
        {

            if (__hit.collider.tag == "tower")
            {
                if (__hit.collider.GetComponent<Tower>() != null)
                {
                    Tower __tower = __hit.collider.GetComponent<Tower>();
                    float __timeToHit = __hit.distance * p_speed * _timeDT;
                    __tower.Damage(p_damage, __timeToHit, p_bullet);
                }
            }
        }
    }

    public virtual void EnableExternal(bool p_value)
    {
        switch(p_value)
        {
            case true:
                for (int i = 0; i < fireEmission.Length; i++)
                {
                    int __num = i;
                    fireEmission[__num].enableEmission = true;
                }
                for (int i = 0; i < fireEmissionBoost.Length; i++)
                {
                    int __num = i;
                    fireEmissionBoost[__num].enableEmission = true;
                }
                break;
            case false:
                for (int i = 0; i < fireEmission.Length; i++)
                {
                    int __num = i;
                    fireEmission[__num].enableEmission = false;
                }
                for (int i = 0; i < fireEmissionBoost.Length; i++)
                {
                    int __num = i;
                    fireEmissionBoost[__num].enableEmission = false;
                }
                break;
        }
    }
}
