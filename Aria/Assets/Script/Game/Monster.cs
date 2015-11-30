using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Monster : MonoBehaviour {

    public enum monsterType
    {
        DESERT,
        AERIAL,
        LITTLE
    }
    public event Action<float> onEnergyChange;
    public event Action onPlayerEnterMonsterZone;
    public event Action onPlayerExitMonsterZone;
    public event Action onDisable;
    public event Action eventOne;
    public event Action eventTwo;
    public event Action eventThree;

    public monsterType type;
    public float energy;
    public float maxSpeed;
    public float hitDamage;
    public float acceletaion;
    public float idleTime;
    public float timeBetweenAttacks;
    public float roarDamage;
    public GameObject body;
    public GameObject hitBloodPrefab;
    public Transform vulcanExit;

    [HideInInspector]
    public Transform[] islands;

    protected bool _isDead;
    protected bool _inPatrol;
    protected bool[] _eventCompleted = new bool[3];
    public bool inPatrol
    {
        get { return _inPatrol; }
        set { _inPatrol = value; }
    }
    protected bool _wasThrow;
    public bool wasThrow
    {
        get { return _wasThrow; }
        set { _wasThrow = value; }
    }
    protected float _currentSpeed;
    protected float _lifeEventOne;
    protected float _lifeEventTwo;
    protected float _lifeEventThree;
    protected Animator _anim;
    protected Rigidbody _rigidbody;
    protected List<GameObject> _blood = new List<GameObject>();
    protected List<ParticleSystem> _bloodParticle = new List<ParticleSystem>();
    protected List<ParticleSystem> _bloodParticleFree = new List<ParticleSystem>();
    protected List<ParticleSystem> _bloodParticleUsed = new List<ParticleSystem>();
    protected bool _isActive;
    public bool isActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }
    protected bool _hasAttacked;
    public bool hasAttacked
    {
        get { return _hasAttacked; }
        set { _hasAttacked = value; }
    }

    public virtual void ManualStart (AirCraft p_playerCS) {
        _lifeEventOne = energy - (energy * 0.2f);
        _lifeEventTwo = energy - (energy * 0.4f);
        _lifeEventThree = energy - (energy * 0.8f);
    }

    public virtual void AnimationEvent(string p_event)
    {

    }

    public virtual void Damage(string p_point, float p_damage, float p_time, Bullet p_bullet, Vector3 p_pos)
    {

    }

    public virtual void LifeChange()
    {
        if (energy <= _lifeEventOne && !_eventCompleted[0])
        {
            _eventCompleted[0] = true;
            if (eventOne != null) eventOne();
        }
        else if (energy <= _lifeEventTwo && !_eventCompleted[1])
        {
            _eventCompleted[1] = true;
            if (eventTwo != null) eventTwo();
        }
        else if (energy <= _lifeEventTwo && !_eventCompleted[2])
        {
            _eventCompleted[2] = true;
            if (eventThree != null) eventThree();
        }
        if (onEnergyChange != null) onEnergyChange(energy);
    }

    public virtual void PauseGame(bool p_value)
    {

    }

    public virtual void Enable()
    {

    }

    public virtual void Disable()
    {

    }
    public virtual void HitOnPlayer()
    {

    }

    public virtual void OnDisableMonster()
    {
        if (onDisable != null) onDisable();
    }

    public virtual void PlayerOnMonsterZone(bool p_value)
    {
        if (p_value)
        {
            if (onPlayerEnterMonsterZone != null) onPlayerEnterMonsterZone();
        }
        else
        {
            if (onPlayerExitMonsterZone != null) onPlayerExitMonsterZone();
        }
        
    }

    public virtual void GoHome(Vector3 p_position)
    {

    }

    public virtual void InvokeGoHome()
    {

    }

    public virtual void FollowPlayer(GameObject p_player)
    {

    }

    public virtual void ThrowMonster(Vector3 p_pos, float p_radius)
    {

    }

}
