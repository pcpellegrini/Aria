using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    private float _startTime;
    private bool _isActive;
    private GameObject _gameObject;
    private Rigidbody _rigidbody;
    private TrailRenderer _trail;

    public bool isActive
    {
        get { return _isActive; }
        set { _isActive = value; }
    }
    public Rigidbody bulletRigidbody
    {
        get { return _rigidbody; }
    }

    // Use this for initialization
    public void ManualStart()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _trail = GetComponent<TrailRenderer>();
        _startTime = _trail.time;
        _gameObject = gameObject;
    }

    public void Disable()
    {
        CancelInvoke("Disable");
        _trail.time = 0f;
        _trail.enabled = false;
        gameObject.SetActive(false);
        _isActive = false;
        _rigidbody.useGravity = false;
        _rigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(-1000, -1000, -1000);
    }

    public void Enable(Vector3 p_position, Quaternion p_rot)
    {
        gameObject.SetActive(true);
        transform.position = p_position;
        transform.rotation = p_rot;
        _trail.time = _startTime;
        _trail.enabled = true;
        _rigidbody.useGravity = true;
        Invoke("Disable", 2f);
    }
}
