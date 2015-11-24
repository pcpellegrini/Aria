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
    public GameObject monsterParent;
    public GameObject littleMonsterPrefab;
    public GuiManager guiManager;
    public Monster currentMonster;
    public GameObject aimHUD;
    public AudioClip levelMusic;
    public AudioSource monsterSource;
    public Transform[] spawnPoints;

    private AirCraft _currentPLayerCS;
    private Info _gameInfo;
    private GameObject _currentPLayer;
    private AudioSource _audioSource;
    private int _currentMonsterRelease = 5;
    private int _monsterCount = 0;
    private int _maxMonster = 50;

    private List<Bullet> _bullets = new List<Bullet>();
    private List<Monster> _monsterLittle = new List<Monster>();
    List<GameObject> _bulletsGO = new List<GameObject>();
    List<GameObject> _monsterLittleGO = new List<GameObject>();

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
        
        for (int i = 0; i < 30; i++)
        {
            _monsterLittleGO.Add(Instantiate(littleMonsterPrefab, new Vector3(-1000, -1000, -1000), Quaternion.identity) as GameObject);
        }
        for (int i = 0; i < _monsterLittleGO.Count; i++)
        {
            int __num = i;
            _monsterLittle.Add(_monsterLittleGO[__num].GetComponent<Monster>());
            //_monsterLittle[__num].gameObject.transform.parent = monsterParent.transform;
            _monsterLittle[__num].onDisable += delegate
            {
                _monsterCount--;
            };
            _monsterLittle[__num].isActive = false;
            _monsterLittle[__num].enabled = false;
            _monsterLittle[__num].ManualStart(_currentPLayerCS);
        }

        _currentPLayerCS.soundController.monsterSound = monsterSource;
        _currentPLayerCS.ManualStart(_bullets, InstPanel, guiManager, aimHUD, _audioSource, levelMusic);
        currentMonster.onEnergyChange += delegate (float p_energy)
        {
            guiManager.ChangeValue("monsterEnergy", p_energy);
        };
        guiManager.inGame = true;
        _currentPLayerCS.TPSCameraController.monster = currentMonster.gameObject;
        _currentPLayerCS.FPSCameraController.monster = currentMonster.gameObject;
        currentMonster.ManualStart(_currentPLayerCS);
        guiManager.monsterEnergy.maxValue = currentMonster.energy;
        guiManager.monsterEnergy.value = currentMonster.energy;
        guiManager.goToMenu += delegate
        {
            Destroy(_gameInfo);
        };
        InvokeRepeating("ReleaseMonsters", 10f, 20f);
        if (currentMonster.type == Monster.monsterType.AERIAL)
        {
            currentMonster.islands = spawnPoints;
        }
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

    private void ReleaseMonsters()
    {
        for (int j = 0; j < _currentMonsterRelease; j++)
        {
            int __num2 = j;
            for (int i = 0; i < _monsterLittle.Count; i++)
            {
                int __num = i;
                if (!_monsterLittle[__num].isActive && _monsterCount < _maxMonster)
                {
                    if (_gameInfo.selectedLevel == "01")
                    {
                        _monsterLittle[__num].gameObject.transform.position = currentMonster.vulcanExit.position + (Random.insideUnitSphere * 30);
                    }
                    else
                    {
                        _monsterLittle[__num].gameObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position + (Random.insideUnitSphere * 5);
                    }                    
                    _monsterLittle[__num].isActive = true;
                    _monsterLittle[__num].enabled = true;
                    _monsterLittle[__num].Enable();
                    _monsterCount++;
                    break;
                }
            }
        }
    }
}
