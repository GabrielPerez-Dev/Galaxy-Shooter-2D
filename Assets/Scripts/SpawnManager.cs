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
    private GameObject enemyInstance = null;
    private EnemyType _enemyType = EnemyType.None;

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

            var randomValue = Random.value;

            if (randomValue > 0 && randomValue < 0.5f) //Infantry
                enemyInstance = Instantiate(_enemyPrefabs[0], randomXposition, Quaternion.identity);
            else if (randomValue > 0.5f && randomValue <= 0.9f) //Assault
                enemyInstance = Instantiate(_enemyPrefabs[1], randomXposition, Quaternion.identity);
            else if (randomValue > 0.9f && randomValue <= 1f) // Carrier
            {
                Vector3 offSetY = new Vector3(0, 2, 0);
                enemyInstance = Instantiate(_enemyPrefabs[2], randomXposition + offSetY, Quaternion.identity);
            }

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

            PowerupProbabilityCheck(randomXposition);

            float randomTime = Random.Range(6, 20);
            yield return new WaitForSeconds(randomTime);
        }
    }

    private void PowerupProbabilityCheck(Vector3 randomXposition)
    {
        var randomValue = Random.value;

        if (randomValue > 0f && randomValue <= .05f)                             //Gods Wish
            Instantiate(_powerupPrefabs[0], randomXposition, Quaternion.identity);
        else if (randomValue > 0.05f && randomValue <= 0.2f)                     //Shields
            Instantiate(_powerupPrefabs[1], randomXposition, Quaternion.identity);
        else if (randomValue > 0.2f && randomValue <= 0.4f)                      //Health
            Instantiate(_powerupPrefabs[2], randomXposition, Quaternion.identity);
        else if (randomValue > 0.04f && randomValue <= 0.6f)                      //TripleShot
            Instantiate(_powerupPrefabs[3], randomXposition, Quaternion.identity);
        else if (randomValue > 0.6f && randomValue <= 1f)                        //Ammo
            Instantiate(_powerupPrefabs[4], randomXposition, Quaternion.identity);

        if (_enemyType == EnemyType.Carrier)
        {
            Instantiate(_powerupPrefabs[0], randomXposition, Quaternion.identity);
        }

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
}
