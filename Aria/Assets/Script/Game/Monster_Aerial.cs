using UnityEngine;
using System.Collections;

public class Monster_Aerial : Monster {
    
    public MonsterCollisionManager collisionFront;
    public MonsterCollisionManager collisionAround;
    public MonsterCollisionManager collisionBody;
    public Transform spitPosition;
    public MonsterSpit spit;
    public float grabTime;

    private MonsterCollisionManager.type _currentArea;
    private bool _goWalk;
    private bool _walking;
    private bool _grabing;
    private bool _inRasanteAttack;
    private bool _inAntecipation;
    private bool _inSpitAttack;
    private bool _inRoarAttack;
    private int _animGrab;
    private int _animRoar;
    private int _animSpit;
    private int _animPlan;
    private int _animDeath;
    private float _damageCount = 0;
    private Vector3 _grabIslandPosition;
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

        for (int i = 0; i < 200; i++)
        {
            int __num = i;
            _blood.Add(Instantiate(hitBloodPrefab, new Vector3(-2000, -2000, -2000), Quaternion.identity) as GameObject);
            _bloodParticle.Add(_blood[__num].GetComponent<ParticleSystem>());
            _bloodParticle[__num].Stop();
            _bloodParticleFree.Add(_bloodParticle[__num]);
        }

        collisionBody.onGrabCollision += delegate (Vector3 p_point)
        {
            if (!_grabing && !_isDead)
            {
                _grabing = true;
                Grab(p_point);
            }
        };
        collisionFront.onTrigger += delegate (GameObject p_player)
        {
            if (!_grabing && !_inRasanteAttack && !_inSpitAttack && !_isDead && !_inAntecipation)
            {
                _player = p_player;
                int __rnd = Random.Range(1, 2);
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
                        Antecipation();
                        //RasanteAttack(true);
                        _inRasanteAttack = true;
                    }
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
            PlayerOnMonsterZone(true);
            _currentArea = collisionAround.positionType;
        };
        collisionAround.onExitArea += delegate
        {
            PlayerOnMonsterZone(false);
            if (_currentArea == MonsterCollisionManager.type.AROUND)
                _currentArea = MonsterCollisionManager.type.AWAY;
            //StartCoroutine(GoToAnimation(timeBetweenAttacks, "Walk"));
            if (!_inRasanteAttack)
                _player = null;
        };

        this.enabled = true;
        spit.ManualStart();
        Invoke("MoveToRandom", idleTime);
    }

    void FixedUpdate()
    {
        if (_currentArea == MonsterCollisionManager.type.FRONT && _player != null && !_isDead)
        {
            //var newRot = Quaternion.LookRotation(_player.transform.position);
            //transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 0.2f);
            transform.LookAt(_player.transform);
        }
    }

    public override void HitOnPlayer()
    {
        base.HitOnPlayer();
        if (_inRasanteAttack && _player != null)
        {
            CancelInvoke("CancelRasante");
            RasanteAttack(false);
            Invoke("EndRasanteAttack", 1f);
        }
    }

    public override void AnimationEvent(string p_event)
    {
        base.AnimationEvent(p_event);
        if (!_isDead)
        {
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
                        spit.Enable(spitPosition.position, _playerCS.airCraftBody);
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
        energy -= p_damage * 1f;
        if (energy <= 0f)
        {
            _anim.SetTrigger(_animDeath);
            _rigidbody.useGravity = true;
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
        _damageCount += p_damage;
        if (_damageCount > (energy*0.05f) && !_inRasanteAttack && !_inSpitAttack && !_grabing)
        {
            _damageCount = 0;
            GoHide();
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

    private void CancelRasante()
    {
        RasanteAttack(false);
    }

    private void Antecipation()
    {
        if (!_isDead && !_inRasanteAttack && !_inAntecipation)
        {
            _inAntecipation = true;
            _rigidbody.velocity = Vector3.zero;
            Vector3 __distance = transform.position - _player.transform.position;
            float __mag = __distance.magnitude;
            if (__mag == 0)
                __mag = 0.01f;
            Vector3 __dir = __distance / __mag;
            _rigidbody.velocity = __dir * (maxSpeed);
            Invoke("DoRasante", 0.5f);
        }
        
    }

    private void DoRasante()
    {
        RasanteAttack(true);
        _inAntecipation = false;
    }

    private void RasanteAttack(bool p_value)
    {
        if (p_value && !_isDead && _player != null)
        {
            _rigidbody.velocity = Vector3.zero;
            Vector3 __distance = _player.transform.position - transform.position;
            float __mag = __distance.magnitude;
            if (__mag == 0)
                __mag = 0.01f;
            Vector3 __dir = __distance / __mag;
            _rigidbody.velocity = __dir * (maxSpeed*2);
            _anim.SetBool(_animPlan, true);
            Invoke("CancelRasante", 1f);
        }
        else if(!_isDead)
        {
            _inRasanteAttack = false;
            Quaternion __rot = transform.rotation;
            __rot.eulerAngles = new Vector3(0f, __rot.eulerAngles.y, __rot.eulerAngles.z);
            transform.rotation = __rot;
            _rigidbody.velocity = Vector3.zero;
            Vector3 __distance = transform.position - _player.transform.position;
            float __mag = __distance.magnitude;
            if (__mag == 0)
                __mag = 0.01f;
            Vector3 __dir = __distance / __mag;
            _rigidbody.velocity = __dir * (maxSpeed);
            _anim.SetBool(_animPlan, false);
        }
    }

    private void EndRasanteAttack()
    {
        _hasAttacked = false;
        _rigidbody.velocity = Vector3.zero;
        MoveToRandom();
    }

    IEnumerator GoToAnimation(float p_time, string p_anim)
    {
        yield return new WaitForSeconds(p_time);
        if (!_isDead)
        {
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
    }

    private void Grab(Vector3 p_point)
    {
        if (!_isDead)
        {
            _grabIslandPosition = p_point;
            _rigidbody.velocity = Vector3.zero;
            _anim.SetBool(_animGrab, true);
            StartCoroutine(EndGrab(grabTime));
        }
    }

    private void MoveToRandom()
    {
        if (!_inRasanteAttack && !_inSpitAttack && !_grabing && !_isDead)
        {
            Vector3 __dir = UnityEngine.Random.insideUnitSphere * 10;
            _rigidbody.velocity = Vector3.zero;
            transform.LookAt(__dir+transform.position);
            _rigidbody.velocity = (maxSpeed) * (__dir / 10);
            float __timeRnd = UnityEngine.Random.Range(4, 10);
            Invoke("StopMoveRandom", __timeRnd);
        }
    }

    private void StopMoveRandom()
    {
        _rigidbody.velocity = Vector3.zero;
        if (!_inRasanteAttack && !_inSpitAttack && !_grabing && !_isDead)
        {            
            Invoke("MoveToRandom", idleTime);
        }
    }

    private void GoHide()
    {
        if (!_isDead && !_inRasanteAttack)
        {
            float __dist = Mathf.Infinity;
            int __idx = 99;
            for (int i = 0; i < islands.Length; i++)
            {
                int __num = i;
                float __tmpDist = Vector3.Distance(islands[__num].position, transform.position);
                if (__tmpDist < __dist && islands[__num].position.y > transform.position.y)
                {
                    __dist = __tmpDist;
                    __idx = __num;
                }
            }
            if (__idx != 99)
            {
                transform.LookAt(islands[__idx]);
                Vector3 __distance = islands[__idx].position - transform.position;
                float __mag = __distance.magnitude;
                Vector3 __dir = __distance / __mag;
                _rigidbody.velocity = (__dir) * maxSpeed;
            }
            else
            {
                Vector3 __dir = Vector3.down;
                transform.LookAt(__dir * 100);
                _rigidbody.velocity = (__dir) * maxSpeed;
                Invoke("GoHide", 2f);
            }
        }
    }

    IEnumerator EndGrab(float p_time)
    {
        yield return new WaitForSeconds(p_time);
        if (!_isDead)
        {
            _anim.SetBool(_animGrab, false);
            Vector3 __dir = transform.position - _grabIslandPosition;

            _rigidbody.AddForce(__dir * 10000, ForceMode.Force);
            _grabing = false;
            MoveToRandom();
        }
    }
    public override void PlayerOnMonsterZone(bool p_value)
    {
        base.PlayerOnMonsterZone(p_value);
    }
}
