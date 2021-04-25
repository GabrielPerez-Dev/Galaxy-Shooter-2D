using UnityEngine;

[System.Serializable]
public class Wave
{
    [SerializeField] private string _waveName = "Wave 1";
    [SerializeField] private GameObject[] _enemies = null;
    [SerializeField] private int _numberOfEnemies;

    public int GetNumberOfEnemiesPerWave()
    {
        return _numberOfEnemies;
    }

    public GameObject[] GetEnemies()
    {
        return _enemies;
    }

    public string GetWaveName()
    {
        return _waveName;
    }
}

