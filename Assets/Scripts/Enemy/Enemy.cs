using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyType    _enemyType          = EnemyType.None;
    [SerializeField] protected GameObject   _explosionPrefab    = null;
    [SerializeField] protected GameObject   _shieldPrefab       = null;
    [SerializeField] protected bool         _isShieldActive     = false;
    [SerializeField] protected int          _shieldStrength     = 0;
    [SerializeField] private float          _shieldProbability  = 0.2f;
    [SerializeField] private float          _spawnRate          = 0f;
    [SerializeField] protected int          _lives              = 1;
    [SerializeField] private int            _maxLives           = 1;
    [SerializeField] private int            _giveDamage         = 1;
    [SerializeField] private int            _givePoints         = 10;
    [SerializeField] private Collider2D     _collider           = null;
    [SerializeField] protected float        _fireRate           = 3.0f;
    [SerializeField] private bool           _isFinalBoss        = false;

    protected EnemyMovement     _movement       = null;
    protected SpawnManager      _spawnManager   = null;
    protected AudioManager      _audioManager   = null;
    protected Flash             _flash        = null;
    protected Player            _player         = null;

    protected float             _canFire        = -1f;

    protected bool              _isDead         = false;

    private void Awake()
    {
        _movement = GetComponent<EnemyMovement>();
        if (_movement == null)
            Debug.Log("EnemyMovement is null");

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) 
            Debug.LogError("The Player is NULL");

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
            Debug.LogError("AudioManager is null");

        _flash = GetComponent<Flash>();
        if (_flash == null)
            Debug.LogError("Flasher is null");

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("SpawnManager is null");
    }

    private void Start()
    {
        _lives = _maxLives;

        if (Random.value < _shieldProbability)
        {
            if (_enemyType != EnemyType.Carrier && _enemyType != EnemyType.Drone)
            {
                _isShieldActive = true;
                _shieldPrefab.SetActive(true);
            }
        }

        if (_isShieldActive)
        {
            _shieldPrefab.SetActive(true);
        }
    }

    private void Update()
    {
        OnAttack();
    }

    protected abstract void OnAttack();

    protected void TakeDamage(int amount)
    {
        if (_isShieldActive)
        {
            _shieldStrength--;

            if (_enemyType != EnemyType.Carrier && _enemyType != EnemyType.Drone && _shieldStrength == 0)
            {
                _isShieldActive = false;
                _shieldPrefab.SetActive(false);
            }

            return;
        }

        _flash.WhenHit();

        _lives -= amount;

        if (_lives <= 0)
        {
            DestroySequence();
        }
    }

    private void DestroySequence()
    {
        if (_player != null)
        {
            _player.AddScore(_givePoints);
        }

        _isDead = true;
        _collider.enabled = false;
        _movement.Speed = 1f;
        _audioManager.PlayExplosionSound();
        _spawnManager.EnemyDefeated();
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject, 0.2f);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage(_giveDamage);
                player.AddScore(-10);

                if(player.GetScore() <= 0)
                {
                    player.SetScore(0);
                }
            }

            TakeDamage(1);
        }

        if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);

            TakeDamage(1);
        }

        if (other.gameObject.CompareTag("Enemy Projectile"))
        {
            return;
        }
    }

    public int GetLives()
    {
        return _lives;
    }

    public void SetLives()
    {
        _lives = _maxLives;
    }

    public float GetSpawnRate()
    {
        return _spawnRate;
    }

    public EnemyType GetEnemyType()
    {
        return _enemyType;
    }

    public bool IsFinalBoss()
    {
        return _isFinalBoss;
    }
}
