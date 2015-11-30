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

        for (int i = 0; i < 200; i++)
        {
            int __num = i;
            _blood.Add(Instantiate(hitBloodPrefab, new Vector3(-2000, -2000, -2000), Quaternion.identity) as GameObject);
            _bloodParticle.Add(_blood[__num].GetComponent<ParticleSystem>());
            _bloodParticle[__num].Stop();
            _bloodParticleFree.Add(_bloodParticle[__num]);
        }
        collisionTail.onTrigger += delegate (GameObject p_player)
        {
            _player = p_player;
            if (!_inTailAttack && !_isDead)
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
            if (!_inSpitAttack && !_isDead)
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
            if (!_isDead)
            {
                PlayerOnMonsterZone(true);
                _player = p_player;
                if (!_inSpitAttack)
                {
                    _anim.SetTrigger(_animRoar);
                    _inRoarAttack = true;
                }
                _currentArea = collisionAround.positionType;
            }
        };
        collisionAround.onExitArea += delegate
        {
            if (!_isDead)
            {
                PlayerOnMonsterZone(false);
                if (_currentArea == MonsterCollisionManager.type.AROUND)
                    _currentArea = MonsterCollisionManager.type.AWAY;
                StartCoroutine(GoToAnimation(timeBetweenAttacks, "Walk"));
                _player = null;
            }
        };

        this.enabled = true;
        spit.ManualStart();
    }
	
	void FixedUpdate () {
        if (_isDead)
        {
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
                _currentSpeed -= acceletaion * 2;
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
	}

    public override void AnimationEvent(string p_event)
    {
        base.AnimationEvent(p_event);
        if (!_isDead)
        {
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
                    if (_player != null)//&& _currentArea == MonsterCollisionManager.type.FRONT)
                    {
                        spit.Enable(spitPosition.position, _playerCS.airCraftBody);
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
    }

    public override void Damage(string p_point, float p_damage, float p_time, Bullet p_bullet, Vector3 p_pos)
    {
        base.Damage(p_point, p_time, p_damage, p_bullet, p_pos);
        if (!_isDead)
            StartCoroutine(ApplyDamage(p_point, p_damage, p_time, p_bullet, p_pos));
        
    }

    IEnumerator ApplyDamage(string p_point, float p_damage, float p_time, Bullet p_bullet, Vector3 p_pos)
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
        if (energy <= 0f)
        {
            _anim.SetTrigger(_animDeath);
            _isDead = true;
            this.enabled = false;
        }
        if (_bloodParticleFree.Count > 0)
        {
            _bloodParticleUsed.Add(_bloodParticleFree[0]);
            _bloodParticleFree.RemoveAt(0);
            int __idx = _bloodParticleUsed.Count - 1;
            _bloodParticleUsed[__idx].transform.position = p_pos;
            _bloodParticleUsed[__idx].Play();
            StartCoroutine(DisableBlood(_bloodParticleUsed[__idx].duration, _bloodParticleUsed[__idx]));
        }
        base.LifeChange();
    }

    IEnumerator DisableBlood(float p_time, ParticleSystem p_particle)
    {
        yield return new WaitForSeconds(p_time);
        p_particle.Stop();
        p_particle.transform.position = new Vector3(-2000, -2000, -2000);
        _bloodParticleUsed.Remove(p_particle);
        _bloodParticleFree.Add(p_particle);
    }

    IEnumerator GoToAnimation(float p_time, string p_anim)
    {
        yield return new WaitForSeconds(p_time);
        if (!_isDead)
        {
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

    public override void PlayerOnMonsterZone(bool p_value)
    {
        base.PlayerOnMonsterZone(p_value);
    }

}
