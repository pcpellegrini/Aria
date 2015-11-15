using UnityEngine;
using System.Collections;

public class MonsterSpit : MonoBehaviour {

    public float damageValue;
    [HideInInspector]
    public Rigidbody spitRigidbody;

    private TrailRenderer _trail;
    

    public void ManualStart()
    {
        transform.position = new Vector3(-1000, -1000, -1000);
        spitRigidbody = GetComponent<Rigidbody>();
        _trail = GetComponent<TrailRenderer>();
        Disable();
    }

	void OnCollisionEnter(Collision p_collision)
    {
        AirCraft __aircraft = p_collision.gameObject.GetComponent<AirCraft>();
        __aircraft.DecreaseEnergy(damageValue);
        Disable();

    }

    public void Enable(Vector3 p_position, Vector3 p_dir)
    {
        transform.position = p_position;
        _trail.time = 1f;
        _trail.enabled = true;
        spitRigidbody.useGravity = true;
        spitRigidbody.AddForce(p_dir, ForceMode.Impulse);
        Invoke("Disable", 3f);
    }

    public void Disable()
    {
        CancelInvoke("Disable");
        _trail.time = 0f;
        _trail.enabled = false;
        spitRigidbody.useGravity = false;
        spitRigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(-1000, -1000, -1000);
    }
}
