using UnityEngine;
using System.Collections;

public class Info : MonoBehaviour {

    public AirCraft.type selectedAircraft;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
