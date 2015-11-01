using UnityEngine;
using System.Collections;

public class AirCraft : MonoBehaviour {

    public enum  type
    {
        AIRCRAFT_1,
        AIRCRAFT_2,
        AIRCRAFT_3
    }

    public type currentType;
    public float energy = 100f;
    public float specialWeaponDamage = 100f;
    public float specialWeaponSpeed = 0.08f;
    public int specialWeaponAmmo = 3;
    public float normalWeaponDamage = 1f;
    public float normalWeaponSpeed = 0.01f;
    public float normalWeaponTime = 0.1f;
    public float normalWeaponHeatTime = 3f;
    public float normalWeaponRepairTime = 2f;
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

    public AudioSource boostSource;
    public AudioSource specialWeaponSource;
    public AudioSource normalWeaponSource;
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
    protected int _animIDBarrelLeft;
    protected int _animIDBarrelRight;
    protected int _animIDAcc;
    protected int _animIDBoost;
    protected int _animIDShake;
    protected int _animIDShakeInside;
    protected CameraContoller.cameraType _currentCamera;
    protected AircraftCollisionManager _collisionManager;
    protected Flight _flightController;
    protected GameObject _specialBullet;
    protected GameObject _instPanel;
    protected GuiManager _guiManager;
    protected GameObject _aimHUD;
    protected AudioSource _audioSource;
    protected Rigidbody _rigidbody;
    protected Animator _anim;
    protected Animator _mainAnim;

    public virtual void ManualStart (GameObject p_bullet, GameObject p_panel, GuiManager p_gui, GameObject p_aim) {

        _specialBullet = p_bullet;
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
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _anim = airCraftBody.GetComponent<Animator>();
        _mainAnim = GetComponent<Animator>();
        _flightController = GetComponent<Flight>();
        _collisionManager = GetComponent<AircraftCollisionManager>();


        _animIDBarrelLeft = Animator.StringToHash("BarrelRollLeft");
        _animIDBarrelRight = Animator.StringToHash("BarrelRollRight");
        _animIDShake = Animator.StringToHash("Shake");
        _animIDShakeInside = Animator.StringToHash("ShakeInside");
        
        if (currentType == type.AIRCRAFT_1)
        {
            _animIDAcc = Animator.StringToHash("Accelerate");
            _animIDBoost = Animator.StringToHash("Boost");
        }
        if (currentType == type.AIRCRAFT_1)
            _anim.SetBool(_animIDAcc, true);
        _currentCamera = CameraContoller.cameraType.FIRST_PERSON_VISION;
        _collisionManager.ManualStart();
        _collisionManager.onHitGround += delegate
        {
            DecreaseEnergy(5f);
            _flightController.ApplyImpactForce();
            if (_currentCamera == CameraContoller.cameraType.FIRST_PERSON_VISION)
            {
                _anim.SetTrigger(_animIDShakeInside);
                Invoke("CancelAnim", 0.5f);
            }
            else
            {
                //_mainAnim.enabled = true;
                _mainAnim.SetTrigger(_animIDShake);
                Invoke("CancelAnim", 0.5f);
            }
        };
        _collisionManager.onHitStaticObject += delegate
        {
            DecreaseEnergy(5f);
            _flightController.ApplyImpactForce();
            if (_currentCamera == CameraContoller.cameraType.FIRST_PERSON_VISION)
            {
                _anim.SetTrigger(_animIDShakeInside);
                Invoke("CancelAnim", 0.5f);
            }
            else
            {
                //_mainAnim.enabled = true;
                _mainAnim.SetTrigger(_animIDShake);
                Invoke("CancelAnim", 0.5f);
            }
        };
        _collisionManager.onHitEnemy += delegate
        {
            DecreaseEnergy(10f);
            _flightController.ApplyImpactForce();
            if (_currentCamera == CameraContoller.cameraType.FIRST_PERSON_VISION)
            {
                _anim.SetTrigger(_animIDShakeInside);
                Invoke("CancelAnim", 0.5f);
            }
            else
            {
                //_mainAnim.enabled = true;
                _mainAnim.SetTrigger(_animIDShake);
                Invoke("CancelAnim", 0.5f);
            }
        };
    }

    public virtual void DecreaseEnergy(float p_value)
    {
        energy -= p_value;
        _guiManager.ChangeValue("energy", energy);
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

        // Boost
        if (__accelerator > 0f)
        {
            if (currentType == type.AIRCRAFT_1)
                _anim.SetBool(_animIDBoost, true);
            _flightController.acceleratorValue = __accelerator;
            _flightController.accelerating = true;
            boostSource.Play();
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
        else if (_flightController.accelerating)
        {
            if (currentType == type.AIRCRAFT_1)
                _anim.SetBool(_animIDBoost, false);
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
        if (__breaker > 0f)
        {
            if (currentType == type.AIRCRAFT_1)
                _anim.SetBool(_animIDAcc, false);
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
            _flightController.breakValue = __breaker;
            _flightController.breaking = true;
        }
        else if (_flightController.breaking)
        {
            if (currentType == type.AIRCRAFT_1)
                _anim.SetBool(_animIDAcc, true);
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
            _flightController.rotating = true;
        else if (_flightController.rotating && _flightController.rotationDirection == 0f)
            _flightController.rotating = false;

        if (_flightController.pitchDirection != 0f && !_flightController.pitching)
            _flightController.pitching = true;
        else if (_flightController.pitching && _flightController.pitchDirection == 0f)
            _flightController.pitching = false;

        //Pause Game
        if (Input.GetButtonDown("Pause"))
        {
            Debug.Log("ok");
            if (_inGame)
            {
                _flightController.inGame = false;
                _inGame = false;
                Time.timeScale = 0f;
                _instPanel.SetActive(true);
            }
            else
            {
                _flightController.inGame = true;
                _inGame = true;
                Time.timeScale = 1f;
                _instPanel.SetActive(false);
            }
        }

        // Cameras
        if (Input.GetButtonDown("Camera"))
        {
            if (_currentCamera == CameraContoller.cameraType.FIRST_PERSON_VISION)
            {
                _currentCamera = CameraContoller.cameraType.THIRD_PERSON_VSION;
                _flightController.camera_TPV.enabled = true;
                _flightController.camera_FPV.enabled = false;
                _audioSource.pitch = 1;
                boostSource.pitch = 1;
                normalWeaponSource.pitch = 1;
                specialWeaponSource.pitch = 1;
                //EnableExternal(true);
            }
            else
            {
                _currentCamera = CameraContoller.cameraType.FIRST_PERSON_VISION;
                _flightController.camera_FPV.enabled = true;
                _flightController.camera_TPV.enabled = false;
                _audioSource.pitch = 0.5f;
                boostSource.pitch = 0.5f;
                normalWeaponSource.pitch = 0.8f;
                specialWeaponSource.pitch = 0.8f;
                //EnableExternal(false);
            }
        }

        // Camera Back
        if (Input.GetButtonDown("Camera Back"))
        {
            _flightController.camera_TPVBack.enabled = true;
            _flightController.camera_TPV.enabled = false;
            _flightController.camera_FPV.enabled = false;
            _aimHUD.SetActive(false);
        }
        if (Input.GetButtonUp("Camera Back"))
        {
            _flightController.camera_TPVBack.enabled = false;
            _aimHUD.SetActive(true);
            if (_currentCamera == CameraContoller.cameraType.FIRST_PERSON_VISION)
            {
                _flightController.camera_TPV.enabled = false;
                _flightController.camera_FPV.enabled = true;
            }
            else
            {
                _flightController.camera_FPV.enabled = false;
                _flightController.camera_TPV.enabled = true;
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

    public virtual void ChangeNormalWeaponSound()
    {
        int __randSound = Random.Range(0, normalWeaponSound.Length);
        normalWeaponSource.clip = normalWeaponSound[__randSound];
        normalWeaponSource.Play();
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
                GameObject __tmpBullet = Instantiate(_specialBullet, fireExit.position, Quaternion.identity) as GameObject;
                Bullet __bulletCreated = __tmpBullet.GetComponent<Bullet>();
                __bulletCreated.ManualStart();
                __bulletCreated.bulletRigidbody.AddForce((((_flightController.currentSpeed / _rigidbody.mass) + 10f) * _rigidbody.transform.forward), ForceMode.Impulse);
                FireAction(specialWeaponDamage, specialWeaponSpeed, __tmpBullet);
                if (currentType != type.AIRCRAFT_1)
                {
                    GameObject __tmpBullet2 = Instantiate(_specialBullet, fireExitR.position, Quaternion.identity) as GameObject;
                    Bullet __bulletCreated2 = __tmpBullet2.GetComponent<Bullet>();
                    __bulletCreated2.ManualStart();
                    __bulletCreated2.bulletRigidbody.AddForce((((_flightController.currentSpeed / _rigidbody.mass) + 10f) * _rigidbody.transform.forward), ForceMode.Impulse);
                    FireAction(specialWeaponDamage, specialWeaponSpeed, __tmpBullet2);
                }
                break;
        }
    }

    public virtual void FireAction(float p_damage, float p_speed, GameObject p_bullet)
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
