using UnityEngine;
using System.Collections;

public class Monster_Desert : MonoBehaviour {

    public GameObject body;


    private Animator _anim;
    private Rigidbody _rigidbody;
    private bool walk;
    // Use this for initialization
    void Start () {
        _anim = body.GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        Invoke("Walk", 30);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (walk)
        {
            _rigidbody.velocity = -transform.forward * 100000;
        }
	}

    private void Walk()
    {
        _anim.SetTrigger("Walk");
        walk = true;
        Invoke("Tail", 20);
    }

    private void Tail()
    {
        _anim.SetTrigger("Tail");
        walk = false;
        Invoke("Spit", 20);
    }

    private void Spit()
    {
        _anim.SetTrigger("Spit");
        Invoke("Spit", 20);
    }

    private void Roar()
    {
        _anim.SetTrigger("Roar");
        Invoke("Walk", 20);
    }
}
