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
        AWAY,
        BODY
    }
    public type positionType;
    public event Action<GameObject> onTrigger;
    public event Action onExitArea;
    public event Action<Vector3> onGrabCollision;

    /*void OnCollisionEnter(Collision p_collision)
    {
        Debug.Log("collided");
        if (p_collision.gameObject.tag == "island" && positionType == type.BODY)
        {
            Debug.Log("hitIsland");
            
        }
    }*/

    void OnTriggerEnter(Collider p_collider)
    {
        if (p_collider.tag == "island" && positionType == type.BODY)
        {
            if (onGrabCollision != null) onGrabCollision(p_collider.transform.position);
        }
        else if (p_collider.tag == "Player" && positionType != type.BODY)
        {
            if (onTrigger != null) onTrigger(p_collider.gameObject);
        }
    }

    void OnTriggerExit(Collider p_collider)
    {
        if (p_collider.tag == "Player" && positionType != type.BODY)
        {
            if (onExitArea != null) onExitArea();
        }
    }
}
