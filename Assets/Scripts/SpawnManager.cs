using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab = null;
    [SerializeField] private GameObject _enemyContainer = null;
    [SerializeField] private float _waitTime = 5f;
    private Player _player;
    private bool _stopSpawning = false;

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (!_stopSpawning)
        {
            float randomX = Random.Range(-11, 11);
            Vector3 randomXposition = new Vector3(randomX, 8, 0);
            GameObject enemyInstance = Instantiate(_enemyPrefab, randomXposition, Quaternion.identity);
            enemyInstance.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_waitTime);
        }
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }
}
