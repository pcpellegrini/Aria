using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

    public AirCraft targetAircraft;

    public void EndAnimation(string p_animation)
    {
        targetAircraft.EndAnimation(p_animation);
    }


}
