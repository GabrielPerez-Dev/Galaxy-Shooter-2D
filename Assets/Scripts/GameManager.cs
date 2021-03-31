using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;
    private bool _isNewScene = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
        {
            RestartScene();
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(1); //Game scene
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public bool IsNewScene(bool isNewScene)
    {
        _isNewScene = isNewScene;
        return _isNewScene;
    }
}
