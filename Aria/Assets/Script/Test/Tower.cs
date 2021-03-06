﻿using UnityEngine;
using System.Collections;

public class Tower : MonoBehaviour {

    public ParticleSystem explosionParticle;
    public GameObject tower;
    public float hp;

    private Collider _collider;
    private Renderer _renderer;
	void Start () {
        explosionParticle.Stop();
        _collider = GetComponent<Collider>();
        _renderer = transform.GetChild(0).GetComponent<Renderer>();
    }

    public void Damage(float p_damage, float p_time, Bullet p_bullet)
    {
        StartCoroutine(ApplyDamage(p_time, p_damage, p_bullet));
    }

    IEnumerator ApplyDamage(float p_time, float p_damage, Bullet p_bullet)
    {
        yield return new WaitForSeconds(p_time);
        if (p_bullet != null)
            p_bullet.Disable();
        hp -= p_damage;
        _renderer.material.color += new Color(0.5f, 0f, 0f);
        if (hp <= 0)
            TowerDeath();
    }

    private void TowerDeath()
    {
        _collider.enabled = false;
        tower.SetActive(false);
        explosionParticle.enableEmission = true;
        explosionParticle.Play();
        Invoke("DisableTower", explosionParticle.duration);
    }

    private void DisableTower()
    {

    }
}
