using UnityEngine;
using System.Collections;

public class Info : MonoBehaviour {

    public AirCraft.type selectedAircraft;
    public string selectedLevel;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
}
