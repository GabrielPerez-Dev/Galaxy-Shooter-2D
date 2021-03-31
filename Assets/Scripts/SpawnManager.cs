using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab = null;
    [SerializeField] private GameObject[] _powerupPrefabs = null;
    [SerializeField] private GameObject _enemyContainer = null;
    [SerializeField] private float _spawnTime = 5f;

    private GameManager _gameManager = null;
    private bool _stopSpawning = false;

    private void Awake()
    {
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
    }

    private void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        if (_gameManager.IsNewScene(true))
        {
            yield return new WaitForSeconds(4f);
        }

        while (!_stopSpawning)
        {
            float randomX = Random.Range(-11, 11);
            Vector3 randomXposition = new Vector3(randomX, 8, 0);

            GameObject enemyInstance = Instantiate(_enemyPrefab, randomXposition, Quaternion.identity);

            enemyInstance.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(_spawnTime);
        }
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        if (_gameManager.IsNewScene(true))
        {
            yield return new WaitForSeconds(4f);
        }

        while (!_stopSpawning)
        {
            float randomX = Random.Range(-11, 11);
            Vector3 randonXposition = new Vector3(randomX, 8, 0);

            int randomPowerUp = Random.Range(0, _powerupPrefabs.Length);
            Instantiate(_powerupPrefabs[randomPowerUp], randonXposition, Quaternion.identity);

            float randomTime = Random.Range(6, 20);
            yield return new WaitForSeconds(randomTime);
        }
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }
}
