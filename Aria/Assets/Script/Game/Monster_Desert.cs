using UnityEngine;
using System.Collections;

public class Monster_Desert : Monster {
    
    public MonsterCollisionManager collisionTail;
    public MonsterCollisionManager collisionFront;
    public MonsterCollisionManager collisionAround;
    public Transform spitPosition;
    public MonsterSpit spit;

    private MonsterCollisionManager.type _currentArea;
    private bool _goWalk;
    private bool _walking;
    private bool _inTailAttack;
    private bool _inSpitAttack;
    private bool _inRoarAttack;
    private int _animWalk;
    private int _animRoar;
    private int _animSpit;
    private int _animTail;
    private int _animDeath;
    private GameObject _player;
    private AirCraft _playerCS;

    // Use this for initialization
    public override void ManualStart (AirCraft p_playerCS) {
        base.ManualStart(p_playerCS);
        _playerCS = p_playerCS;
        _anim = body.GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _animTail = Animator.StringToHash("Tail");
        _animWalk = Animator.StringToHash("Walk");
        _animSpit = Animator.StringToHash("Spit");
        _animRoar = Animator.StringToHash("Roar");
        _animDeath = Animator.StringToHash("Death");
        _currentSpeed = 0;
        collisionTail.onTrigger += delegate (GameObject p_player)
        {
            _player = p_player;
            if (!_inTailAttack)
            {
                _anim.SetTrigger(_animTail);
                _inTailAttack = true;
            }
            _currentArea = collisionTail.positionType;
        };
        collisionTail.onExitArea += delegate
        {
            if (_currentArea == MonsterCollisionManager.type.TAIL)
                _currentArea = MonsterCollisionManager.type.AWAY;
        };

        collisionFront.onTrigger += delegate (GameObject p_player)
        {
            _player = p_player;
            if (!_inSpitAttack)
            {
                _anim.SetTrigger(_animSpit);
                _inSpitAttack = true;
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
            if (!_inSpitAttack)
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
            StartCoroutine(GoToAnimation(timeBetweenAttacks, "Walk"));
            _player = null;
        };

        this.enabled = true;
        spit.ManualStart();
    }
	
	void FixedUpdate () {
        if (_walking)
        {
            if (_currentSpeed < maxSpeed)
            {
                _currentSpeed += acceletaion;
            }
            _rigidbody.velocity = _rigidbody.transform.forward * _currentSpeed;
        }
        else if (_currentSpeed > 0)
        {
            _currentSpeed -= acceletaion*2;
            if (_currentSpeed >= 0)
            {
                _rigidbody.velocity = _rigidbody.transform.forward * _currentSpeed;
            }
            else
            {
                _currentSpeed = 0f;
                _rigidbody.velocity = Vector3.zero;
            }
        }
	}

    public override void AnimationEvent(string p_event)
    {
        base.AnimationEvent(p_event);
        switch (p_event)
        {
            case "Walk":
                _rigidbody.isKinematic = false;
                _walking = true;
                break;
            case "StopWalk":
                _walking = false;
                break;
            case "EndTail":
                _inTailAttack = false;
                if (_currentArea == MonsterCollisionManager.type.TAIL)
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Tail"));
                else
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Roar"));
                break;
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
                    Vector3 __dir = (_player.transform.position - spitPosition.position) * 10f;
                    spit.Enable(spitPosition.position, __dir);
                }
                break;
            case "EndRoar":
                _inRoarAttack = false;
                if (_currentArea == MonsterCollisionManager.type.AROUND)
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Roar"));
                else
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Walk"));
                break;
            case "EndWalk":
                _rigidbody.isKinematic = true;
                if (_currentArea == MonsterCollisionManager.type.AWAY)
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Walk"));
                else
                    StartCoroutine(GoToAnimation(timeBetweenAttacks, "Roar"));
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
        StartCoroutine(ApplyDamage(p_point, p_damage, p_time, p_bullet));
        
    }

    IEnumerator ApplyDamage(string p_point, float p_damage, float p_time, Bullet p_bullet)
    {
        yield return new WaitForSeconds(p_time);
        if (p_bullet != null)
            p_bullet.Disable();
        switch (p_point)
        {
            case "enemyHead":
                if (_inRoarAttack || _inSpitAttack)
                    energy -= p_damage*2f;
                break;
            case "enemyAntenna":
                energy -= p_damage*1.2f;
                break;
            case "enemyVulcan":
                energy -= p_damage*1f;
                break;
            case "enemyPurple":
                energy -= p_damage*1.5f;
                break;
        }
        base.LifeChange();
    }

    IEnumerator GoToAnimation(float p_time, string p_anim)
    {
        yield return new WaitForSeconds(p_time);
        switch (p_anim)
        {
            case "Walk":
                if (!_inSpitAttack && !_inTailAttack)
                {
                    _anim.SetTrigger(_animWalk);
                }
                break;
            case "Roar":
                if (!_inRoarAttack && !_inSpitAttack && !_inTailAttack && !_walking && _currentArea != MonsterCollisionManager.type.AWAY)
                {
                    _inRoarAttack = true;
                    _anim.SetTrigger(_animRoar);
                }
                break;
            case "Spit":
                if (!_inRoarAttack && !_inSpitAttack && !_inTailAttack && !_walking)
                {
                    _inSpitAttack = true;
                    _anim.SetTrigger(_animSpit);
                }
                break;
            case "Tail":
                if (!_inRoarAttack && !_inSpitAttack && !_inTailAttack && !_walking)
                {
                    _inTailAttack = true;
                    _anim.SetTrigger(_animTail);
                }
                break;
        }
    }



}
