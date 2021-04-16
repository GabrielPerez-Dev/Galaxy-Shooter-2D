using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyMovemenType _enemyMovementType = EnemyMovemenType.Default;
    [SerializeField] private float _speed = 4f;

    private Transform _target = null;
    private Vector3 _right = Vector3.right;
    private Vector3 _left = Vector3.left;

    private int randomSide = 0;
    private float _switchSideRate = 0f;
    private float _minSeconds = 2f;
    private float _maxSeconds = 3f;

    private bool _canSwitch = false;
    private bool _isHovering = false;
    private bool _isAggressive = false;

    private Player _player = null;
    private Enemy _enemy = null;

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

        _enemy = GetComponent<Enemy>();
        if (_enemy == null)
            Debug.Log("Enemy is null");

        _target = GameObject.Find("Player").transform;
        if (_target == null)
            Debug.Log("Target is null");

    }

    private void Start()
    {
        if (_enemy.GetEnemyType() == EnemyType.Infantry)
            InitInfantryMovementType();
        else if (_enemy.GetEnemyType() == EnemyType.Assault)
            InitAssaultMovementType();
        else if (_enemy.GetEnemyType() == EnemyType.Aggressor)
            InitAggressorMovementType();

        StartCoroutine(SideSwitcherRoutine());

        float randomValue = Random.Range(1.5f, 3f);
        //_speed = randomValue;
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
        
        if (_enemyMovementType == EnemyMovemenType.ZigZag)
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
        
        if(_enemyMovementType == EnemyMovemenType.Hover)
        {
            if (transform.position.y > 4f)
            {
                transform.Translate(Vector3.down.normalized * _speed * Time.deltaTime);
                _isHovering = true;
            }
            else if (_isHovering)
            {
                Vector3 newPos = new Vector3(transform.position.x, 4f, 0);
                transform.position = newPos;

                StartCoroutine(HoverRoutine());
            }
            else 
            {
                transform.Translate(Vector3.down.normalized * 5 * Time.deltaTime);
            }
        }

        if(_enemyMovementType == EnemyMovemenType.Follow)
        {
            if (_target == null) return;

            if (Vector3.Distance(transform.position, _target.position) > 1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
            }  
        }

        if(_enemyMovementType == EnemyMovemenType.Aggressive)
        {
            _isAggressive = true;
            transform.Translate(Vector3.down * (_speed * 2) * Time.deltaTime);
        }
        else
        {
            _isAggressive = false;
        }

        if (transform.position.y < -8f)
        {
            if (_player.IsDead()) return;
            if (_enemy.GetLives() > 1)
            {
                _enemy.SetLives();
            }

            float randomXposition = Random.Range(-11, 11);
            transform.position = new Vector3(randomXposition, 8f, 0);
        }
        else if(transform.position.y < -10.3f && _enemy.GetEnemyType() == EnemyType.Carrier)
        {
            Destroy(gameObject);
        }

        EnemyHorizontalScreenWrap();
    }

    private void InitAggressorMovementType()
    {
        int randomIntValue = Random.Range(0, 2);
        if (randomIntValue == 0)
            _enemyMovementType = EnemyMovemenType.ZigZag;
        else if (randomIntValue == 1)
            _enemyMovementType = EnemyMovemenType.Default;
    }

    private void InitInfantryMovementType()
    {
        int randomIntValue = Random.Range(0, 2);
        if (randomIntValue == 0)
            _enemyMovementType = EnemyMovemenType.Default;
        else if (randomIntValue == 1)
            _enemyMovementType = EnemyMovemenType.ZigZag;
    }

    private void InitAssaultMovementType()
    {
        int randomIntValue = Random.Range(0, 2);
        if (randomIntValue == 0)
            _enemyMovementType = EnemyMovemenType.ZigZag;
        else if (randomIntValue == 1)
            _enemyMovementType = EnemyMovemenType.Hover;
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

    private IEnumerator HoverRoutine()
    {
        float randomTime = Random.Range(3f, 6f);

        while (_isHovering)
        {
            yield return new WaitForSeconds(randomTime);
            _isHovering = false;
        }
    }

    public bool IsHovering()
    {
        return _isHovering;
    }

    public bool IsAggressive()
    {
        return _isAggressive;
    }

    public EnemyMovemenType SetMovementType(EnemyMovemenType type)
    {
        _enemyMovementType = type;
        return type;
    }
}
