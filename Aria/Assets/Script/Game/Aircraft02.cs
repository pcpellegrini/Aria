using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Aircraft02 : AirCraft {

    public GameObject[] rotors;

    public override void InputControl()
    {
        base.InputControl();
    }

    public override void ManualStart(List<Bullet> p_bullet, GameObject p_panel, GuiManager p_gui, GameObject p_aim, AudioSource p_musicSource, AudioClip p_musicLevel)
    {
        base.ManualStart(p_bullet, p_panel, p_gui, p_aim, p_musicSource, p_musicLevel);
        TPSCameraController.camLimitsRotationMax = new Vector2(5, 5);
        TPSCameraController.camLimitsRotationMin = new Vector2(359, 355);
    }
    public override void ChangeSpeed(string p_state)
    {
        base.ChangeSpeed(p_state);
        /*switch (p_state)
        {
            case "break":
                _anim.SetBool(_animIDAcc, false);
                break;
            case "normal":
                _anim.SetBool(_animIDBoost, false);
                _anim.SetBool(_animIDAcc, true);
                break;
            case "boost":
                _anim.SetBool(_animIDBoost, true);
                break;
        }*/
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        for (int i = 0; i< rotors.Length; i++)
        {
            int __num = i;
            Quaternion __rot = rotors[__num].transform.localRotation;
            __rot.eulerAngles += new Vector3(0f, 0f, _speed * Time.deltaTime);
            rotors[__num].transform.localRotation = __rot;
        }
    }
}
