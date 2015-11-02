using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour {

    public enum source
    {
        ENGINE,
        BOOST,
        MUSIC,
        NORMAL_WEAPON,
        SPECIAL_WEAPON
    }

    public AudioSource engineSound;
    public AudioSource boostSound;
    public AudioSource normalWeaponSound;
    public AudioSource specialweaponSound;
    [HideInInspector]
    public AudioSource musicSound;

    private float normalPitch = 1f;
    private float supPitch = 0.5f;
    private List<AudioSource> playingSources = new List<AudioSource>();

    public void PlaySound(source p_source, AudioClip p_clip)
    {
        switch (p_source)
        {
            case source.MUSIC:
                musicSound.clip = p_clip;
                musicSound.Play();
                break;
            case source.ENGINE:
                engineSound.clip = p_clip;
                engineSound.Play();
                break;
            case source.BOOST:
                if (!boostSound.isPlaying)
                {
                    boostSound.clip = p_clip;
                    boostSound.Play();
                }
                break;
            case source.NORMAL_WEAPON:
                normalWeaponSound.clip = p_clip;
                normalWeaponSound.Play();
                break;
            case source.SPECIAL_WEAPON:
                specialweaponSound.clip = p_clip;
                specialweaponSound.Play();
                break;
        }
    }

    public void StopSound(source p_source)
    {
        switch (p_source)
        {
            case source.MUSIC:
                musicSound.Stop();
                break;
            case source.ENGINE:
                engineSound.Stop();
                break;
            case source.BOOST:
                boostSound.Stop();
                break;
            case source.NORMAL_WEAPON:
                normalWeaponSound.Stop();
                break;
            case source.SPECIAL_WEAPON:
                specialweaponSound.Stop();
                break;
        }
    }

    public void InsideCockpit(bool p_cond)
    {
        if (p_cond)
        {
            engineSound.pitch = supPitch;
            boostSound.pitch = supPitch;
            normalWeaponSound.pitch = supPitch;
            specialweaponSound.pitch = supPitch;
        }
        else
        {
            engineSound.pitch = normalPitch;
            boostSound.pitch = normalPitch;
            normalWeaponSound.pitch = normalPitch;
            specialweaponSound.pitch = normalPitch;
        }
    }

    public void PauseAll(bool p_cond)
    {
        if (p_cond)
        {
            if (musicSound.isPlaying) playingSources.Add(musicSound);
            if (engineSound.isPlaying) playingSources.Add(engineSound);
            if (boostSound.isPlaying) playingSources.Add(boostSound);
            if (normalWeaponSound.isPlaying) playingSources.Add(normalWeaponSound);
            if (specialweaponSound.isPlaying) playingSources.Add(specialweaponSound);
            
            for (int i = 0; i < playingSources.Count; i++)
            {
                int __num = i;
                playingSources[__num].Pause();
            }
        }
        else
        {
            for (int i = 0; i < playingSources.Count; i++)
            {
                int __num = i;
                playingSources[__num].Play();
            }
            playingSources.Clear();
        }
    }
}
