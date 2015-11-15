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
    public Monster currentMonster;
    public GameObject aimHUD;
    public AudioClip levelMusic;

    private AirCraft _currentPLayerCS;
    private Info _gameInfo;
    private GameObject _currentPLayer;
    private AudioSource _audioSource;

    private List<Bullet> _bullets = new List<Bullet>();
    List<GameObject> _bulletsGO = new List<GameObject>();

    void Start () {
        UnityEngine.Cursor.visible = false;
        _gameInfo = GameObject.Find("_info").GetComponent<Info>();
        _audioSource = GetComponent<AudioSource>();
        _currentPLayer = Instantiate(aircrafts[(int)_gameInfo.selectedAircraft], startPosition.position, Quaternion.identity) as GameObject;
        _currentPLayerCS = _currentPLayer.GetComponent<AirCraft>();
        
        for (int i = 0; i< 50; i++)
        {
            _bulletsGO.Add(Instantiate(bulletPrefab, new Vector3(-1000, -1000, -1000), Quaternion.identity) as GameObject);
        }
        for (int i = 0; i < _bulletsGO.Count; i++)
        {
            int __num = i;
            _bullets.Add(_bulletsGO[__num].GetComponent<Bullet>());
            _bullets[__num].gameObject.transform.parent = bulletsParent.transform;
            _bullets[__num].isActive = false;
            _bullets[__num].ManualStart();
        }
        _currentPLayerCS.ManualStart(_bullets, InstPanel, guiManager, aimHUD, _audioSource, levelMusic);
        currentMonster.onEnergyChange += delegate (float p_energy)
        {
            guiManager.ChangeValue("monsterEnergy", p_energy);
        };
        guiManager.inGame = true;
        currentMonster.ManualStart(_currentPLayerCS);
        guiManager.monsterEnergy.maxValue = currentMonster.energy;
        guiManager.monsterEnergy.value = currentMonster.energy;
        guiManager.goToMenu += delegate
        {
            Destroy(_gameInfo);
        };
    }
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (guiManager.inGame)
            {
                _currentPLayerCS.PauseGame(true);
                currentMonster.PauseGame(true);
                _currentPLayerCS.enabled = false;
                currentMonster.enabled = false;
                UnityEngine.Cursor.visible = true;
                guiManager.inGame = false;
                Time.timeScale = 0;
            }
            else
            {
                UnityEngine.Cursor.visible = false;
                guiManager.inGame = true;
                _currentPLayerCS.PauseGame(false);
                currentMonster.PauseGame(false);
                _currentPLayerCS.enabled = true;
                currentMonster.enabled = true;
                Time.timeScale = 1;
            }
        }
    }
}
