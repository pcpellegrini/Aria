using UnityEngine;
using System.Collections;

public class CameraContoller : MonoBehaviour {

    public enum cameraType
    {
        FIRST_PERSON_VISION,
        THIRD_PERSON_VSION
    }
    
    public cameraType type;
    public GameObject aim;
    public GameObject arrow;
    public GameObject monster;
    public GameObject camPosition;
    public Vector2 aimLimits;

    [HideInInspector]
    public bool aimFollowMouse = true;
    [HideInInspector]
    public Vector2 _mousePos;

    protected float _timeDT;
    protected Rigidbody _rigidbody;
    protected Camera _camera;
    public Camera mainCam
    {
        get { return _camera; }
    }
    protected Vector2 _mouseOrigin;
    protected Vector2 _mouseStartPos;
    protected Vector3 _cameraStartPosition;
    protected Vector3 _cameraStartRotation;
    protected Vector2 _camLimitsMin = new Vector2(-15, 0);
    protected Vector2 _camLimitsMax = new Vector2(15, 30);
    protected Vector2 _camLimitsRotationMin = new Vector2(358, 340);
    protected Vector2 _camLimitsRotationMax = new Vector2(25, 20);
    protected Rigidbody _mainRigidbody;
    protected Animator _aimAnim;
    protected int _aimFiring;
    protected int _aimHit;

    private float _roll;
    public float roll
    {
        get { return _roll; }
        set { _roll = value; }
    }

    // Use this for initialization
    public virtual void ManualStart (Rigidbody p_rigibody) {
        _mainRigidbody = p_rigibody;
        _timeDT = Time.deltaTime;
        _camera = GetComponent<Camera>();
        _mouseOrigin = new Vector2(0.5f, 0.5f);
        //_mouseStartPos = _camera.ScreenToViewportPoint(Input.mousePosition);
        _mouseStartPos = Input.mousePosition;
        _cameraStartPosition = transform.localPosition;
        _cameraStartRotation = transform.rotation.eulerAngles;
        _aimAnim = aim.GetComponent<Animator>();
        _aimFiring = Animator.StringToHash("Firing");
        _aimHit = Animator.StringToHash("Hit");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector2 __aimScreen = _camera.WorldToViewportPoint(aim.transform.position);
        Vector2 __mousePos;
        if (aimFollowMouse)
        {
            __mousePos = _camera.ScreenToViewportPoint(Input.mousePosition);
            //__mousePos = Input.mousePosition;
            //__mousePos = Input.mousePosition;
         }
         else
         {
             __mousePos = __aimScreen + _mousePos*0.02f;
         }
         Vector2 __dif = __mousePos - __aimScreen;
         //Vector3 __dif = __mousePos - _mouseStartPos;
         /*Debug.Log(__dif);
         float __newPosX;
         float __tmp = _cameraStartPosition.x + (__dif.x * 100);

         if ((__tmp > _camLimitsRotationMin.y) || (__tmp < _camLimitsRotationMax.y))
         {
             __newPosX = __tmp;
         }
         else
         {
             __newPosX = transform.localRotation.eulerAngles.y;
         }

         float __newPosY = _cameraStartPosition.y + (__dif.y * 10);

         Quaternion __rot = transform.localRotation;

         __rot.eulerAngles = new Vector3(-((__newPosY*10)-48), __newPosX, _roll);
         transform.localRotation = __rot;*/

         if (__mousePos.x > __aimScreen.x && aim.transform.localPosition.x < aimLimits.x)
         {
             aim.transform.localPosition += new Vector3(__dif.x, 0f, 0f);
         }
         else if (__mousePos.x < __aimScreen.x && aim.transform.localPosition.x > -aimLimits.x)
         {
             aim.transform.localPosition += new Vector3(__dif.x, 0f, 0f);
         }

         if (__mousePos.y > __aimScreen.y && aim.transform.localPosition.y < aimLimits.y)
         {
             aim.transform.localPosition += new Vector3(0f, __dif.y, 0f);
         }
         else if (__mousePos.y < __aimScreen.y && aim.transform.localPosition.y > -aimLimits.y)
         {
             aim.transform.localPosition += new Vector3(0f, __dif.y, 0f);
         }
         
        Vector2 __camDif = __aimScreen - _mouseOrigin;
       // __camDif = (__mousePos - _mouseStartPos)/100;
        //_mouseStartPos = __mousePos;
        /*Quaternion __rot = transform.localRotation;
        __rot.eulerAngles = new Vector3(__camDif.y, __camDif.x, _roll);
        transform.localRotation = __rot;*/
        //if (__camDif.x < 0 && transform.localPosition.x < 5)
           // transform.localPosition = new Vector3(transform.localPosition.x + (__camDif.x / 10), transform.localPosition.y, transform.localPosition.z);

        if (__camDif.y > 0 && (transform.localRotation.eulerAngles.x >= _camLimitsRotationMin.x || transform.localRotation.eulerAngles.x < _camLimitsRotationMax.x + 1))
         {
             Quaternion __rot = transform.localRotation;
             if (__rot.eulerAngles.x - __camDif.y < _camLimitsRotationMin.x && __rot.eulerAngles.x >= _camLimitsRotationMin.x)
                 __rot.eulerAngles = new Vector3(_camLimitsRotationMin.x, __rot.eulerAngles.y, _roll);
             else
                 __rot.eulerAngles = new Vector3(__rot.eulerAngles.x -__camDif.y, __rot.eulerAngles.y, _roll);
             transform.localRotation = __rot;
        }
         else if (__camDif.y < 0 && (transform.localRotation.eulerAngles.x <= _camLimitsRotationMax.x || transform.localRotation.eulerAngles.x >= _camLimitsRotationMin.x))
         {
             Quaternion __rot = transform.localRotation;
             if (__rot.eulerAngles.x - __camDif.y > _camLimitsRotationMax.x && __rot.eulerAngles.x >= 0 && __rot.eulerAngles.x < _camLimitsRotationMax.x)
                 __rot.eulerAngles = new Vector3(_camLimitsRotationMax.x, __rot.eulerAngles.y, _roll);
             else
                 __rot.eulerAngles = new Vector3(__rot.eulerAngles.x - __camDif.y, __rot.eulerAngles.y, _roll);
             transform.localRotation = __rot;
         }

         if (__camDif.x > 0 && (transform.localRotation.eulerAngles.y < _camLimitsRotationMax.y || transform.localRotation.eulerAngles.y >= _camLimitsRotationMin.y))
         {
             Quaternion __rot = transform.localRotation;
             if (__rot.eulerAngles.y + __camDif.x >=  _camLimitsRotationMax.y && __rot.eulerAngles.y < _camLimitsRotationMax.y)
                 __rot.eulerAngles = new Vector3(__rot.eulerAngles.x, _camLimitsRotationMax.y, _roll);
             else
                 __rot.eulerAngles = new Vector3(__rot.eulerAngles.x, __rot.eulerAngles.y + __camDif.x, _roll);
             transform.localRotation = __rot;
         }
         else if (__camDif.x < 0 && (transform.localRotation.eulerAngles.y > _camLimitsRotationMin.y || transform.localRotation.eulerAngles.y <= _camLimitsRotationMax.y + 1))
         {
             Quaternion __rot = transform.localRotation;
             if (__rot.eulerAngles.y + __camDif.x <= _camLimitsRotationMin.y && __rot.eulerAngles.y > _camLimitsRotationMax.y + 1)
                 __rot.eulerAngles = new Vector3(__rot.eulerAngles.x, _camLimitsRotationMin.y, _roll);
             else
                 __rot.eulerAngles = new Vector3(__rot.eulerAngles.x, __rot.eulerAngles.y + __camDif.x, _roll);
             transform.localRotation = __rot;
         }

        // avoid z rotation

        /* Quaternion __rotLocal = transform.localRotation;
         __rotLocal.eulerAngles = new Vector3(__rotWorld.eulerAngles.x, __rotWorld.eulerAngles.y, -__rotWorld.z);
         transform.localRotation = __rotLocal;*/
        Arrow();

    }

    public Ray AimPosition()
    {
        Vector2 __aimScreen = _camera.WorldToViewportPoint(aim.transform.position);
        Ray __worldPosition = _camera.ViewportPointToRay(new Vector3(__aimScreen.x, __aimScreen.y, 0f));
        return __worldPosition;
    }
    
    public void Arrow()
    {
        arrow.transform.LookAt(monster.transform);
    }

    public void StartFiring(bool p_value)
    {
        if (p_value)
        {
            _aimAnim.SetBool(_aimFiring, true);
        }
        else
        {
            _aimAnim.SetBool(_aimFiring, false);
        }
    }

    public void AimHit()
    {
        _aimAnim.SetTrigger(_aimHit);
    }
}
