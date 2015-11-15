using UnityEngine;
using System.Collections;
using System;

public class Monster : MonoBehaviour {

    public event Action<float> onEnergyChange;

    public float energy;
    public float maxSpeed;
    public float acceletaion;
    public float idleTime;
    public float timeBetweenAttacks;
    public float roarDamage;
    public GameObject body;

    protected bool _isDead;
    protected float _currentSpeed;
    protected Animator _anim;
    protected Rigidbody _rigidbody;
    
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
}
