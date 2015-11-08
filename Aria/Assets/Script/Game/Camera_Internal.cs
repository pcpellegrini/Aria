using UnityEngine;
using System.Collections;

public class Camera_Internal : CameraContoller {

	// Use this for initialization
	public override void ManualStart (Rigidbody p_rigibody) {
        base.ManualStart(p_rigibody);
        _camLimitsRotationMin.x = 345;
        _camLimitsRotationMax.x = 5;
        _camLimitsRotationMin.y = 350;
        _camLimitsRotationMax.y = 10;
    }
}
