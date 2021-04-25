using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI    _scoreText          = null;
    [SerializeField] private TextMeshProUGUI    _gameOverText       = null;
    [SerializeField] private TextMeshProUGUI    _restartText        = null;
    [SerializeField] private TextMeshProUGUI    _sceneStartText     = null;
    [SerializeField] private TextMeshProUGUI    _newWaveText        = null;
    [SerializeField] private TextMeshProUGUI    _ammoText           = null;
    [SerializeField] private TextMeshProUGUI    _missileText         = null;
    [SerializeField] private TextMeshProUGUI    _victoryText        = null;
    [SerializeField] private Image              _livesImg           = null;
    [SerializeField] private Image              _shieldsImg         = null;
    [SerializeField] private Image              _thrusterChargeImg  = null;
    [SerializeField] private Sprite[]           _livesSprites       = null;
    [SerializeField] private Sprite[]           _shieldsSprites     = null;
    [SerializeField] private GameObject         _pausePanel         = null;

    private float           _thrusterFill   = 0f;

    private GameManager     _gameManager    = null;
    private SpawnManager    _spawnManager   = null;
    private Player          _player         = null;
    private AudioManager    _audioManager   = null;

    private void Awake()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.Log("GameManage script is null");

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.Log("Player script is null");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("SpawnManage is null");

        _audioManager = GameObject.Find("Audio_Manager").GetComponentInChildren<AudioManager>();
        if (_audioManager == null)
            Debug.Log("AudioManager is null");
    }

    private void Start()
    {
        _pausePanel.SetActive(false);

        if(_gameManager.IsNewScene == true || _gameManager.IsNewWave == true)
        {
            _sceneStartText.gameObject.SetActive(true);
            _newWaveText.gameObject.SetActive(true);
        }
        else
        {
            _sceneStartText.gameObject.SetActive(false);
            _newWaveText.gameObject.SetActive(false);
        }

        if(_gameManager.IsNewScene)
            StartCoroutine(StartSceneTimerRoutine());

        if (_gameManager.IsNewWave)
        {
            StartCoroutine(StartWaveRoutine());
        }

        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
    }

    private void Update()
    {
        _scoreText.text = "Score: " + _player.GetScore().ToString();
        _livesImg.sprite = _livesSprites[_player.GetLives()];
        _shieldsImg.sprite = _shieldsSprites[_player.GetShields()];
        _ammoText.text = _player.GetAmmo().ToString() + " / " + _player.GetMaxAmmo().ToString();
        _missileText.text = _player.GetMissileCount().ToString() + " / " + _player.GetMaxMissile().ToString();

        _thrusterFill = _player.ThrusterCharge / _player.ThrusterMaxCharge;
        if(_thrusterChargeImg != null)
            _thrusterChargeImg.fillAmount = _thrusterFill;

        if(_player.GetLives() == 0 && _player.IsDead())
        {
            GameOverSequence();
        }
    }

    private void GameOverSequence()
    {
        _gameManager.GameOver();
        _audioManager.PlayGameOverSound();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
    }

    public void WonGameSequence()
    {
        _gameManager.WonGame();
        _audioManager.PlayVictoryMusicSound();
        _victoryText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
    }

    private IEnumerator StartSceneTimerRoutine()
    {
        _sceneStartText.text = "";
        yield return new WaitForSeconds(1f);
        _sceneStartText.text = "3";
        yield return new WaitForSeconds(1f);
        _sceneStartText.text = "2";
        yield return new WaitForSeconds(1f);
        _sceneStartText.text = "1";
        yield return new WaitForSeconds(1f);
        _sceneStartText.fontSize = 120;
        _sceneStartText.text = "GO!";
        yield return new WaitForSeconds(1f);
        _sceneStartText.gameObject.SetActive(false);

        _gameManager.IsNewScene = false;
    }

    public IEnumerator StartWaveRoutine()
    {
        _newWaveText.gameObject.SetActive(true);

        _newWaveText.text = "";
        yield return new WaitForSeconds(1f);
        _newWaveText.text = "Wave " + _spawnManager.GetCurrentWaveNumber();
        yield return new WaitForSeconds(2f);

        _newWaveText.fontSize = 120;
        _newWaveText.text = "GO!";
        yield return new WaitForSeconds(1f);

        _newWaveText.gameObject.SetActive(false);

        _gameManager.IsNewWave = false;
    }

    public void PausePanel(bool activate)
    {
        _pausePanel.SetActive(activate);
    }

    public void VictoryTextActive()
    {
        _victoryText.gameObject.SetActive(true);
    }

    public void GameOverDeActivate()
    {
        _gameOverText.gameObject.SetActive(false);
    }
}
