using UnityEngine;
using System.Collections;

public class CameraContoller : MonoBehaviour {

    public enum cameraType
    {
        FIRST_PERSON_VISION,
        THIRD_PERSON_VSION
    }

    public bool normalizeZPosition;
    public bool normalizeXPosition;
    public bool normalizeYPosition;
    public float cameraMovementSpeed;
    public cameraType type;
    public Transform follow;
    public Transform look;
    public Vector3 distance;

    private bool[] _isLerping = new bool[3];
    private float _timeDT;
    private float[] _timeTakenDuringLerp = new float[3];
    private float[] _timeStartedLerping = new float[3];
    private Vector3 _lerpStartPosition;
    private Rigidbody _rigidbody;

	// Use this for initialization
	void Start () {
        transform.localPosition = distance;
        _timeDT = Time.deltaTime;

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        _timeDT = Time.deltaTime;

        if (normalizeXPosition)
        {
            float __dist = transform.localPosition.x - distance.x;
            if (__dist < 0)
                __dist = -__dist;
            ChangeToOriginalPosition("x", __dist);
        }
        else if (_isLerping[0])
            _isLerping[0] = false;

        if (normalizeYPosition)
        {
            float __dist = transform.localPosition.y - distance.y;
            if (__dist < 0)
                __dist = -__dist;
            ChangeToOriginalPosition("y", __dist);
        }
        else if (_isLerping[1])
            _isLerping[1] = false;

        if (normalizeZPosition)
        {
            float __dist = transform.localPosition.z - distance.z;
            if (__dist < 0)
                __dist = -__dist;
            ChangeToOriginalPosition("z", __dist);
        }
        else if (_isLerping[2])
            _isLerping[2] = false;


    }

    public void ChangePosition(string p_axis, float p_direction)
    {
        switch (p_axis)
        {
            case "z":
                normalizeZPosition = false;
                if ((transform.localPosition.z > distance.z - 7))
                transform.localPosition += new Vector3(0f, 0f, -p_direction * _timeDT * cameraMovementSpeed);
                break;
            case "y":
                normalizeYPosition = false;
                if ((p_direction > 0f && transform.localPosition.y < 6f) || (p_direction < 0f && transform.localPosition.y > 0f))
                    transform.localPosition += new Vector3(0f, p_direction * _timeDT * cameraMovementSpeed, 0f);
                break;
            case "x":
                normalizeXPosition = false;
                if ((p_direction > 0f && transform.localPosition.x < 4f) || (p_direction < 0f && transform.localPosition.x > -4f))
                    transform.localPosition += new Vector3(p_direction * _timeDT * cameraMovementSpeed, 0f, 0f);
                break;
        }

    }

    public bool NeedsNormalize(string p_axis)
    {
        switch (p_axis)
        {
            case "z":
                if (transform.localPosition.z != distance.z)
                    return true;
                else
                    return false;
            case "y":
                if (transform.localPosition.y != distance.y)
                    return true;
                else
                    return false;
            case "x":
                if (transform.localPosition.x != distance.x)
                    return true;
                else
                    return false;
            default:
                return false;
        }
    }

    public void ChangeToOriginalPosition(string p_axis, float p_dist)
    {
        switch (p_axis)
        {
            case "z":
                if (NeedsNormalize("z"))
                {
                    float __percent = LerpPercent("z", p_dist);
                    transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, transform.localPosition.y, _lerpStartPosition.z), new Vector3(transform.localPosition.x, transform.localPosition.y, distance.z), __percent);
                }
                else
                    normalizeZPosition = false;
                break;
            case "y":
                if (NeedsNormalize("y"))
                {
                    float __percent = LerpPercent("y", p_dist);
                    transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, _lerpStartPosition.y, transform.localPosition.z), new Vector3(transform.localPosition.x, distance.y, transform.localPosition.z), __percent);
                }
                else
                    normalizeYPosition = false;
                break;
            case "x":
                if (NeedsNormalize("x"))
                {
                    float __percent = LerpPercent("x", p_dist);
                    transform.localPosition = Vector3.Lerp(new Vector3(_lerpStartPosition.x, transform.localPosition.y, transform.localPosition.z), new Vector3(distance.x, transform.localPosition.y, transform.localPosition.z), __percent);
                }
                else
                    normalizeXPosition = false;
                break;
        }   

    }

    private float LerpPercent(string p_axis, float p_dist)
    {
        switch (p_axis)
        {
            case "z":
                if (!_isLerping[2])
                {
                    _timeTakenDuringLerp[2] = p_dist * 0.12f;
                    _isLerping[2] = true;
                    _timeStartedLerping[2] = Time.time;
                    _lerpStartPosition.z = transform.localPosition.z;
                }
                float __timeSinceStartedZ = Time.time - _timeStartedLerping[2];
                float __percentageCompleteZ = __timeSinceStartedZ / _timeTakenDuringLerp[2];
                if (__percentageCompleteZ >= 1.0f)
                {
                    _isLerping[2] = false;
                    _lerpStartPosition.z = 0f;
                }
                return __percentageCompleteZ;
            case "y":
                if (!_isLerping[1])
                {
                    _timeTakenDuringLerp[1] = p_dist * 0.5f;
                    _isLerping[1] = true;
                    _timeStartedLerping[1] = Time.time;
                    _lerpStartPosition.y = transform.localPosition.y;
                }
                float __timeSinceStartedY = Time.time - _timeStartedLerping[1];
                float __percentageCompleteY = __timeSinceStartedY / _timeTakenDuringLerp[1];
                if (__percentageCompleteY >= 1.0f)
                {
                    _isLerping[1] = false;
                    _lerpStartPosition.y = 0f;
                }
                return __percentageCompleteY;
            case "x":
                if (!_isLerping[0])
                {
                    _timeTakenDuringLerp[0] = p_dist*0.7f;
                    _isLerping[0] = true;
                    _timeStartedLerping[0] = Time.time;
                    _lerpStartPosition.x = transform.localPosition.x;
                }
                float __timeSinceStartedX = Time.time - _timeStartedLerping[0];
                float __percentageCompleteX = __timeSinceStartedX / _timeTakenDuringLerp[0];
                if (__percentageCompleteX >= 1.0f)
                {
                    _isLerping[0] = false;
                    _lerpStartPosition.x = 0f;
                }
                return __percentageCompleteX;
            default:
                return 0f;
        }
        
    }
}
