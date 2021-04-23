using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _powerupPrefabs = null;
    [SerializeField] private GameObject[] _asteroidPrefabs = null;
    [SerializeField] private GameObject _enemyContainer = null;

    private GameManager _gameManager = null;
    private UIManager _uiManager = null;
    private bool _stopSpawning = false;

    private float _waitToSpawn = 4f;

    [Header("Wave Settings")]
    [SerializeField] private Wave[] _waves;
    private int _currentWave = 0;
    private int _totalWaves = 0;

    private int _totalEnemiesInCurrentWave = 0;
    private int _enemiesLeftInWave = 0;
    private int _enemiesSpawned = 0;

    private float _timeBetweenEnemies = 2f;

    private AudioManager _audioManager = null;

    private void Awake()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.Log("GameManager is null");

        _uiManager = GameObject.Find("UI_Manager").GetComponent<UIManager>();
        if (_uiManager == null)
            Debug.Log("UIManager is null");

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
            Debug.Log("AudioManager is null");
    }

    private void Start()
    {
        _currentWave = -1;
        _totalWaves = _waves.Length - 1; // Minus 1 because we are using 0 index

        StartNextWave();

        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnAsteroidRoutine());
    }

    private void StartNextWave()
    {
        _currentWave++;

        if (_currentWave > _totalWaves)
        {
            _uiManager.VictoryTextActive();
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                _audioManager.PlayDroneExplosionSound();
                
                Destroy(enemies[i].gameObject);
            }

            Time.timeScale = 0;

            return;
        }

        _totalEnemiesInCurrentWave = _waves[_currentWave].GetNumberOfEnemiesPerWave();
        _enemiesLeftInWave = 0;
        _enemiesSpawned = 0;

        StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        if (_gameManager.IsNewWave)
        {
            StartCoroutine(_uiManager.StartWaveRoutine());
            yield return new WaitForSeconds(_waitToSpawn);
            _gameManager.IsNewWave = false;
        }

        while (!_stopSpawning && !_gameManager.IsNewWave && _enemiesSpawned < _totalEnemiesInCurrentWave)
        {
            _enemiesSpawned++;
            _enemiesLeftInWave++;

            float randomX = Random.Range(-11, 11);
            Vector3 randomXposition = new Vector3(randomX, 8, 0);

            var enemyInstance = Instantiate(GetEnemyGOwithProbability(), randomXposition, Quaternion.identity); 

            if (enemyInstance != null)
                enemyInstance.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(_timeBetweenEnemies);
        }

        yield return null;
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        if (_gameManager.IsNewScene || _gameManager.IsNewWave)
        {
            yield return new WaitForSeconds(_waitToSpawn);
        }

        while (!_stopSpawning)
        {
            float randomX = Random.Range(-11, 11);
            Vector3 randomXposition = new Vector3(randomX, 8, 0);

            Instantiate(GetPowerupGameObject(), randomXposition, Quaternion.identity);

            float randomTime = Random.Range(6, 20);
            yield return new WaitForSeconds(randomTime);
        }
    }

    private GameObject GetEnemyGOwithProbability()
    {
        float sum = 0f;

        for (int i = 0; i < _waves.Length; i++)
        {
            var enemies = _waves[_currentWave].GetEnemies();
            for (int x = 0; x < enemies.Length; x++)
            {
                var enemyGO = enemies[x];
                var enemy = enemyGO.GetComponent<Enemy>();
                sum += enemy.GetSpawnRate();
            }
        }

        float randomRate = 0f;
        do
        {
            // no rate on any number?
            if (sum == 0)
                return null;
            randomRate = Random.Range(0, sum);
        }
        while (randomRate == sum);

        for (int i = 0; i < _waves.Length; i++)
        {
            var enemies = _waves[_currentWave].GetEnemies();
            for (int x = 0; x < enemies.Length; x++)
            {
                var enemyGO = enemies[x];
                var enemy = enemyGO.GetComponent<Enemy>();
                if (randomRate < enemy.GetSpawnRate())
                    return enemyGO;
                randomRate -= enemy.GetSpawnRate();
            }
        }

        return null;
    }

    private GameObject GetPowerupGameObject()
    {
        // Add up all the spawn rate to a sum
        float sum = 0f;
        foreach (var powerupGO in _powerupPrefabs)
        {
            var powerup = powerupGO.GetComponent<PowerUp>();
            sum += powerup.GetSpawnRate();
        }

        // Generate a random number between 0 and sum
        float randomRate = 0f;
        do
        {
            // no weight on any number?
            if (sum == 0)
                return null;
            randomRate = Random.Range(0, sum);
        }
        while (randomRate == sum);

        // Go through all numbers and check if the random number is less than the rate of the powerup. 
        //If not, then subtract the weight
        foreach (var powerupGO in _powerupPrefabs)
        {
            var powerup = powerupGO.GetComponent<PowerUp>();
            if (randomRate < powerup.GetSpawnRate())
                return powerupGO;
            randomRate -= powerup.GetSpawnRate();
        }

        return null;
    }

    private IEnumerator SpawnAsteroidRoutine()
    {
        if (_gameManager.IsNewScene || _gameManager.IsNewWave)
        {
            yield return new WaitForSeconds(_waitToSpawn);
        }

        while (!_stopSpawning)
        {
            float randomX = Random.Range(-11, 11);
            Vector3 randonXposition = new Vector3(randomX, 9, 0);

            int randomAsteroid = Random.Range(0, _asteroidPrefabs.Length);
            Instantiate(_asteroidPrefabs[randomAsteroid], randonXposition, Quaternion.identity);

            float randomTime = Random.Range(6, 20);
            yield return new WaitForSeconds(randomTime);
        }
    }

    public void EnemyDefeated()
    {
        _enemiesLeftInWave--;

        if(_enemiesLeftInWave == 0 && _enemiesSpawned == _totalEnemiesInCurrentWave)
        {
            _gameManager.IsNewWave = true;
            StartNextWave();
        }
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }

    public int GetCurrentWave()
    {
        return _currentWave + 1;
    }
}
