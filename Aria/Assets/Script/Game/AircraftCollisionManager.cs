using UnityEngine;
using System.Collections;
using System;

public class AircraftCollisionManager : MonoBehaviour {

    public event Action onHitGround;
    public event Action onHitStaticObject;

	// Use this for initialization
	public void ManualStart () {
	
	}

    void OnCollisionEnter(Collision p_collision)
    {
        GameObject __GO = p_collision.gameObject;
        if (__GO.tag == "terrain")
        {
            if (onHitGround != null)
                onHitGround();
        }
        else if (__GO.tag == "tower")
        {
            if (onHitStaticObject != null)
                onHitStaticObject();
        }
    }
}
