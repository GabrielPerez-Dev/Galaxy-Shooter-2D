using System.Collections;
using UnityEngine;

public enum EnemyType { Default, }

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyType  _enemyType      = EnemyType.Default;
    [SerializeField] private GameObject _laserPrefab    = null;
    [SerializeField] private GameObject _shieldPrefab   = null;
    [SerializeField] private int        _lives          = 1;
    [SerializeField] private int        _maxLives       = 1;
    [SerializeField] private int        _giveDamage     = 1;
    [SerializeField] private int        _givePoints     = 10;
    [SerializeField] private Collider2D _collider       = null;

    private EnemyMovement _movement = null;
    private AudioManager _audioManager = null;
    private Flasher _flasher = null;
    private Animator _animator = null;
    private Player _player = null;

    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    private bool _isDead = false;
    private bool _isShieldActive = false;


    private void Awake()
    {
        _movement = GetComponent<EnemyMovement>();
        if (_movement == null)
            Debug.Log("EnemyMovement is null");

        _animator = GetComponentInChildren<Animator>();
        if (_animator == null) 
            Debug.LogError("The Animator is NULL");

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
            _isShieldActive = true;
            _shieldPrefab.SetActive(true);
        }
    }

    private void Update()
    {
        if(Time.time > _canFire && !_isDead)
        {
            _fireRate = Random.Range(3f, 6f);
            _canFire = Time.time + _fireRate;

            Vector3 offsetY = new Vector3(0, -0.5f, 0);
            var enemeyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Projectile[] lasers = enemeyLaser.GetComponentsInChildren<Projectile>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].IsEnemyLaser();
            }
        }
    }

    private void TakeDamage(int amount)
    {
        _flasher.FlashWhenHit();

        _lives -= amount;

        if (_lives <= 0)
        {
            _isDead = true;
            Destroy(gameObject, 2f);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
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

            if (_isShieldActive)
            {
                _isShieldActive = false;
                _shieldPrefab.SetActive(false);
                return;
            }

            _animator.SetTrigger("isDestroyed");
            _movement.Speed = 1f;
            _audioManager.PlayExplosionSound();
            TakeDamage(1);
        }

        if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);

            if (_isShieldActive)
            {
                _isShieldActive = false;
                _shieldPrefab.SetActive(false);
                return;
            }

            _isDead = true;

            if (_player != null)
            {
                _player.AddScore(_givePoints);
            }

            _animator.SetTrigger("isDestroyed");
            _movement.Speed = 1f;
            _audioManager.PlayExplosionSound();

            _collider.enabled = false;

            TakeDamage(1);
        }

        if(other.gameObject.CompareTag("Enemy Projectile"))
        {
            return;
        }
    }
}
