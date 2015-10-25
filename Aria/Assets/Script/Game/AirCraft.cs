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
    public Flight flightController;
    public GameObject airCraftBody;
    [HideInInspector]
    public GameObject specialBullet;
    public Transform fireExit;
    public AudioSource boostSource;
    public AudioSource specialWeaponSource;
    public AudioSource normalWeaponSource;
    public AudioClip[] normalWeaponSound;
    public AudioClip[] specialWeaponSound;

    private bool _normalGunFiring;
    private float _timeDT;
    private int _animIDBarrelLeft;
    private int _animIDBarrelRight;
    private AudioSource _audioSource;
    private Rigidbody _rigidbody;
    private Animator _anim;

    void Start () {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _anim = airCraftBody.GetComponent<Animator>();
        _animIDBarrelLeft = Animator.StringToHash("BarrelRollLeft");
        _animIDBarrelRight = Animator.StringToHash("BarrelRollRight");
    }

    void FixedUpdate()
    {
        _timeDT = Time.deltaTime;
    }

    void Update()
    {
        InputControl();
        if (_normalGunFiring)
            Fire("normal");
    }

    private void InputControl() // Check Rotations control input
    {
        if (Input.GetButtonDown("Accelerate"))
        {
            flightController.accelerating = true;
            boostSource.Play();
        }
        if (Input.GetButtonUp("Accelerate"))
        {
            flightController.accelerating = false;
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


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Cameras
        if (Input.GetKeyDown(KeyCode.F1))
        {
            flightController.camera_FPV.enabled = true;
            flightController.camera_TPV.enabled = false;
            _audioSource.pitch = 0.5f;
            boostSource.pitch = 0.5f;
            normalWeaponSource.pitch = 0.8f;
            specialWeaponSource.pitch = 0.8f;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            flightController.camera_TPV.enabled = true;
            flightController.camera_FPV.enabled = false;
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

        // BarrelRoll
        if (Input.GetButtonDown("Barrel Roll"))
        {
            if (flightController.rotationDirection < 0f)
            {
                _anim.SetTrigger(_animIDBarrelLeft);
            }                
            else if (flightController.rotationDirection > 0f)
            {
                _anim.SetTrigger(_animIDBarrelRight);
            }
            else
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

    private void Fire(string p_type)
    {
        switch (p_type)
        {
            case "normal":
                FireAction(normalWeaponDamage, normalWeaponSpeed, null);
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
}
