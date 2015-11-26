using UnityEngine;
using System.Collections;
using System;

public class AircraftCollisionManager : MonoBehaviour {

    public event Action<Vector3> onHitGround;
    public event Action<Vector3> onHitStaticObject;
    public event Action<Vector3, float> onHitEnemyLittle;
    public event Action<Vector3, float> onHitEnemy;

    // Use this for initialization
    public void ManualStart () {
        this.enabled = true;
    }

    void OnCollisionEnter(Collision p_collision)
    {
        GameObject __GO = p_collision.gameObject;
        Vector3 __point = p_collision.contacts[0].point;
        if (__GO.tag == "terrain")
        {
            if (onHitGround != null)
                onHitGround(__point);
        }
        else if (__GO.tag == "staticObject")
        {
            if (onHitStaticObject != null)
                onHitStaticObject(__point);
        }
        else if (__GO.tag == "enemy")
        {
            Monster __monster = p_collision.gameObject.transform.root.GetComponent<Monster>();
            if (__monster != null)
            {
                if ((__monster.type == Monster.monsterType.LITTLE && !__monster.hasAttacked))
                {
                    __monster.hasAttacked = true;
                    __monster.HitOnPlayer();
                    if (onHitEnemyLittle != null) onHitEnemyLittle(__point, __monster.hitDamage);
                }
                else if (__monster.type != Monster.monsterType.LITTLE)
                {
                    __monster.hasAttacked = true;
                    __monster.HitOnPlayer();
                    if (onHitEnemy != null) onHitEnemy(__point, __monster.hitDamage);
                }
            }
        }
    }
}
