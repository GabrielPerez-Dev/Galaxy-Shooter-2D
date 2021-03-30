using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText = null;
    [SerializeField] private TextMeshProUGUI _gameOverText = null;
    [SerializeField] private TextMeshProUGUI _restartText = null;
    [SerializeField] private Image _livesImg = null;
    [SerializeField] private Sprite[] _livesSprites = null;

    private GameManager _gameManager;
    private Player _player = null;

    private void Awake()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null) Debug.Log("GameManage script is null");

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) Debug.Log("Player script is null");
    }

    private void Start()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
    }

    private void Update()
    {
        _scoreText.text = "Score: " + _player.GetScore().ToString();
        _livesImg.sprite = _livesSprites[_player.GetLives()];

        if(_player.GetLives() == 0)
        {
            GameOverSequence();
        }
    }

    private void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
    }
}
