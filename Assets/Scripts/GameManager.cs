using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _isNewScene;
    public bool IsNewScene { get { return _isNewScene; } set{ _isNewScene = value; } }
    private bool _isGameOver = false;

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
}
