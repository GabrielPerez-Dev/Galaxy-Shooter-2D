using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefabs = null;
    [SerializeField] private GameObject[] _powerupPrefabs = null;
    [SerializeField] private GameObject[] _asteroidPrefabs = null;
    [SerializeField] private GameObject _enemyContainer = null;
    [SerializeField] private float _spawnTime = 5f;

    private GameManager _gameManager = null;

    private bool _stopSpawning = false;

    private float _waitToSpawn = 5f;

    private void Awake()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        if (_gameManager == null)
            Debug.Log("GameManager is null");
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
        StartCoroutine(SpawnAsteroidRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        if (_gameManager.IsNewScene)
        {
            yield return new WaitForSeconds(_waitToSpawn);
        }

        while (!_stopSpawning)
        {
            float randomX = Random.Range(-11, 11);
            Vector3 randomXposition = new Vector3(randomX, 8, 0);

            var enemyInstance = Instantiate(GetEnemyGameObject(), randomXposition, Quaternion.identity);

            enemyInstance.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(_spawnTime);
        }
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        if (_gameManager.IsNewScene)
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

    private GameObject GetEnemyGameObject()
    {
        float sum = 0f;
        foreach (var enemyGO in _enemyPrefabs)
        {
            var enemy = enemyGO.GetComponent<Enemy>();
            sum += enemy.GetSpawnRate();
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

        foreach (var enemyGO in _enemyPrefabs)
        {
            var enemy = enemyGO.GetComponent<Enemy>();
            if (randomRate < enemy.GetSpawnRate())
                return enemyGO;
            randomRate -= enemy.GetSpawnRate();
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
        if (_gameManager.IsNewScene)
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

    public void StopSpawning()
    {
        _stopSpawning = true;
    }

    public GameObject[] GetPowerupPrefabs()
    {
        return _powerupPrefabs;
    }
}
