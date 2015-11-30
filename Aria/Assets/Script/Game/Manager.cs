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
    public float timeToSpawnLittleMonsters = 20f;
    public Transform[] spawnPoints;
    public float[] timeEvents;

    private AirCraft _currentPLayerCS;
    private Info _gameInfo;
    private GameObject _currentPLayer;
    private AudioSource _audioSource;
    private int _currentMonsterRelease = 2;
    private int _monsterCount = 0;
    private int _spawnedMonsterCount = 0;
    private int _maxMonster = 20;
    private float timeCount = 600;
    private bool[] completedEvents = new bool[3];

    private List<Bullet> _bullets = new List<Bullet>();
    private List<Monster> _monsterLittle = new List<Monster>();
    private List<Monster> _monsterLittleUsed = new List<Monster>();
    private List<Monster> _monsterLittleFree = new List<Monster>();
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
                _monsterLittleUsed.Remove(_monsterLittle[__num]);
                _monsterLittleFree.Add(_monsterLittle[__num]);
            };
            _monsterLittleFree.Add(_monsterLittle[__num]);
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
        currentMonster.onPlayerEnterMonsterZone += delegate
        {
            for (int i = 0; i < _monsterLittle.Count; i++)
            {
                int __num = i;
                if (_monsterLittle[__num].isActive)
                {
                    if (_monsterLittle[__num].inPatrol)
                    {
                        _monsterLittle[__num].InvokeGoHome();
                        
                    }
                    else
                    {
                        _monsterLittle[__num].FollowPlayer(_currentPLayer);
                    }
                }
            }
        };
        currentMonster.onPlayerExitMonsterZone += delegate
        {
            for (int i = 0; i < _monsterLittle.Count; i++)
            {
                int __num = i;
                if (_monsterLittle[__num].isActive)
                {
                    if (_monsterLittle[__num].inPatrol)
                    {
                        _monsterLittle[__num].FollowPlayer(_currentPLayer);
                    }
                }
            }
        };
        currentMonster.eventOne += delegate
        {
            completedEvents[0] = true;
            _currentMonsterRelease = 7;
            currentMonster.timeBetweenAttacks -= 5;
            currentMonster.idleTime -= 5;
        };
        currentMonster.eventTwo += delegate
        {
            completedEvents[1] = true;
            _currentMonsterRelease = 10;
            currentMonster.timeBetweenAttacks -= 5;
            currentMonster.idleTime -= 5;
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
        StartCoroutine(ReleaseMonsters(timeToSpawnLittleMonsters));
        if (currentMonster.type == Monster.monsterType.AERIAL)
        {
            currentMonster.islands = spawnPoints;
        }
    }
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            PauseGame();
        }
        if (guiManager.inGame)
        {
            timeCount -= Time.deltaTime;
            guiManager.ChangeValue("time", Mathf.Round(timeCount));
        }
        if (timeCount <= timeEvents[0] && !completedEvents[0])
        {
            completedEvents[0] = true;
            _currentMonsterRelease = 7;
            currentMonster.timeBetweenAttacks -= 5;
            currentMonster.idleTime -= 5;
            timeToSpawnLittleMonsters += 5;
        }
        if (timeCount <= timeEvents[1] && !completedEvents[1])
        {
            completedEvents[1] = true;
            _currentMonsterRelease = 15;
            currentMonster.timeBetweenAttacks -= 5;
            currentMonster.idleTime -= 5;
            timeToSpawnLittleMonsters += 5;
        }

    }

    public void PauseGame()
    {
        if (guiManager.inGame)
        {
            for (int i = 0; i < _monsterLittleUsed.Count; i++)
            {
                int __num = i;
                _monsterLittleUsed[__num].enabled = false;
            }
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
            for (int i = 0; i < _monsterLittleUsed.Count; i++)
            {
                int __num = i;
                _monsterLittleUsed[__num].enabled = true;
            }
            UnityEngine.Cursor.visible = false;
            guiManager.inGame = true;
            _currentPLayerCS.PauseGame(false);
            currentMonster.PauseGame(false);
            _currentPLayerCS.enabled = true;
            currentMonster.enabled = true;
            Time.timeScale = 1;
        }
    }

    IEnumerator ReleaseMonsters(float p_time)
    {
        yield return new WaitForSeconds(p_time);
        for (int i = 0; i < _currentMonsterRelease; i++)
        {
            int __num = i;
            if (_monsterLittleFree.Count > 0)
            {
                _monsterLittleUsed.Add(_monsterLittleFree[0]);
                int __idx = _monsterLittleUsed.Count - 1;
                _monsterLittleFree.RemoveAt(0);
                if (_gameInfo.selectedLevel == "01")
                {
                    _monsterLittleUsed[__idx].gameObject.transform.position = currentMonster.vulcanExit.position + (Random.insideUnitSphere * 30);
                }
                else
                {
                    _monsterLittleUsed[__idx].gameObject.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position + (Random.insideUnitSphere * 5);
                }
                _monsterLittleUsed[__idx].isActive = true;
                _monsterLittleUsed[__idx].enabled = true;
                _monsterLittleUsed[__idx].Enable();
            }
        }
        //_currentMonsterRelease += 1;
        /*for (int i = 0; i < _monsterLittle.Count; i++)
            {
                int __num = i;
                if (!_monsterLittle[__num].isActive && _monsterCount < _maxMonster)
                {
                    _spawnedMonsterCount++;
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
                }
                if (_spawnedMonsterCount >= _currentMonsterRelease || _monsterCount >= _maxMonster)
                {
                    break;
                }
            }*/
            _spawnedMonsterCount = 0;
        StartCoroutine(ReleaseMonsters(timeToSpawnLittleMonsters));
    }
}
