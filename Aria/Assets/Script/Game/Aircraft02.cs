using UnityEngine;
using System.Collections;

public class Aircraft02 : AirCraft {

    public GameObject[] rotors;

    public override void InputControl()
    {
        base.InputControl();
    }

    public override void ChangeSpeed(string p_state)
    {
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

    public override void ManualFixedUpdate()
    {
        base.ManualFixedUpdate();
        for (int i = 0; i< rotors.Length; i++)
        {
            int __num = i;
            Quaternion __rot = rotors[__num].transform.localRotation;
            __rot.eulerAngles += new Vector3(0f, 0f, _speed * Time.deltaTime);
            rotors[__num].transform.localRotation = __rot;
        }
    }
}
