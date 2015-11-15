using UnityEngine;
using System.Collections;

public class AnimationControllerMonster : MonoBehaviour {

    public Monster target;

    public void AnimationEvent(string p_event)
    {
        target.AnimationEvent(p_event);
    }
}
