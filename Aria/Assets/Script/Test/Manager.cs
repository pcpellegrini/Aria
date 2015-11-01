using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    public float initialBullets;
    public Transform startPosition;
    public GameObject InstPanel;
    public GameObject bulletPrefab;
    public GameObject bulletsParent;
    public GameObject[] aircrafts;
    public GuiManager guiManager;
    public GameObject aimHUD;


    private AirCraft _currentPLayerCS;
    private Info _gameInfo;
    private GameObject _currentPLayer;


    private List<GameObject> _bullets = new List<GameObject>();
    
	void Start () {
        _gameInfo = GameObject.Find("_info").GetComponent<Info>();
        _currentPLayer = Instantiate(aircrafts[(int)_gameInfo.selectedAircraft], startPosition.position, Quaternion.identity) as GameObject;
        _currentPLayerCS = _currentPLayer.GetComponent<AirCraft>();
        _currentPLayerCS.ManualStart(bulletPrefab, InstPanel, guiManager, aimHUD);
	}
    void Update()
    {
        _currentPLayerCS.ManualUpdate();
    }

    void FixedUpdate()
    {
        _currentPLayerCS.ManualFixedUpdate();
    }
}
