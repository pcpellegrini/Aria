﻿using UnityEngine;
using System.Collections;
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

    public monsterType type;
    public float energy;
    public float maxSpeed;
    public float hitDamage;
    public float acceletaion;
    public float idleTime;
    public float timeBetweenAttacks;
    public float roarDamage;
    public GameObject body;
    public Transform vulcanExit;

    [HideInInspector]
    public Transform[] islands;

    protected bool _isDead;
    protected bool _inPatrol;
    public bool inPatrol
    {
        get { return _inPatrol; }
        set { _inPatrol = value; }
    }
    protected float _currentSpeed;
    protected Animator _anim;
    protected Rigidbody _rigidbody;
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
	    
	}

    public virtual void AnimationEvent(string p_event)
    {

    }

    public virtual void Damage(string p_point, float p_damage, float p_time, Bullet p_bullet)
    {

    }

    public virtual void LifeChange()
    {
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
}
