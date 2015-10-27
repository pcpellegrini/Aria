using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public GameObject bulletPrefab;
    public GameObject bulletsParent;
    public float initialBullets;
    public AirCraft currentShip;
    public GameObject InstPanel;

    private List<GameObject> _bullets = new List<GameObject>();
    
	void Start () {
        currentShip.specialBullet = bulletPrefab;
        currentShip.instPanel = InstPanel;
	}
}
