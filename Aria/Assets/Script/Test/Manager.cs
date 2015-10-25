using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour {

    public GameObject bulletPrefab;
    public GameObject bulletsParent;
    public float initialBullets;
    public Flight currentShip;

    private List<GameObject> _bullets = new List<GameObject>();
    
	void Start () {
        for (int i = 0; i < initialBullets; i++)
        {
            int __idx = i;
            _bullets.Add(Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity) as GameObject);
            _bullets[__idx].GetComponent<Bullet>().ManualStart();
            _bullets[__idx].transform.parent = bulletsParent.transform;
        }
        currentShip.bullets = _bullets;
	}
}
