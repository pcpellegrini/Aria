using UnityEngine;
using System.Collections;

public class Aircraft01 : AirCraft {


    public override void InputControl()
    {
        base.InputControl();
    }

    public override void ChangeSpeed(string p_state)
    {
        base.ChangeSpeed(p_state);
        switch (p_state)
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
        }
    }
}
