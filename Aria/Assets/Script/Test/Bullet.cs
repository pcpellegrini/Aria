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
        _gameObject = gameObject;
        Destroy(_gameObject, 2f);
    }
}
