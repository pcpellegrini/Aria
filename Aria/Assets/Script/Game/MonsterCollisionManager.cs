using UnityEngine;
using System.Collections;
using System;

public class MonsterCollisionManager : MonoBehaviour {

    public enum type
    {
        FRONT,
        TAIL,
        VULCAN_TOP,
        RIGHT,
        LEFT,
        AROUND,
        AWAY
    }
    public type positionType;
    public event Action<GameObject> onTrigger;
    public event Action onExitArea;

    void OnTriggerEnter(Collider p_collider)
    {
        if (p_collider.tag == "Player")
        {
            if (onTrigger != null) onTrigger(p_collider.gameObject);
        }
    }

    void OnTriggerExit(Collider p_collider)
    {
        if (p_collider.tag == "Player")
        {
            if (onExitArea != null) onExitArea();
        }
    }
}
