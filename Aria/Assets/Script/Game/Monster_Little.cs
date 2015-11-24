using UnityEngine;
using System.Collections;
using System;

public class Monster_Little : Monster {

    public MonsterCollisionManager collisionAround;

    public float timeGoingUp;

    protected bool _goUp;
    protected bool _attack;
    protected GameObject player;
    protected int _animRasante;
    protected Vector3 _origin;
    protected int _timePatrol;

    public override void ManualStart(AirCraft p_playerCS)
    {
        base.ManualStart(p_playerCS);
        _anim = body.GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _anim.enabled = false;
        _animRasante = Animator.StringToHash("Rasante");
        collisionAround.onTrigger += delegate (GameObject p_player)
        {
            if (!_attack)
            {
                _rigidbody.velocity = Vector3.zero;
                player = p_player;
                _goUp = false;
                _currentSpeed = 0f;
                _attack = true;
                transform.LookAt(player.transform);
                _anim.SetBool(_animRasante, true); _rigidbody.AddForce((player.transform.position - transform.position) * 200f, ForceMode.Impulse);
            }
        };
        collisionAround.onExitArea += delegate
        {
            if (_attack)
            {
                _currentSpeed = 0f;
                _attack = false;
                _anim.SetBool(_animRasante, false);
                transform.LookAt(player.transform);
            }
        };
        this.enabled = false;
    }
    
    void Update()
    {

	}

    public override void Damage(string p_point, float p_damage, float p_time, Bullet p_bullet)
    {
        base.Damage(p_point, p_time, p_damage, p_bullet);
        StartCoroutine(ApplyDamage(p_point, p_damage, p_time, p_bullet));

    }

    IEnumerator ApplyDamage(string p_point, float p_damage, float p_time, Bullet p_bullet)
    {
        yield return new WaitForSeconds(p_time);
        if (p_bullet != null)
        p_bullet.Disable();
        energy -= p_damage * 1f;
        if (energy <= 0)
        {
            Debug.Log("isDed");
            Disable();
        }
    }

    public override void Enable()
    {
        base.Enable();
        _rigidbody.velocity = Vector3.zero;
        _goUp = true;
        Invoke("StopGoUp", timeGoingUp);
        _anim.enabled = true;
        _rigidbody.AddForce(transform.up * 1000f, ForceMode.Impulse);
    }

    public override void Disable()
    {
        base.Disable();
        energy = 5f;
        _currentSpeed = 0f;
        transform.position = new Vector3(-1500, -1500, -1500);
        _rigidbody.velocity = Vector3.zero;
        _anim.enabled = false;
        this.enabled = false;
        _hasAttacked = false;
        OnDisableMonster();
    }

    private void StopGoUp()
    {
        _currentSpeed = 0f;
        _goUp = false;
        _rigidbody.velocity = Vector3.zero;
        _origin = transform.position;
        GoPatrol();
    }

    public override void HitOnPlayer()
    {
        base.HitOnPlayer();
        Disable();
    }

    public void GoHome()
    {
        if (!_attack)
        {
            Invoke("GoPatrol", _timePatrol);
            _rigidbody.velocity = Vector3.zero;
            _anim.SetBool(_animRasante, false);
            transform.LookAt(_origin);
            Vector3 __distance = _origin - transform.position;
            float __mag = __distance.magnitude;
            Vector3 __dir = __distance / __mag;
            _rigidbody.velocity = (__dir) * maxSpeed;
        }
    }

    public void GoPatrol()
    {
        if (!_attack)
        {
            _rigidbody.velocity = Vector3.zero;
            _timePatrol = UnityEngine.Random.Range(5, 10);
            Invoke("GoHome", _timePatrol);
            Vector3 __pos = _origin + (UnityEngine.Random.insideUnitSphere * 100);
            transform.LookAt(__pos);
            Vector3 __distance = __pos - transform.position;
            float __mag = __distance.magnitude;
            Vector3 __dir = __distance / __mag;
            _rigidbody.velocity = (__dir) * maxSpeed;
        }

    }

    public override void OnDisableMonster()
    {
        base.OnDisableMonster();
    }
}
