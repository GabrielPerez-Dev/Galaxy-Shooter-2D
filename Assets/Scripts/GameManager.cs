using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isNewScene = false;
    [SerializeField] private bool _isNewWave = false;

    private UIManager _uiManager = null;
    private bool isPause = false;
    private bool _isGameOver = false;
    private bool _wonGame = false;

    public bool IsNewScene 
    { 
        get { return _isNewScene; } 
        set{ _isNewScene = value; } 
    }

    public bool IsNewWave
    {
        get { return _isNewWave; }
        set { _isNewWave = value; }
    }

    private void Awake()
    {
        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.LogError("UIManager is NULL");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver || _wonGame)
        {
            RestartScene();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !isPause)
        {
            _uiManager.PausePanel(true);
            Time.timeScale = 0;
            isPause = true;
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && isPause)
        {
            _uiManager.PausePanel(false);
            Time.timeScale = 1;
            isPause = false;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1); //Game scene
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void WonGame()
    {
        _isGameOver = true;
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }
}
