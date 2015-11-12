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
        gameObject.SetActive(true);
        Invoke("Disable", 2f);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        _isActive = false;
        _rigidbody.velocity = Vector3.zero;
        transform.position = new Vector3(-1000, -1000, -1000);
    }
}
