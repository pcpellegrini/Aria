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
    protected Vector2 _camLimitsMin = new Vector2(-20, -5);
    protected Vector2 _camLimitsMax = new Vector2(20, 40);
    protected Vector2 _camLimitsRotationMin = new Vector2(355, 330);
    protected Vector2 _camLimitsRotationMax = new Vector2(30, 30);
    protected Rigidbody _mainRigidbody;

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
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        Vector2 __aimScreen = _camera.WorldToViewportPoint(aim.transform.position);
        Vector2 __mousePos;
        if (aimFollowMouse)
        {
            __mousePos = _camera.ScreenToViewportPoint(Input.mousePosition);
        }
        else
        {
            __mousePos = __aimScreen + _mousePos*0.02f;
        }
        Vector2 __dif = __mousePos - __aimScreen;

        
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

    }

    public Ray AimPosition()
    {
        Vector2 __aimScreen = _camera.WorldToViewportPoint(aim.transform.position);
        Ray __worldPosition = _camera.ViewportPointToRay(new Vector3(__aimScreen.x, __aimScreen.y, 0f));
        return __worldPosition;
    }
    
}
