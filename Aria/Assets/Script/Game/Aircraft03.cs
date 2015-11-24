using UnityEngine;
using System.Collections;

public class Aircraft03 : AirCraft {
    
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

                break;
            case "normal":

                break;
            case "boost":

                break;
        }
    }
}
