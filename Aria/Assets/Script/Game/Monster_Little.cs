using UnityEngine;
using System.Collections;
using System;

public class Monster_Little : Monster {

    public MonsterCollisionManager collisionAround;

    public float timeGoingUp;
    public ParticleSystem explosionBlood;

    protected bool _goUp;
    protected bool _attack;
    protected bool _followPlayer;
    protected GameObject player;
    protected int _animRasante;
    protected Vector3 _origin;
    protected int _timePatrol;

    public override void ManualStart(AirCraft p_playerCS)
    {
        base.ManualStart(p_playerCS);
        float __min = maxSpeed - (maxSpeed * 0.6f);
        float __max = maxSpeed + (maxSpeed * 0.2f);
        maxSpeed = UnityEngine.Random.Range(__min, __max);
        _anim = body.GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _anim.enabled = false;
        _animRasante = Animator.StringToHash("Rasante");
        collisionAround.onTrigger += delegate (GameObject p_player)
        {
            if (!_attack)
            {
                AirCraft __aircraft = p_player.transform.root.GetComponent<AirCraft>();
                _rigidbody.velocity = Vector3.zero;
                player = __aircraft.airCraftBody;
                _goUp = false;
                _currentSpeed = 0f;
                _followPlayer = false;
                _attack = true;
                transform.LookAt(player.transform);
                _anim.SetBool(_animRasante, true);
                //_rigidbody.AddForce((player.transform.position - transform.position) * 300f, ForceMode.Impulse);
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
        if (_attack)
        {
            if (_currentSpeed < maxSpeed)
            {
                _currentSpeed += acceletaion;
            }
            transform.LookAt(player.transform);
            _rigidbody.velocity = (player.transform.position - transform.position) * _currentSpeed;
        }
        else if (_followPlayer)
        {
            if (_currentSpeed < maxSpeed/10)
            {
                _currentSpeed += acceletaion;
            }
            transform.LookAt(player.transform);
            _rigidbody.velocity = (player.transform.position - transform.position) * _currentSpeed;
        }
	}

    public override void Damage(string p_point, float p_damage, float p_time, Bullet p_bullet, Vector3 p_pos)
    {
        base.Damage(p_point, p_time, p_damage, p_bullet, p_pos);
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
            Invoke("Disable", explosionBlood.duration);
            explosionBlood.Play();
            body.SetActive(false);
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
        _attack = false;
        _hasAttacked = false;
        OnDisableMonster();
        explosionBlood.Stop();
        body.SetActive(true);
        _wasThrow = false;
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
        Invoke("Disable", explosionBlood.duration);
        explosionBlood.Play();
        body.SetActive(false);
    }

    public override void GoHome(Vector3 p_position)
    {
        base.GoHome(p_position);
        CancelInvoke("InvokeGoHome");
        CancelInvoke("GoPatrol");
        if (!_attack)
        {
            Invoke("GoPatrol", _timePatrol);
            _rigidbody.velocity = Vector3.zero;
            _anim.SetBool(_animRasante, false);
            transform.LookAt(p_position);
            Vector3 __distance = p_position - transform.position;
            float __mag = __distance.magnitude;
            if (__mag == 0)
                __mag = 0.01f;
            Vector3 __dir = __distance / __mag;
            _rigidbody.velocity = (__dir) * maxSpeed;
        }
    }

    public override void InvokeGoHome()
    {
        base.InvokeGoHome();
        _inPatrol = false;
        GoHome(_origin);
    }

    public void GoPatrol()
    {
        if (!_attack)
        {
            _inPatrol = true;
            _rigidbody.velocity = Vector3.zero;
            _timePatrol = UnityEngine.Random.Range(5, 10);
            Invoke("InvokeGoHome", _timePatrol);
            Vector3 __pos = _origin + (UnityEngine.Random.insideUnitSphere * 50);
            transform.LookAt(__pos);
            Vector3 __distance = transform.position - __pos;
            float __mag = __distance.magnitude;
            Vector3 __dir = __distance / __mag;
            _rigidbody.velocity = (__dir) * maxSpeed;
        }

    }

    public override void OnDisableMonster()
    {
        base.OnDisableMonster();
    }

    public override void FollowPlayer(GameObject p_player)
    {
        base.FollowPlayer(p_player);
        if (!_attack)
        {
            _currentSpeed = 0f;
            _followPlayer = true;
            player = p_player;
        }
    }

    public override void ThrowMonster(Vector3 p_pos, float p_radius)
    {
        base.ThrowMonster(p_pos, p_radius);
        _attack = false;
        _wasThrow = true;
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.AddExplosionForce(maxSpeed, p_pos, p_radius);
        Invoke("ThrowInvoke", 1f);
    }

    private void ThrowInvoke()
    {
        Invoke("Disable", explosionBlood.duration);
        explosionBlood.Play();
        body.SetActive(false);
    }
}
