using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {

    public GameObject bulletPrefab;
    public GameObject bulletsParent;
    public float initialBullets;
    public AirCraft currentShip;

    private List<GameObject> _bullets = new List<GameObject>();
    
	void Start () {
        currentShip.specialBullet = bulletPrefab;
	}
}
