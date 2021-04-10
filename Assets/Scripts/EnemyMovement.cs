using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMovemenType { Default, ZigZag, }

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyMovemenType _enemyMovementType = EnemyMovemenType.Default;
    [SerializeField] private float _speed = 4f;

    private Vector3 _right = Vector3.right;
    private Vector3 _left = Vector3.left;

    private int randomSide = 0;
    private float _switchSideRate = 0f;
    private float _minSeconds = 2f;
    private float _maxSeconds = 3f;
    private bool _canSwitch = false;

    private Player _player = null;

    public float Speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    private void Awake()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.Log("Player is null");
    }

    private void Start()
    {
        InitMovementType();

        StartCoroutine(SideSwitcherRoutine());

        float randomValue = Random.Range(1.5f, 3f);
        _speed = randomValue;
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (_enemyMovementType == EnemyMovemenType.Default)
        {
            transform.Translate(Vector3.down.normalized * _speed * Time.deltaTime);
        }
        else if (_enemyMovementType == EnemyMovemenType.ZigZag)
        {
            if (!_canSwitch)
            {
                randomSide = Random.Range(1, 3);
                _canSwitch = true;
            }

            if (randomSide == 1)
                transform.Translate((Vector3.down + _right).normalized * _speed * Time.deltaTime);
            else if (randomSide == 2)
                transform.Translate((Vector3.down + _left).normalized * _speed * Time.deltaTime);
        }

        if (transform.position.y < -8f)
        {
            if (_player.IsDead()) return;

            float randomXposition = Random.Range(-11, 11);
            transform.position = new Vector3(randomXposition, 8f, 0);
        }

        EnemyHorizontalScreenWrap();
    }

    private void InitMovementType()
    {
        int randomIntValue = Random.Range(0, 2);
        if (randomIntValue == 0)
            _enemyMovementType = EnemyMovemenType.Default;
        else if (randomIntValue == 1)
            _enemyMovementType = EnemyMovemenType.ZigZag;
    }

    private void EnemyHorizontalScreenWrap()
    {
        if (transform.position.x >= 13f)
        {
            transform.position = new Vector3(-13, transform.position.y, 0);
        }
        else if (transform.position.x <= -13f)
        {
            transform.position = new Vector3(13, transform.position.y, 0);
        }
    }

    private IEnumerator SideSwitcherRoutine()
    {
        _switchSideRate = Random.Range(_minSeconds, _maxSeconds);

        yield return new WaitForSeconds(_switchSideRate);

        _canSwitch = false;
    }
}
