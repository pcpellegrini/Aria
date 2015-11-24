using UnityEngine;
using System.Collections;

public class Monster_Aerial : Monster {
    
    public MonsterCollisionManager collisionFront;
    public MonsterCollisionManager collisionAround;
    public MonsterCollisionManager collisionBody;
    public Transform spitPosition;
    public MonsterSpit spit;

    private MonsterCollisionManager.type _currentArea;
    private bool _goWalk;
    private bool _walking;
    private bool _grabing;
    private bool _inRasanteAttack;
    private bool _inSpitAttack;
    private bool _inRoarAttack;
    private int _animGrab;
    private int _animRoar;
    private int _animSpit;
    private int _animPlan;
    private int _animDeath;
    private GameObject _player;
    private AirCraft _playerCS;

    // Use this for initialization
    public override void ManualStart(AirCraft p_playerCS)
    {
        base.ManualStart(p_playerCS);
        _playerCS = p_playerCS;
        _anim = body.GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _animPlan = Animator.StringToHash("Plan");
        _animGrab = Animator.StringToHash("Grab");
        _animSpit = Animator.StringToHash("Spit");
        _animRoar = Animator.StringToHash("Roar");
        _animDeath = Animator.StringToHash("Death");
        _currentSpeed = 0;

        collisionBody.onGrabCollision += delegate (Vector3 p_point)
        {
            if (!_grabing)
            {
                _grabing = true;
                Grab(p_point);
            }
        };
        collisionFront.onTrigger += delegate (GameObject p_player)
        {
            _player = p_player;
            int __rnd = Random.Range(1, 3);
            if (__rnd > 1)
            {
                if (!_inSpitAttack && !_isDead)
                {
                    _anim.SetTrigger(_animSpit);
                    _inSpitAttack = true;
                }
            }
            else
            {
                if (!_inRasanteAttack && !_isDead)
                {
                    RasanteAttack(true);
                    _inRasanteAttack = true;
                }
            }
            _currentArea = collisionFront.positionType;
        };
        collisionFront.onExitArea += delegate
        {
            if (_currentArea == MonsterCollisionManager.type.FRONT)
                _currentArea = MonsterCollisionManager.type.AWAY;
        };

        collisionAround.onTrigger += delegate (GameObject p_player)
        {
            _player = p_player;
            if (!_inRoarAttack && !_isDead)
            {
                _anim.SetTrigger(_animRoar);
                _inRoarAttack = true;
            }
            _currentArea = collisionAround.positionType;
        };
        collisionAround.onExitArea += delegate
        {
            if (_currentArea == MonsterCollisionManager.type.AROUND)
                _currentArea = MonsterCollisionManager.type.AWAY;
            //StartCoroutine(GoToAnimation(timeBetweenAttacks, "Walk"));
            _player = null;
        };

        this.enabled = true;
        spit.ManualStart();
        //Invoke("GoHide", 10f);
    }

    void FixedUpdate()
    {
        if (_currentArea == MonsterCollisionManager.type.FRONT && _player != null && !_isDead)
        {
            transform.LookAt(_player.transform);
        }
    }

    public override void AnimationEvent(string p_event)
    {
        base.AnimationEvent(p_event);
        switch (p_event)
        {
            case "EndSpit":
                _inSpitAttack = false;
                if (_currentArea == MonsterCollisionManager.type.FRONT)
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Spit"));
                else
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Roar"));
                break;
            case "Spit":
                if (_player != null)
                {
                    Vector3 __dir = (_player.transform.position - spitPosition.position) * 15f;
                    spit.Enable(spitPosition.position, __dir);
                }
                break;
            case "EndRoar":
                _inRoarAttack = false;
                if (_currentArea == MonsterCollisionManager.type.AROUND)
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Roar"));
                else
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Fly"));
                break;
            case "RoarDamage":
                if (_player != null)
                {
                    _playerCS.DecreaseEnergy(roarDamage);
                }
                break;
        }
    }

    public override void Damage(string p_point, float p_damage, float p_time, Bullet p_bullet)
    {
        base.Damage(p_point, p_time, p_damage, p_bullet);
        if (!_isDead)
            StartCoroutine(ApplyDamage(p_point, p_damage, p_time, p_bullet));

    }

    IEnumerator ApplyDamage(string p_point, float p_damage, float p_time, Bullet p_bullet)
    {
        yield return new WaitForSeconds(p_time);
        if (p_bullet != null)
            p_bullet.Disable();
        energy -= p_damage * 1f;
        if (energy <= 0f)
        {
            _anim.SetTrigger(_animDeath);
            _rigidbody.useGravity = true;
            _isDead = true;
        }
        base.LifeChange();
    }

    private void CancelRasante()
    {
        RasanteAttack(false);
    }

    private void RasanteAttack(bool p_value)
    {
        if (p_value)
        {
            Vector3 __dir = _player.transform.position - transform.position;
            _rigidbody.AddForce(__dir * 1000f, ForceMode.Impulse);
            _anim.SetBool(_animPlan, true);
            Invoke("CancelRasante", 3f);
        }
        else
        {
            Quaternion __rot = transform.rotation;
            __rot.eulerAngles = new Vector3(0f, __rot.eulerAngles.y, __rot.eulerAngles.z);
            transform.rotation = __rot;
            _inRasanteAttack = false;
            _rigidbody.velocity = Vector3.zero;
            _anim.SetBool(_animPlan, false);
        }
    }

    IEnumerator GoToAnimation(float p_time, string p_anim)
    {
        yield return new WaitForSeconds(p_time);
        switch (p_anim)
        {
            case "Roar":
                if (!_inRoarAttack && !_inSpitAttack && !_inRasanteAttack && !_walking && _currentArea != MonsterCollisionManager.type.AWAY)
                {
                    _inRoarAttack = true;
                    _anim.SetTrigger(_animRoar);
                }
                break;
            case "Spit":
                if (!_inRoarAttack && !_inSpitAttack && !_inRasanteAttack && !_walking)
                {
                    _inSpitAttack = true;
                    _anim.SetTrigger(_animSpit);
                }
                break;
        }
    }

    private void Grab(Vector3 p_point)
    {
        _rigidbody.velocity = Vector3.zero;
        _anim.SetBool(_animGrab, true);
    }

    private void GoHide()
    {
        float __dist = Mathf.Infinity;
        int __idx = 0;
        for (int i = 0; i < islands.Length; i++ )
        {
            int __num = i;
            float __tmpDist = Vector3.Distance(islands[__num].position, transform.position);
            if (__tmpDist < __dist)
            {
                __dist = __tmpDist;
                __idx = __num;
            }
        }
        transform.LookAt(islands[__idx]);
        Vector3 __distance = islands[__idx].position - transform.position;
        float __mag = __distance.magnitude;
        Vector3 __dir = __distance / __mag;
        _rigidbody.velocity = (__dir) * maxSpeed;
    }
}
