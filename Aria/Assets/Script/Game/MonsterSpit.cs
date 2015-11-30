using UnityEngine;
using System.Collections;

public class MonsterSpit : MonoBehaviour {

    public float damageValue;
    public float speed;
    [HideInInspector]
    public Rigidbody spitRigidbody;

    private TrailRenderer _trail;
    private bool _followPlayer;
    private Vector3 _direction;
    private GameObject _player;

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
        if (__aircraft != null)
        {
            __aircraft.DecreaseEnergy(damageValue);
        }
        Disable();

    }
    void Update()
    {
        if (_followPlayer)
        {
            Vector3 __distance = _player.transform.position - transform.position;
            float __mag = __distance.magnitude;
            _direction = __distance / __mag;
            spitRigidbody.velocity = _direction * speed;
        }
    }

    public void Enable(Vector3 p_position, GameObject p_player)
    {
        transform.position = p_position;
        _player = p_player;
        _trail.time = 1f;
        _trail.enabled = true;
        spitRigidbody.useGravity = true;
        //spitRigidbody.AddForce(p_dir, ForceMode.Impulse);
        this.enabled = true;
        _followPlayer = true;
        Invoke("Disable", 3f);
    }

    public void Disable()
    {
        this.enabled = false;
        _followPlayer = false;
        CancelInvoke("Disable");
        _trail.time = 0f;
        _trail.enabled = false;
        spitRigidbody.useGravity = false;
        spitRigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(-1000, -1000, -1000);
    }
}
