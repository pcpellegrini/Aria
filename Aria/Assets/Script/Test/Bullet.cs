using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

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
        _gameObject = gameObject;
        CancelBullet();
    }

    public void Enable(Vector3 p_position)
    {
        _isActive = true;
        _gameObject.transform.position = p_position;
        _trail.enabled = true;
        _gameObject.SetActive(true);
        Invoke("CancelBullet", 2f);
    }

    public void CancelBullet()
    {
        _trail.enabled = false;
        _rigidbody.velocity = Vector3.zero;
        transform.position = Vector3.zero;
        _gameObject.SetActive(false);
        _isActive = false;
    }
}
