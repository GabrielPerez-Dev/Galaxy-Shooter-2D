using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyType    _enemyType          = EnemyType.None;
    [SerializeField] protected GameObject   _explosionPrefab    = null;
    [SerializeField] protected GameObject   _shieldPrefab       = null;
    [SerializeField] protected int          _lives              = 1;
    [SerializeField] private int            _maxLives           = 1;
    [SerializeField] private int            _giveDamage         = 1;
    [SerializeField] private int            _givePoints         = 10;
    [SerializeField] private Collider2D     _collider           = null;
    [SerializeField] protected float        _fireRate           = 3.0f;

    protected EnemyMovement _movement = null;
    private AudioManager _audioManager = null;
    protected Flasher _flasher = null;
    protected Player _player = null;

    protected float _canFire = -1f;

    protected bool _isDead = false;
    private bool _isShieldActive = false;

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

        _flasher = GetComponent<Flasher>();
        if (_flasher == null)
            Debug.LogError("Flasher is null");
    }

    private void Start()
    {
        _lives = _maxLives;

        float shieldProbability = 0.2f;
        if (Random.value < shieldProbability)
        {
            if (_enemyType != EnemyType.Carrier && _enemyType != EnemyType.Drone)
            {
                _isShieldActive = true;
                _shieldPrefab.SetActive(true);
            }
        }
    }

    private void Update()
    {
        OnAttack();
    }

    protected abstract void OnAttack();

    private void TakeDamage(int amount)
    {
        if (_isShieldActive)
        {
            if(_enemyType != EnemyType.Carrier && _enemyType != EnemyType.Drone)
            {
                _isShieldActive = false;
                _shieldPrefab.SetActive(false);
            }

            return;
        }

        _flasher.FlashWhenHit();

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

    public EnemyType GetEnemyType()
    {
        return _enemyType;
    }
}
