using UnityEngine;
using System.Collections;

public class AirCraft : MonoBehaviour {

    public enum  type
    {
        AIRCRAFT_01,
        AIRCRAFT_02,
        AIRCRAFT_03
    }

    public type currentType;
    public float specialWeaponDamage = 100f;
    public float specialWeaponSpeed = 0.08f;
    public float normalWeaponDamage = 1f;
    public float normalWeaponSpeed = 0.01f;
    public float normalWeaponTime = 0.1f;
    public Flight flightController;
    public GameObject airCraftBody;
    public ParticleSystem fireEmissionL;
    public ParticleSystem fireEmissionR;
    public ParticleSystem fireEmissionBoostL;
    public ParticleSystem fireEmissionBoostR;
    public GameObject[] fireEffectLeft1;
    public GameObject[] fireEffectLeft2;
    public GameObject[] fireEffectRight1;
    public GameObject[] fireEffectRight2;
    public GameObject aimHUD;
    [HideInInspector]
    public GameObject specialBullet;
    [HideInInspector]
    public GameObject instPanel; // Temp
    public Transform fireExit;
    public AudioSource boostSource;
    public AudioSource specialWeaponSource;
    public AudioSource normalWeaponSource;
    public AudioClip[] normalWeaponSound;
    public AudioClip[] specialWeaponSound;

    private bool _normalGunFiring;
    private bool _specialGunFiring;
    private bool _inGame;
    private float _timeDT;
    private int _animIDBarrelLeft;
    private int _animIDBarrelRight;
    private CameraContoller.cameraType _currentCamera;
    private AudioSource _audioSource;
    private Rigidbody _rigidbody;
    private Animator _anim;

    void Start () {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _anim = airCraftBody.GetComponent<Animator>();
        _animIDBarrelLeft = Animator.StringToHash("BarrelRollLeft");
        _animIDBarrelRight = Animator.StringToHash("BarrelRollRight");
        _currentCamera = CameraContoller.cameraType.FIRST_PERSON_VISION;
    }

    void FixedUpdate()
    {
        _timeDT = Time.deltaTime;
    }

    void Update()
    {
        InputControl();
    }

    private void InputControl() // Check Rotations control input
    {
        float __accelerator = Input.GetAxis("Accelerate");
        float __breaker = Input.GetAxis("Break");

        // Boost
        if (__accelerator > 0f)
        {
            flightController.acceleratorValue = __accelerator;
            flightController.accelerating = true;
            boostSource.Play();
            fireEmissionBoostL.enableEmission = true;
            fireEmissionBoostR.enableEmission = true;
            fireEmissionL.enableEmission = false;
            fireEmissionR.enableEmission = false;
        }
        else if (flightController.accelerating)
        {
            flightController.accelerating = false;
            flightController.acceleratorValue = 0f;
            fireEmissionBoostL.enableEmission = false;
            fireEmissionBoostR.enableEmission = false;
            fireEmissionL.enableEmission = true;
            fireEmissionR.enableEmission = true;
        }

        // Break
        if (__breaker > 0f)
        {
            fireEmissionL.startSpeed = 0f;
            fireEmissionR.startSpeed = 0f;
            flightController.breakValue = __breaker;
            flightController.breaking = true;
        }
        else if (flightController.breaking)
        {
            fireEmissionL.startSpeed = 4.6f;
            fireEmissionR.startSpeed = 4.6f;
            flightController.breaking = false;
            flightController.breakValue = 0f;
        }

        flightController.rotationDirection = Input.GetAxis("Horizontal");
        flightController.pitchDirection = Input.GetAxis("Vertical");

        if (flightController.rotationDirection != 0f && !flightController.rotating)
            flightController.rotating = true;
        else if (flightController.rotating && flightController.rotationDirection == 0f)
            flightController.rotating = false;

        if (flightController.pitchDirection != 0f && !flightController.pitching)
            flightController.pitching = true;
        else if (flightController.pitching && flightController.pitchDirection == 0f)
            flightController.pitching = false;

        //Pause Game
        if (Input.GetButtonDown("Pause"))
        {
            if (_inGame)
            {
                flightController.inGame = false;
                _inGame = false;
                Time.timeScale = 0f;
                instPanel.SetActive(true);
            }
            else
            {
                flightController.inGame = true;
                _inGame = true;
                Time.timeScale = 1f;
                instPanel.SetActive(false);
            }
        }

        // Cameras
        if (Input.GetButtonDown("Camera"))
        {
            if (_currentCamera == CameraContoller.cameraType.FIRST_PERSON_VISION)
            {
                _currentCamera = CameraContoller.cameraType.THIRD_PERSON_VSION;
                flightController.camera_TPV.enabled = true;
                flightController.camera_FPV.enabled = false;
                _audioSource.pitch = 1;
                boostSource.pitch = 1;
                normalWeaponSource.pitch = 1;
                specialWeaponSource.pitch = 1;
                //EnableExternal(true);
            }
            else
            {
                _currentCamera = CameraContoller.cameraType.FIRST_PERSON_VISION;
                flightController.camera_FPV.enabled = true;
                flightController.camera_TPV.enabled = false;
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
            flightController.camera_TPVBack.enabled = true;
            flightController.camera_TPV.enabled = false;
            flightController.camera_FPV.enabled = false;
            aimHUD.SetActive(false);
        }
        if (Input.GetButtonUp("Camera Back"))
        {
            flightController.camera_TPVBack.enabled = false;
            aimHUD.SetActive(true);
            if (_currentCamera == CameraContoller.cameraType.FIRST_PERSON_VISION)
            {
                flightController.camera_TPV.enabled = false;
                flightController.camera_FPV.enabled = true;
            }
            else
            {
                flightController.camera_FPV.enabled = false;
                flightController.camera_TPV.enabled = true;
            }
            
        }

        // Fire
        if (Input.GetButtonDown("Fire2"))
        {
            Fire("special");
            FireSound(true, "special");
        }
        if (Input.GetButtonDown("Fire1"))
        {
            _normalGunFiring = true;
            InvokeRepeating("NormalFireCaller", 0f, normalWeaponTime);
            FireSound(true, "normal");
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
            InvokeRepeating("NormalFireCaller", 0f, normalWeaponTime);
            FireSound(true, "normal");
        }
        if (Input.GetButtonUp("Fire1Joystick"))
        {
            _normalGunFiring = false;
            FireSound(false, "normal");
            CancelInvoke("NormalFireCaller");
            DisableAllFire();
        }

        // Fire Special
        if (Input.GetButtonDown("Fire2Joystick") && !_specialGunFiring)
        {
            _specialGunFiring = true;
            Fire("special");
            FireSound(true, "special");
        }
        if (Input.GetButtonUp("Fire2Joystick"))
        {
            _specialGunFiring = false;
        }

            // BarrelRoll
        if (Input.GetButtonDown("Barrel Roll Left") && !flightController.barrelRollActive)
        {
            _anim.SetTrigger(_animIDBarrelLeft);             
            flightController.barrelRollActive = true;
        }
        if (Input.GetButtonDown("Barrel Roll Right") && !flightController.barrelRollActive)
        {
            _anim.SetTrigger(_animIDBarrelRight);
            flightController.barrelRollActive = true;
        }
    }

    public void EndAnimation(string p_anim)
    {
        switch (p_anim)
        {
            case "BarrelRoll":
                flightController.barrelRollActive = false;
                break;
        }
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

    private void DisableAllFire()
    {
        for (int i = 0; i < fireEffectLeft1.Length; i++)
        {
            int __num = i;
            fireEffectLeft1[__num].SetActive(false);
        }
        for (int i = 0; i < fireEffectLeft2.Length; i++)
        {
            int __num = i;
            fireEffectLeft2[__num].SetActive(false);
        }
        for (int i = 0; i < fireEffectRight1.Length; i++)
        {
            int __num = i;
            fireEffectRight1[__num].SetActive(false);
        }
        for (int i = 0; i < fireEffectRight2.Length; i++)
        {
            int __num = i;
            fireEffectRight2[__num].SetActive(false);
        }
    }

    private void Fire(string p_type)
    {
        switch (p_type)
        {
            case "normal":
                FireAction(normalWeaponDamage, normalWeaponSpeed, null);
                DisableAllFire();
                fireEffectLeft1[Random.Range(0, fireEffectLeft1.Length)].SetActive(true);
                fireEffectLeft2[Random.Range(0, fireEffectLeft2.Length)].SetActive(true);
                fireEffectRight1[Random.Range(0, fireEffectRight1.Length)].SetActive(true);
                fireEffectRight2[Random.Range(0, fireEffectRight2.Length)].SetActive(true);
                Invoke("DisableAllFire", normalWeaponTime *0.5f);
                break;
            case "special":
                GameObject __tmpBullet = Instantiate(specialBullet, fireExit.position, Quaternion.identity) as GameObject;
                Bullet __bulletCreated = __tmpBullet.GetComponent<Bullet>();
                __bulletCreated.ManualStart();
                __bulletCreated.bulletRigidbody.AddForce((((flightController.currentSpeed / _rigidbody.mass) + 10f) * _rigidbody.transform.forward), ForceMode.Impulse);
                FireAction(specialWeaponDamage, specialWeaponSpeed, __tmpBullet);
                break;
        }
    }

    private void FireAction(float p_damage, float p_speed, GameObject p_bullet)
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

    public void EnableExternal(bool p_value)
    {
        switch(p_value)
        {
            case true:
                fireEmissionBoostL.gameObject.SetActive(true);
                fireEmissionBoostR.gameObject.SetActive(true);
                fireEmissionL.gameObject.SetActive(true);
                fireEmissionR.gameObject.SetActive(true);
                break;
            case false:
                fireEmissionBoostL.gameObject.SetActive(false);
                fireEmissionBoostR.gameObject.SetActive(false);
                fireEmissionL.gameObject.SetActive(false);
                fireEmissionR.gameObject.SetActive(false);
                break;
        }
    }
}
