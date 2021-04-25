using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private EnemyMovementType  _enemyMovementType  = EnemyMovementType.Default;
    [SerializeField] private float              _speed              = 4f;
    [SerializeField] private Transform[]        _bossPositions      = null;
    [SerializeField] private float              _startWaitTime      = 0f;

    private Transform   _target = null;
    private Vector3     _right  = Vector3.right;
    private Vector3     _left   = Vector3.left;

    private int     _randomSide                 = 0;
    private int     _randomPoint                = 0;
    private int     _randomSideJuke             = 0;
    private float   _switchSideRate             = 0f;
    private float   _minSeconds                 = 2f;
    private float   _maxSeconds                 = 3f;
    private float   _waitTime                   = 0f;
    private float   _chanceToChangeDirection    = 0.005f;

    private bool _canSwitch     = false;
    private bool _isHovering    = false;
    private bool _isPatrolling  = false;
    private bool _isAggressive  = false;
    private bool _isLeftRight   = false;
    private bool _isSwitching   = false;

    private Player  _player     = null;
    private Enemy   _enemy      = null;

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

        _bossPositions = GameObject.Find("BossPositions").transform.GetComponentsInChildren<Transform>();
    }

    private void Start()
    {
        _waitTime = _startWaitTime;

        if (_enemy.GetEnemyType() == EnemyType.Infantry)
            InitInfantryMovementType();
        else if (_enemy.GetEnemyType() == EnemyType.Assault)
            InitAssaultMovementType();
        else if (_enemy.GetEnemyType() == EnemyType.Aggressor)
            InitAggressorMovementType();

        StartCoroutine(SideSwitcherRoutine());
        
        if(_isLeftRight)
            StartCoroutine(GoLeftRightRoutine());

        _randomPoint = Random.Range(0, _bossPositions.Length);
    }

    private void Update()
    {
        Movement();

        if (!_isSwitching)
        {
            _randomSideJuke = Random.Range(1, 3);
        }
    }

    private void Movement()
    {
        if (_enemyMovementType == EnemyMovementType.Default)
        {
            transform.Translate(Vector3.down.normalized * _speed * Time.deltaTime);
        }
        
        if (_enemyMovementType == EnemyMovementType.ZigZag)
        {
            if (!_canSwitch)
            {
                _randomSide = Random.Range(1, 3);
                _canSwitch = true;
            }

            if (_randomSide == 1)
                transform.Translate((Vector3.down + _right).normalized * _speed * Time.deltaTime);
            else if (_randomSide == 2)
                transform.Translate((Vector3.down + _left).normalized * _speed * Time.deltaTime);
        }
        
        if(_enemyMovementType == EnemyMovementType.Hover)
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

        if(_enemyMovementType == EnemyMovementType.Follow)
        {
            if (_target == null) return;

            if (Vector3.Distance(transform.position, _target.position) > .5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
            }  
        }

        if(_enemyMovementType == EnemyMovementType.Aggressive)
        {
            _isAggressive = true;
            transform.Translate(Vector3.down * (_speed * 2) * Time.deltaTime);
        }
        else
        {
            _isAggressive = false;
        }


        if (_enemyMovementType == EnemyMovementType.Patrol)
        {
            _isPatrolling = true;

            Vector3 pos = transform.position;
            pos = Vector3.MoveTowards(transform.position, _bossPositions[_randomPoint].position, _speed * Time.deltaTime);
            transform.position = pos;

            if (Vector3.Distance(pos, _bossPositions[_randomPoint].position) < 0.2f)
            {
                if (_waitTime <= 0)
                {
                    _randomPoint = Random.Range(0, _bossPositions.Length);
                    _waitTime = _startWaitTime;
                }
                else
                {
                    _waitTime -= Time.deltaTime;
                }
            }
        }

        if(_enemyMovementType == EnemyMovementType.ToAnchorPoint)
        {
            _isPatrolling = false;

            Vector3 pos = transform.position;

            if (pos.y >= _bossPositions[1].position.y || pos.y <= _bossPositions[1].position.y)
            {
                pos = Vector3.MoveTowards(transform.position, _bossPositions[1].position, _speed * Time.deltaTime);
            }
            else
            {
                pos.y = _bossPositions[1].position.y;
            }

            if (transform.position.y == 4f)
            {
                _enemyMovementType = EnemyMovementType.Patrol;
            }

            transform.position = pos;
        }

        if (_enemyMovementType == EnemyMovementType.Panic)
        {
            _isPatrolling = false;

            Vector3 pos = transform.position;

            if (pos.y >= _bossPositions[1].position.y || pos.y <= _bossPositions[1].position.y)
            {
                pos = Vector3.MoveTowards(pos, _bossPositions[1].position, _speed * Time.deltaTime);
            }
            
            if(pos.y == _bossPositions[1].position.y)
            {
                pos = new Vector3(transform.position.x, 4f, 0);

                _isLeftRight = true;

                SetMovementType(EnemyMovementType.LeftRight);
            }

            transform.position = pos;
        }

        if (_enemyMovementType == EnemyMovementType.LeftRight)
        {
            Vector3 pos = transform.position;
            pos.x += _speed * Time.deltaTime;
            transform.position = pos;

            if (_isLeftRight)
            {
                if (pos.x < -10f)
                {
                    _speed = Mathf.Abs(_speed);
                }
                else if (pos.x > 10f)
                {
                    _speed = -Mathf.Abs(_speed);
                }
                else if (Random.value < _chanceToChangeDirection)
                {
                    _speed *= -1;
                }
            }
        }

        if(_enemyMovementType == EnemyMovementType.Juke)
        {
            if(_randomSideJuke == 1)
            {
                if(_isSwitching)
                    transform.Translate((Vector3.right + Vector3.down) * (_speed + 1) * Time.deltaTime);
            }
            else if(_randomSideJuke == 2)
            {
                if(_isSwitching)
                    transform.Translate((Vector3.left + Vector3.down) * (_speed + 1) * Time.deltaTime);
            }
        }

        if (transform.position.y < -8f && _enemy.GetEnemyType() != EnemyType.FinalBoss && _enemy.GetEnemyType() != EnemyType.Carrier)
        {
            if (_player.IsDead()) return;
            if (_enemy.GetLives() > 1)
            {
                _enemy.SetLives();
            }

            float randomXposition = Random.Range(-11, 11);
            transform.position = new Vector3(randomXposition, 8f, 0);
        }
        
        if(transform.position.y < -11f && _enemy.GetEnemyType() == EnemyType.Carrier)
        {
            float randomXposition = Random.Range(-11, 11);
            transform.position = new Vector3(randomXposition, 10.6f, 0);
        }

        EnemyHorizontalScreenWrap();
    }

    private IEnumerator GoLeftRightRoutine()
    {
        float _randomTime = Random.Range(1, 5);
        yield return new WaitForSeconds(_randomTime);
    }

    private void InitAggressorMovementType()
    {
        int randomIntValue = Random.Range(0, 2);
        if (randomIntValue == 0)
            _enemyMovementType = EnemyMovementType.ZigZag;
        else if (randomIntValue == 1)
            _enemyMovementType = EnemyMovementType.Default;
    }

    private void InitInfantryMovementType()
    {
        int randomIntValue = Random.Range(0, 2);
        if (randomIntValue == 0)
            _enemyMovementType = EnemyMovementType.Default;
        else if (randomIntValue == 1)
            _enemyMovementType = EnemyMovementType.ZigZag;
    }

    private void InitAssaultMovementType()
    {
        int randomIntValue = Random.Range(0, 2);
        if (randomIntValue == 0)
            _enemyMovementType = EnemyMovementType.ZigZag;
        else if (randomIntValue == 1)
            _enemyMovementType = EnemyMovementType.Hover;
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

    public EnemyMovementType SetMovementType(EnemyMovementType type)
    {
        _enemyMovementType = type;
        return type;
    }

    public EnemyMovementType GetMovementType()
    {
        return _enemyMovementType;
    }

    public bool IsPatrolling()
    {
        return _isPatrolling;
    }

    public bool SetIsSwitching(bool isSwitching)
    {
        _isSwitching = isSwitching;
        return _isSwitching;
    }
}
